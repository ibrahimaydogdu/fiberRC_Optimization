Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports ETABSv1
Imports FemCore
Imports FemRcModel

'V2 FEM <-> ETABS 22 comparison on the Ex1 trial design: the FemCore model (gross for gravity, cracked for modal / response spectrum) is rebuilt in ETABS through the OAPI
'with identical sections, rigid zones, diaphragms, patterns (beam loads copied from the FemCore patterns), mass source and TBDY spectrum.
'Compared: static member face forces (G, Q, S; gross and cracked), periods, base shear, joint displacements and member forces of the RS cases.
Module Program
    Dim ci As CultureInfo = CultureInfo.InvariantCulture
    Dim log As New System.Text.StringBuilder
    Dim dbgCount As Integer

    Sub P(s As String)
        Console.WriteLine(s) : log.AppendLine(s)
    End Sub

    Sub Chk(ret As Integer, what As String)
        If ret <> 0 Then P("ETABS ERROR ret=" & ret & " in " & what)
    End Sub

    Function Defs() As Dictionary(Of String, RcSectionDef)
        Dim d As New Dictionary(Of String, RcSectionDef)
        For Each pos In {"Corner", "Edge", "Inter"}
            d("C" & pos & "_S12") = New RcSectionDef With {.B = 500.0, .H = 500.0, .Fc = 30.0}
            d("C" & pos & "_S34") = New RcSectionDef With {.B = 400.0, .H = 400.0, .Fc = 30.0}
        Next
        For Each dd In {"x", "y"}
            For Each sg In {"_S12", "_S34"}
                d("B" & dd & sg) = New RcSectionDef With {.B = 300.0, .H = 600.0, .Fc = 30.0}
            Next
        Next
        Return d
    End Function

    Sub BuildEtabs(m As cSapModel, fm As FemModel, pts As Dictionary(Of Integer, String), frames As Dictionary(Of Integer, String))
        Chk(m.InitializeNewModel(eUnits.kN_m_C), "InitializeNewModel")
        Chk(m.File.NewGridOnly(4, 3.0, 3.0, 4, 3, 5.0, 5.0), "NewGridOnly")
        'materials and sections
        Dim mats As New HashSet(Of String)
        For Each s In fm.Sections
            If mats.Add(s.MaterialName) Then
                Chk(m.PropMaterial.SetMaterial(s.MaterialName, eMatType.Concrete), "SetMaterial")
                Chk(m.PropMaterial.SetMPIsotropic(s.MaterialName, s.E, 0.2, 0.00001), "SetMPIsotropic")
                Chk(m.PropMaterial.SetWeightAndMass(s.MaterialName, 1, s.UnitWeight), "SetWeightAndMass w")
                Chk(m.PropMaterial.SetWeightAndMass(s.MaterialName, 2, s.UnitMass), "SetWeightAndMass m")
            End If
            Chk(m.PropFrame.SetRectangle(s.Name, s.MaterialName, s.Depth, s.Width), "SetRectangle " & s.Name)
        Next
        'patterns
        For Each pt In fm.Patterns
            Dim typ As eLoadPatternType = If(pt.Name = "G", eLoadPatternType.Dead, If(pt.Name = "S", eLoadPatternType.Snow, eLoadPatternType.Live))
            Chk(m.LoadPatterns.Add(pt.Name, typ, pt.SelfWeightMultiplier, True), "LoadPatterns.Add " & pt.Name)
        Next
        'frames
        For ei As Integer = 0 To fm.Elements.Count - 1
            Dim e = fm.Elements(ei)
            Dim a = fm.Nodes(e.NI), b = fm.Nodes(e.NJ)
            Dim nm As String = ""
            Chk(m.FrameObj.AddByCoord(a.X, a.Y, a.Z, b.X, b.Y, b.Z, nm, e.Section.Name, e.Name, "Global"), "AddByCoord " & e.Name)
            frames(ei) = nm
            Dim p1 As String = "", p2 As String = ""
            Chk(m.FrameObj.GetPoints(nm, p1, p2), "GetPoints")
            pts(e.NI) = p1 : pts(e.NJ) = p2
            Chk(m.FrameObj.SetEndLengthOffset(nm, False, e.OffsetI, e.OffsetJ, e.RigidZone), "SetEndLengthOffset " & nm)
        Next
        'restraints and diaphragms
        For d As Integer = 0 To fm.DiaphragmCount - 1
            Chk(m.Diaphragm.SetDiaphragm("D" & (d + 1), False), "SetDiaphragm")
        Next
        For ni As Integer = 0 To fm.Nodes.Count - 1
            Dim n = fm.Nodes(ni)
            If Not pts.ContainsKey(ni) Then Continue For
            If n.Restraint(0) Then
                Dim r() As Boolean = {True, True, True, True, True, True}
                Chk(m.PointObj.SetRestraint(pts(ni), r), "SetRestraint")
            End If
            If n.Diaphragm >= 0 Then Chk(m.PointObj.SetDiaphragm(pts(ni), eDiaphragmOption.DefinedDiaphragm, "D" & (n.Diaphragm + 1)), "PointObj.SetDiaphragm")
        Next
        'loads
        For Each pt In fm.Patterns
            For Each dl In pt.DistLoads
                Chk(m.FrameObj.SetLoadDistributed(frames(dl.Element), pt.Name, 1, dl.Dir, dl.D1, dl.D2, dl.V1, dl.V2, If(dl.IsGlobal, "Global", "Local"), dl.Relative, False), "SetLoadDistributed")
            Next
            If pt.PointLoads.Count > 0 OrElse pt.NodalLoads.Count > 0 Then P("WARNING pattern " & pt.Name & " has point/nodal loads (not exported)")
        Next
    End Sub

    Sub SetCracked(m As cSapModel, cracked As FemModel)
        For Each s In cracked.Sections
            Dim mod8() As Double = CType(s.Modifier.Clone(), Double())
            Chk(m.PropFrame.SetModifiers(s.Name, mod8), "SetModifiers " & s.Name)
        Next
    End Sub

    Sub SetMassSourceAndRs(m As cSapModel, cr As FrameAnalysis, tpName As String)
        'mass source G + 0.3 Q + 0.3 S, lateral only, no self mass
        Dim ver As Integer = 0, keys() As String = {"Name", "IsDefault", "IncLateral", "IncVertical", "LumpMass", "SourceSelf", "SourceAdded", "SourceLoads", "LoadPattern", "Multiplier"}
        Dim data() As String = {
            "MsSrc1", "Yes", "Yes", "No", "Yes", "No", "No", "Yes", "G", "1",
            "MsSrc1", "", "", "", "", "", "", "", "Q", "0.3",
            "MsSrc1", "", "", "", "", "", "", "", "S", "0.3"}
        Dim msVar As String = Environment.GetEnvironmentVariable("MSVAR")
        If msVar = "1" Then data = {"MsSrc1", "Yes", "Yes", "No", "Yes", "Yes", "No", "No", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""}.Take(10).ToArray()
        Chk(m.DatabaseTables.SetTableForEditingArray("Mass Source Definition", ver, keys, If(msVar = "1", 1, 3), data), "Mass Source table")
        'response spectrum function (values in g; SF = g)
        Dim sp = cr.Cracked.Model.Spectra(0)
        Dim fk() As String = {"Name", "Period", "Value", "DampRatio"}
        Dim fd As New List(Of String)
        For i As Integer = 0 To sp.T.Length - 1
            fd.AddRange({"SPD", sp.T(i).ToString("R", ci), sp.Sa(i).ToString("R", ci), "0.05"})
        Next
        Dim ver2 As Integer = 0
        Chk(m.DatabaseTables.SetTableForEditingArray("Functions - Response Spectrum - User Defined", ver2, fk, sp.T.Length, fd.ToArray()), "RS function table")
        Dim nf As Integer, ni As Integer, nw As Integer, ne As Integer, lg As String = ""
        Chk(m.DatabaseTables.ApplyEditedTables(True, nf, ne, nw, ni, lg), "ApplyEditedTables")
        P("tables applied: fatal=" & nf & " errors=" & ne & " warnings=" & nw)
        If nf + ne > 0 Then P(lg)
        DumpTable(m, "Mass Source Definition")
        Chk(m.LoadCases.ModalEigen.SetNumberModes("Modal", 12, 1), "SetNumberModes")
        For Each nm In {"EX", "EY"}
            Chk(m.LoadCases.ResponseSpectrum.SetCase(nm), "RS SetCase " & nm)
            Dim ln() As String = {If(nm = "EX", "U1", "U2")}, fn() As String = {"SPD"}, sf() As Double = {FrameModel.G}, cs() As String = {"Global"}, an() As Double = {0.0}
            Chk(m.LoadCases.ResponseSpectrum.SetLoads(nm, 1, ln, fn, sf, cs, an), "RS SetLoads " & nm)
            Chk(m.LoadCases.ResponseSpectrum.SetModalCase(nm, "Modal"), "RS SetModalCase")
            Chk(m.LoadCases.ResponseSpectrum.SetEccentricity(nm, RcCombos.AccidentalEcc), "RS SetEccentricity")
        Next
    End Sub

    Sub DumpTable(m As cSapModel, key As String)
        Dim v As Integer, fi() As String = Nothing, nrec As Integer, td() As String = Nothing
        Dim r = m.DatabaseTables.GetTableForEditingArray(key, "", v, fi, nrec, td)
        P("table " & key & " ret=" & r & " records=" & nrec & " fields=" & If(fi Is Nothing, "-", String.Join("|", fi)))
        If td IsNot Nothing AndAlso fi IsNot Nothing Then
            For i As Integer = 0 To nrec - 1
                P("   " & String.Join("|", td.Skip(i * fi.Length).Take(fi.Length)))
            Next
        End If
    End Sub

    Class FrameForces
        Public Sta As New List(Of Double), Pv As New List(Of Double), V2 As New List(Of Double), V3 As New List(Of Double), T As New List(Of Double), M2 As New List(Of Double), M3 As New List(Of Double)
    End Class

    Function GetForces(m As cSapModel, frame As String, case_ As String) As FrameForces
        Dim n As Integer, obj() As String = Nothing, os() As Double = Nothing, el() As String = Nothing, es() As Double = Nothing, lc() As String = Nothing, st() As String = Nothing, sn() As Double = Nothing
        Dim pP() As Double = Nothing, v2() As Double = Nothing, v3() As Double = Nothing, t() As Double = Nothing, m2() As Double = Nothing, m3() As Double = Nothing
        Dim r = m.Results.FrameForce(frame, eItemTypeElm.ObjectElm, n, obj, os, el, es, lc, st, sn, pP, v2, v3, t, m2, m3)
        Dim o As New FrameForces
        If dbgCount < 3 Then dbgCount += 1 : P("FrameForce(" & frame & ") ret=" & r & " n=" & n & If(n > 0, " case0=" & lc(0) & " step0='" & st(0) & "'", ""))
        For i As Integer = 0 To n - 1
            If lc(i) <> case_ Then Continue For
            If st(i) <> "" AndAlso st(i) <> "Max" AndAlso st(i) <> "Single Value" Then Continue For
            o.Sta.Add(os(i)) : o.Pv.Add(pP(i)) : o.V2.Add(v2(i)) : o.V3.Add(v3(i)) : o.T.Add(t(i)) : o.M2.Add(m2(i)) : o.M3.Add(m3(i))
        Next
        Return o
    End Function

    'compares ETABS first / last station internal forces with the FemCore face forces f (12). Internal force at the I end = -(force on the element), at the J end = +(force on the element).
    Sub CompareStatic(m As cSapModel, fm As FemModel, frames As Dictionary(Of Integer, String), pattern As String, faces As Double()(), tag As String)
        Dim maxRef As Double() = {0, 0, 0}, maxDiff As Double() = {0, 0, 0}
        Dim worst As String = ""
        Dim idx As Integer = 0
        Dim rows As New List(Of Tuple(Of Double, String))
        For ei As Integer = 0 To fm.Elements.Count - 1
            Dim ff = GetForces(m, frames(ei), pattern)
            If ff.Sta.Count < 2 Then Continue For
            Dim i0 As Integer = ff.Sta.IndexOf(ff.Sta.Min()), i1 As Integer = ff.Sta.IndexOf(ff.Sta.Max())
            Dim f = faces(ei)
            'P, V2, M3 at both ends (tension positive in ETABS P: FemCore f(0) force on element at I along +axis1 -> internal P = -f(0)?)
            Dim ref() As Double = {f(0), f(1), f(5), f(6), f(7), f(11)}
            Dim got() As Double = {ff.Pv(i0), ff.V2(i0), ff.M3(i0), ff.Pv(i1), ff.V2(i1), ff.M3(i1)}
            Dim sgn() As Double = {-1, -1, -1, 1, 1, 1}
            If pattern = "G" Then rows.Add(Tuple.Create(Math.Abs(got(2) + ref(2)) + Math.Abs(got(5) - ref(5)), frames(ei) & " M3 I: ETABS " & got(2).ToString("F2", ci) & " FemCore " & (-ref(2)).ToString("F2", ci) & " | J: ETABS " & got(5).ToString("F2", ci) & " FemCore " & ref(5).ToString("F2", ci) & " | P I: " & got(0).ToString("F1", ci) & " vs " & (-ref(0)).ToString("F1", ci)))
            For q As Integer = 0 To 5
                Dim k As Integer = If(q Mod 3 = 0, 0, If(q Mod 3 = 1, 1, 2))
                Dim rv As Double = sgn(q) * ref(q)
                maxRef(k) = Math.Max(maxRef(k), Math.Abs(rv))
                Dim dv As Double = Math.Abs(got(q) - rv)
                If dv > maxDiff(k) Then maxDiff(k) = dv : If k = 2 Then worst = frames(ei) & " end" & (q \ 3)
            Next
        Next
        If rows.Count > 0 Then
            For Each rw In rows.OrderByDescending(Function(t) t.Item1).Take(6) : P("   worst " & tag & " " & rw.Item2) : Next
        End If
        P(tag & " " & pattern & ": max|P| " & maxRef(0).ToString("F1", ci) & " diff " & maxDiff(0).ToString("F3", ci) & " kN | max|V2| " & maxRef(1).ToString("F1", ci) & " diff " & maxDiff(1).ToString("F3", ci) & " kN | max|M3| " & maxRef(2).ToString("F1", ci) & " diff " & maxDiff(2).ToString("F3", ci) & " kNm (" & (100.0 * maxDiff(2) / Math.Max(maxRef(2), 1.0E-9)).ToString("F3", ci) & " %, worst " & worst & ")")
    End Sub

    Sub Main()
        Dim spec As New FrameSpec
        Dim tp As New TbdyParams
        RcFrameBuilder.NoRigidZones = Environment.GetEnvironmentVariable("NORZ") = "1"
                        Dim ecs As String = Environment.GetEnvironmentVariable("ECC")
        If ecs <> Nothing Then RcCombos.AccidentalEcc = Double.Parse(ecs, ci)
        Dim rzm As String = Environment.GetEnvironmentVariable("RZMODE")
        If rzm <> Nothing Then RcFrameBuilder.RzMode = Integer.Parse(rzm)
        P("FemCore analysis of the trial design ... (no rigid zones = " & RcFrameBuilder.NoRigidZones & ")")
        Dim fa = RcCombos.Analyze(spec, Defs(), tp)
        If Environment.GetEnvironmentVariable("DESIGN") = "1" Then
            DesignProbe.Run(fa, spec, tp, False, AddressOf P, AddressOf Chk)
            DesignProbe.Run(fa, spec, tp, True, AddressOf P, AddressOf Chk)
            File.WriteAllText(Path.Combine(Path.GetTempPath(), "etabs_v2b_log.txt"), log.ToString())
            Return
        End If
        RunSession(fa, spec, False)
        RunSession(fa, spec, True)
        File.WriteAllText(Path.Combine(Path.GetTempPath(), "etabs_v2_log.txt"), log.ToString())
    End Sub

    Sub RunSession(fa As FrameAnalysis, spec As FrameSpec, cracked As Boolean)
        Dim helper As cHelper = New Helper()
        Dim obj As cOAPI = helper.CreateObjectProgID("CSI.ETABS.API.ETABSObject")
        Chk(obj.ApplicationStart(), "ApplicationStart")
        Dim m As cSapModel = obj.SapModel
        Dim ver As String = "", vn As Double = 0
        m.GetVersion(ver, vn)
        P("ETABS version " & ver & ", session " & If(cracked, "CRACKED (modal + RS)", "GROSS (static)"))
        Try
            Dim pts As New Dictionary(Of Integer, String), frames As New Dictionary(Of Integer, String)
            Dim fm = fa.Gross.Model
            BuildEtabs(m, fm, pts, frames)
            Dim dir As String = Path.Combine(Path.GetTempPath(), "etabs_v2")
            Directory.CreateDirectory(dir)
            Dim edb As String = Path.Combine(dir, If(cracked, "ex1_v2_cracked.edb", "ex1_v2_gross.edb"))
            If cracked Then
                SetCracked(m, fa.Cracked.Model)
                SetMassSourceAndRs(m, fa, "SPD")
            End If
            Chk(m.File.Save(edb), "Save")
            Dim gm As Boolean = Environment.GetEnvironmentVariable("GROSSMODAL") = "1"
            If Not cracked AndAlso Not gm Then Chk(m.Analyze.SetRunCaseFlag("Modal", False, False), "run flag Modal off")
            Chk(m.Analyze.RunAnalysis(), "RunAnalysis")
            m.Results.Setup.DeselectAllCasesAndCombosForOutput()
            For Each c In {"G", "Q", "S", "EX", "EY", "Modal"} : m.Results.Setup.SetCaseSelectedForOutput(c, True) : Next
            Dim fmx = If(cracked, fa.Cracked.Model, fm)
            Dim rs = If(cracked, fa.Rc, fa.Rg)
            P("--- " & If(cracked, "cracked", "gross") & " model, static cases (ETABS vs FemCore face forces) ---")
            For Each c In {"G", "Q", "S"}
                CompareStatic(m, fmx, frames, c, rs.FaceForce(fmx.PatternIndex(c)), If(cracked, "cracked", "gross"))
            Next
            If Not cracked Then
                For Each cs In {"G"}
                    Dim pi As Integer = fm.PatternIndex(cs)
                    Dim mxU As Double() = {0, 0, 0}, mxD As Double() = {0, 0, 0}, worstN As String = ""
                    For ni As Integer = 0 To fm.Nodes.Count - 1
                        If Not pts.ContainsKey(ni) Then Continue For
                        Dim nn As Integer, ob() As String = Nothing, el() As String = Nothing, lcx() As String = Nothing, stx() As String = Nothing, snx() As Double = Nothing, u1() As Double = Nothing, u2() As Double = Nothing, u3() As Double = Nothing, r1() As Double = Nothing, r2() As Double = Nothing, r3() As Double = Nothing
                        m.Results.JointDispl(pts(ni), eItemTypeElm.ObjectElm, nn, ob, el, lcx, stx, snx, u1, u2, u3, r1, r2, r3)
                        For i As Integer = 0 To nn - 1
                            If lcx(i) <> cs Then Continue For
                            Dim e3() As Double = {u3(i), r1(i), r2(i)}
                            Dim f3() As Double = {fa.Rg.U(pi)(ni * 6 + 2), fa.Rg.U(pi)(ni * 6 + 3), fa.Rg.U(pi)(ni * 6 + 4)}
                            For q As Integer = 0 To 2
                                mxU(q) = Math.Max(mxU(q), Math.Abs(f3(q)))
                                If Math.Abs(e3(q) - f3(q)) > mxD(q) Then mxD(q) = Math.Abs(e3(q) - f3(q)) : If q = 0 Then worstN = fm.Nodes(ni).Name & " ETABS " & (e3(0) * 1000).ToString("F4", ci) & " FemCore " & (f3(0) * 1000).ToString("F4", ci)
                            Next
                        Next
                    Next
                    P("G joint displacements: max|U3| " & (mxU(0) * 1000).ToString("F4", ci) & " mm diff " & (mxD(0) * 1000).ToString("F4", ci) & " mm (" & (100 * mxD(0) / Math.Max(mxU(0), 1.0E-12)).ToString("F2", ci) & " %, node " & worstN & ") | max|R1| " & mxU(1).ToString("G4", ci) & " diff " & mxD(1).ToString("G3", ci) & " (" & (100 * mxD(1) / Math.Max(mxU(1), 1.0E-12)).ToString("F2", ci) & " %) | max|R2| " & mxU(2).ToString("G4", ci) & " diff " & mxD(2).ToString("G3", ci) & " (" & (100 * mxD(2) / Math.Max(mxU(2), 1.0E-12)).ToString("F2", ci) & " %)")
                Next
            End If
            Dim nr As Integer, lc() As String = Nothing, stp() As String = Nothing, sn() As Double = Nothing, fx() As Double = Nothing, fy() As Double = Nothing, fz() As Double = Nothing, mx() As Double = Nothing, my() As Double = Nothing, mz() As Double = Nothing, gx As Double, gy As Double, gz As Double
            m.Results.BaseReact(nr, lc, stp, sn, fx, fy, fz, mx, my, mz, gx, gy, gz)
            If Not cracked AndAlso gm Then
                Dim np0 As Integer, a1() As String = Nothing, a2() As String = Nothing, a3() As Double = Nothing, per0() As Double = Nothing, a5() As Double = Nothing, a6() As Double = Nothing, a7() As Double = Nothing
                Dim r0 As Integer = m.Results.ModalPeriod(np0, a1, a2, a3, per0, a5, a6, a7)
                Dim nT As Integer, tk() As String = Nothing, tn() As String = Nothing, ti() As Integer = Nothing
                m.DatabaseTables.GetAvailableTables(nT, tk, tn, ti)
                For i As Integer = 0 To nT - 1
                    If tn(i).Contains("Mass") Then P("table: " & tn(i))
                Next
                For Each tkey In {"Assembled Joint Masses", "Mass Summary by Diaphragm"}
                    Dim tv As Integer, fi() As String = Nothing, nrec As Integer, td() As String = Nothing, fl() As String = Nothing
                    Dim rt = m.DatabaseTables.GetTableForDisplayArray(tkey, fl, "", tv, fi, nrec, td)
                    P("display " & tkey & " ret=" & rt & " records=" & nrec & " fields=" & If(fi Is Nothing, "-", String.Join("|", fi)))
                    If td IsNot Nothing AndAlso fi IsNot Nothing Then
                        For i As Integer = 0 To Math.Min(nrec, 8) - 1 : P("   " & String.Join("|", td.Skip(i * fi.Length).Take(fi.Length))) : Next
                    End If
                Next
                P("GROSSMODAL ModalPeriod ret=" & r0 & " n=" & np0 & If(np0 > 0, " T1=" & per0(0).ToString("F4", ci), ""))
            End If
            If Not cracked Then
                For i As Integer = 0 To nr - 1
                    Dim pi As Integer = fm.PatternIndex(lc(i))
                    If pi < 0 Then Continue For
                    Dim sumF As Double = 0.0
                    For ni As Integer = 0 To fm.Nodes.Count - 1 : sumF += fa.Rg.Reaction(pi)(ni * 6 + 2) : Next
                    P("base reaction " & lc(i) & ": ETABS Fz=" & fz(i).ToString("F2", ci) & " kN, FemCore " & sumF.ToString("F2", ci) & " kN")
                Next
            Else
                CompareDynamic(m, fa, spec, pts, frames, lc, fx, fy, nr)
            End If
        Catch ex As Exception
            P("EXCEPTION " & ex.GetType().Name & ": " & ex.Message & vbCrLf & ex.StackTrace)
        Finally
            Try : obj.ApplicationExit(False) : Catch : End Try
        End Try
    End Sub

    Sub CompareDynamic(m As cSapModel, fa As FrameAnalysis, spec As FrameSpec, pts As Dictionary(Of Integer, String), frames As Dictionary(Of Integer, String), lc() As String, fx() As Double, fy() As Double, nb As Integer)
        Dim fmc = fa.Cracked.Model
        Dim np As Integer, lc2() As String = Nothing, st2() As String = Nothing, sn2() As Double = Nothing, per() As Double = Nothing, fq() As Double = Nothing, cf() As Double = Nothing, ev() As Double = Nothing
        Dim rmp As Integer = m.Results.ModalPeriod(np, lc2, st2, sn2, per, fq, cf, ev)
        P("ModalPeriod ret=" & rmp & " n=" & np)
        P("mode  T(ETABS)  T(FemCore)  diff%")
        For i As Integer = 0 To Math.Min(np, fa.Modal.Period.Length) - 1
            P((i + 1).ToString().PadLeft(4) & "  " & per(i).ToString("F5", ci) & "  " & fa.Modal.Period(i).ToString("F5", ci) & "  " & (100.0 * (per(i) - fa.Modal.Period(i)) / fa.Modal.Period(i)).ToString("F3", ci))
        Next
        For i As Integer = 0 To nb - 1
            If lc(i) = "EX" Then P("EX base shear Fx: ETABS " & Math.Abs(fx(i)).ToString("F2", ci) & " kN, FemCore " & fa.BaseShearX.ToString("F2", ci) & " kN, diff " & (100.0 * (Math.Abs(fx(i)) - fa.BaseShearX) / fa.BaseShearX).ToString("F3", ci) & " %")
            If lc(i) = "EY" Then P("EY base shear Fy: ETABS " & Math.Abs(fy(i)).ToString("F2", ci) & " kN, FemCore " & fa.BaseShearY.ToString("F2", ci) & " kN, diff " & (100.0 * (Math.Abs(fy(i)) - fa.BaseShearY) / fa.BaseShearY).ToString("F3", ci) & " %")
        Next
        For Each cs In {"EX", "EY"}
            Dim d As Integer = If(cs = "EX", 0, 1)
            Dim rr = If(cs = "EX", fa.RSx, fa.RSy)
            For k As Integer = 1 To spec.NStory
                Dim node As Integer = fa.Cracked.NodeAt(0, 0, k)
                Dim nn As Integer, ob() As String = Nothing, el() As String = Nothing, lcx() As String = Nothing, stx() As String = Nothing, snx() As Double = Nothing, u1() As Double = Nothing, u2() As Double = Nothing, u3() As Double = Nothing, r1() As Double = Nothing, r2() As Double = Nothing, r3() As Double = Nothing
                m.Results.JointDispl(pts(node), eItemTypeElm.ObjectElm, nn, ob, el, lcx, stx, snx, u1, u2, u3, r1, r2, r3)
                For i As Integer = 0 To nn - 1
                    If lcx(i) <> cs Then Continue For
                    Dim ue As Double = Math.Abs(If(d = 0, u1(i), u2(i)))
                    Dim uf As Double = Math.Abs(rr.U(node * 6 + d))
                    P(cs & " storey " & k & " corner joint U: ETABS " & (ue * 1000.0).ToString("F4", ci) & " mm, FemCore " & (uf * 1000.0).ToString("F4", ci) & " mm, diff " & (100.0 * (ue - uf) / uf).ToString("F3", ci) & " %")
                Next
            Next
        Next
        For Each cs In {"EX", "EY"}
            Dim rr = If(cs = "EX", fa.RSx, fa.RSy)
            For Each ei In {0, fmc.Elements.Count - 1}
                Dim ff = GetForces(m, frames(ei), cs)
                If ff.Sta.Count = 0 Then Continue For
                Dim i0 As Integer = ff.Sta.IndexOf(ff.Sta.Min()), i1 As Integer = ff.Sta.IndexOf(ff.Sta.Max())
                Dim f = rr.FaceForce(ei)
                P(cs & " " & frames(ei) & " M3 (I,J) ETABS " & Math.Abs(ff.M3(i0)).ToString("F2", ci) & " / " & Math.Abs(ff.M3(i1)).ToString("F2", ci) & "  FemCore(RS, no torsion add) " & Math.Abs(f(5)).ToString("F2", ci) & " / " & Math.Abs(f(11)).ToString("F2", ci) & " kNm;  V2 ETABS " & Math.Abs(ff.V2(i0)).ToString("F2", ci) & "  FemCore " & Math.Abs(f(1)).ToString("F2", ci))
            Next
        Next
    End Sub
End Module
