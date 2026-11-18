Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports ETABSv1
Imports FemCore
Imports FemRcModel
Imports RcCore
Imports Ex2Model

'Ex2 V1 / V2-a (EX2_SPEC v2.1 §10): the Ex2 planar frame (Ex2Model.RcFrameBuilder2D) is rebuilt in ETABS 22 through the OAPI with identical geometry,
'sections, restraints (U2, R1, R3 restrained above the fixed base), patterns SW (self weight only), G, Q, combinations U1 = 1.4D and U2 = 1.2D + 1.6L, NO rigid end zones.
'Cases: T1 = source Table 1 sections (5 groups; forces and reactions only), PILOT = Q20 pilot (04_Results/ex2/v2a_pilot), K1 = control design
'(pilot columns, beams 250x500: stirrups governed by strength). Design: ETABS Concrete Frame Design ACI 318-14 against the Ex2 gravity design rules.
'Usage: EtabsEx2 <outDir> <workDir> [thesisEdb]
Module Program
    ReadOnly ci As CultureInfo = CultureInfo.InvariantCulture
    Dim log As New System.Text.StringBuilder
    Dim outDir As String, workDir As String

    Sub P(s As String)
        Console.WriteLine(s) : log.AppendLine(s)
    End Sub

    Sub Chk(ret As Integer, what As String)
        If ret <> 0 Then P("ETABS ERROR ret=" & ret & " in " & what)
    End Sub

    Function F(x As Double, Optional fmt As String = "F3") As String
        Return x.ToString(fmt, ci)
    End Function

    ReadOnly CaseNames As String() = {"SW", "G", "Q", "U1", "U2"}

    'pattern factors (SW, G, Q) of a case / combination
    Function Factors(c As String) As Double()
        Select Case c
            Case "SW" : Return {1.0, 0.0, 0.0}
            Case "G" : Return {0.0, 1.0, 0.0}
            Case "Q" : Return {0.0, 0.0, 1.0}
            Case "U1" : Return {1.4, 1.4, 0.0}
            Case "U2" : Return {1.2, 1.2, 1.6}
        End Select
        Throw New ArgumentException(c)
    End Function

    'FEM station forces (ETABS convention: N tension +, V2, V3, T, M2, M3) of a case at the distances xs (m)
    Function FemStations(a As Analysis2D, ei As Integer, c As String, xs As Double()) As Double()()
        Dim m = a.Frame.Model
        Dim fac = Factors(c)
        Dim pats() As String = {"SW", "G", "Q"}
        Dim res(xs.Length - 1)() As Double
        For j As Integer = 0 To xs.Length - 1 : res(j) = New Double(5) {} : Next
        For q As Integer = 0 To 2
            If fac(q) = 0.0 Then Continue For
            Dim st = a.Fs.StationForces(a.R, m.PatternIndex(pats(q)), ei, xs)
            For j As Integer = 0 To xs.Length - 1
                For k As Integer = 0 To 5 : res(j)(k) += fac(q) * st(j)(k) : Next
            Next
        Next
        Return res
    End Function

    Class EtabsModelMap
        Public Frames As New Dictionary(Of Integer, String)
        Public Points As New Dictionary(Of Integer, String)
    End Class

    Class CaseDef
        Public Name As String
        Public Analysis As Analysis2D
        Public Result As Ex2Result          'Nothing for T1
        Public Defs As Dictionary(Of String, RcSectionDef)
    End Class

    '----------------------------------------------------------------------------------------------------------------------------------
    Sub BuildEtabs(m As cSapModel, cd As CaseDef, map As EtabsModelMap)
        Dim a = cd.Analysis, fm = a.Frame.Model
        Chk(m.InitializeNewModel(eUnits.kN_m_C), "InitializeNewModel")
        Chk(m.File.NewGridOnly(3, 3.3, 3.3, 4, 2, 5.0, 5.0), "NewGridOnly")
        Dim mat As String = "C30"
        Dim s0 = fm.Sections(0)
        Chk(m.PropMaterial.SetMaterial(mat, eMatType.Concrete), "SetMaterial")
        Chk(m.PropMaterial.SetOConcrete_1(mat, 30000.0, False, 1.0, 2, 0, 0.002, 0.003, -0.1), "SetOConcrete_1")
        Chk(m.PropMaterial.SetMPIsotropic(mat, s0.E, 0.2, 0.00001), "SetMPIsotropic")
        Chk(m.PropMaterial.SetWeightAndMass(mat, 1, s0.UnitWeight), "SetWeightAndMass")
        Chk(m.PropMaterial.SetMaterial("R420", eMatType.Rebar), "SetMaterial rebar")
        Chk(m.PropMaterial.SetORebar_1("R420", 420000.0, 500000.0, 462000.0, 550000.0, 1, 0, 0.01, 0.09, -0.1, False), "SetORebar_1")
        For Each s In fm.Sections
            If Math.Abs(s.E - s0.E) > 1.0E-6 OrElse Math.Abs(s.UnitWeight - s0.UnitWeight) > 1.0E-9 Then P("WARNING section " & s.Name & " has another E / unit weight")
            Chk(m.PropFrame.SetRectangle(s.Name, mat, s.Depth, s.Width), "SetRectangle " & s.Name)      'T3 = depth h (along local 2), T2 = width b
        Next
        'design reinforcement data: beams cover to bar centroid 60 mm (d = h - 60); columns: clear cover to ties 40 mm, ties 10 mm, bars 20 mm (centroid 60 mm), count from the program
        Dim sizeOk As String = ""
        For Each s In fm.Sections
            If s.Name.StartsWith("B") Then
                Chk(m.PropFrame.SetRebarBeam(s.Name, "R420", "R420", 0.06, 0.06, 0, 0, 0, 0), "SetRebarBeam " & s.Name)
            Else
                Dim nBars As Integer = 4
                If cd.Result IsNot Nothing Then nBars = cd.Result.ColRes(s.Name).NBars
                Dim nB As Integer, nH As Integer
                ColumnDesign.PerimeterCounts(s.Width * 1000.0, s.Depth * 1000.0, nBars, nB, nH)
                Dim ret As Integer = -1
                For Each rs In {"20", "20d", "#20", "Ø20", "20mm", "M20", "N20"}
                    For Each ts In {"10", "10d", "#10", "Ø10", "10mm", "M10", "N10"}
                        ret = m.PropFrame.SetRebarColumn(s.Name, "R420", "R420", 1, 1, 0.04, 0, nB, nH, rs, ts, 0.15, 2, 2, False)
                        If ret = 0 Then sizeOk = rs & "/" & ts : Exit For
                    Next
                    If ret = 0 Then Exit For
                Next
                Chk(ret, "SetRebarColumn " & s.Name)
                P("column section " & s.Name & ": b x h = " & F(s.Width * 1000, "F0") & " x " & F(s.Depth * 1000, "F0") & " mm, bars " & nBars & " (NR3 = " & nB & " on the faces along b, NR2 = " & nH & " along h), size " & sizeOk)
            End If
        Next
        'patterns: SW = self weight only (multiplier 1), G and Q without self weight; the default pattern Dead gets multiplier 0
        Chk(m.LoadPatterns.Add("SW", eLoadPatternType.Dead, 1.0, True), "LoadPatterns SW")
        Chk(m.LoadPatterns.Add("G", eLoadPatternType.SuperDead, 0.0, True), "LoadPatterns G")
        Chk(m.LoadPatterns.Add("Q", eLoadPatternType.Live, 0.0, True), "LoadPatterns Q")
        m.LoadPatterns.SetSelfWTMultiplier("Dead", 0.0)
        For ei As Integer = 0 To fm.Elements.Count - 1
            Dim e = fm.Elements(ei)
            Dim na = fm.Nodes(e.NI), nb2 = fm.Nodes(e.NJ)
            Dim nm As String = ""
            Chk(m.FrameObj.AddByCoord(na.X, na.Y, na.Z, nb2.X, nb2.Y, nb2.Z, nm, e.Section.Name, e.Name, "Global"), "AddByCoord " & e.Name)
            map.Frames(ei) = nm
            Dim p1 As String = "", p2 As String = ""
            Chk(m.FrameObj.GetPoints(nm, p1, p2), "GetPoints")
            map.Points(e.NI) = p1 : map.Points(e.NJ) = p2
            Chk(m.FrameObj.SetEndLengthOffset(nm, False, 0.0, 0.0, 0.0), "SetEndLengthOffset " & nm)       'no rigid end zones (spec §10 V1, AÇIK-3)
        Next
        For ni As Integer = 0 To fm.Nodes.Count - 1
            Dim n = fm.Nodes(ni)
            Dim r(5) As Boolean
            For k As Integer = 0 To 5 : r(k) = n.Restraint(k) : Next
            Chk(m.PointObj.SetRestraint(map.Points(ni), r), "SetRestraint " & n.Name)
        Next
        For Each pt In fm.Patterns
            For Each dl In pt.DistLoads
                Chk(m.FrameObj.SetLoadDistributed(map.Frames(dl.Element), pt.Name, 1, dl.Dir, dl.D1, dl.D2, dl.V1, dl.V2, If(dl.IsGlobal, "Global", "Local"), dl.Relative, True), "SetLoadDistributed")
            Next
            If pt.PointLoads.Count > 0 OrElse pt.NodalLoads.Count > 0 Then P("WARNING pattern " & pt.Name & " has point/nodal loads (not exported)")
        Next
        For Each c In {"U1", "U2"}
            Chk(m.RespCombo.Add(c, 0), "RespCombo.Add " & c)
            Dim fac = Factors(c)
            Dim pats() As String = {"SW", "G", "Q"}
            For q As Integer = 0 To 2
                If fac(q) = 0.0 Then Continue For
                Dim ty As eCNameType = eCNameType.LoadCase
                Chk(m.RespCombo.SetCaseList(c, ty, pats(q), fac(q)), "SetCaseList " & c)
            Next
        Next
    End Sub

    '----------------------------------------------------------------------------------------------------------------------------------
    'V1 (a): geometry / orientation read back from ETABS
    Sub CheckGeometry(m As cSapModel, cd As CaseDef, map As EtabsModelMap, w As StreamWriter)
        Dim fm = cd.Analysis.Frame.Model
        Dim maxDx As Double = 0
        w.WriteLine("element,etabsName,section,xI,zI,xJ,zJ,L_m,t3_mm,t2_mm,local2_global,femDepth_mm,femWidth_mm")
        For ei As Integer = 0 To fm.Elements.Count - 1
            Dim e = fm.Elements(ei)
            Dim p1 As String = "", p2 As String = ""
            m.FrameObj.GetPoints(map.Frames(ei), p1, p2)
            Dim x1 As Double, y1 As Double, z1 As Double, x2 As Double, y2 As Double, z2 As Double
            m.PointObj.GetCoordCartesian(p1, x1, y1, z1) : m.PointObj.GetCoordCartesian(p2, x2, y2, z2)
            Dim ni = fm.Nodes(e.NI), nj = fm.Nodes(e.NJ)
            maxDx = Math.Max(maxDx, {Math.Abs(x1 - ni.X), Math.Abs(z1 - ni.Z), Math.Abs(x2 - nj.X), Math.Abs(z2 - nj.Z), Math.Abs(y1), Math.Abs(y2)}.Max())
            Dim prop As String = "", sAuto As String = ""
            m.FrameObj.GetSection(map.Frames(ei), prop, sAuto)
            Dim fn As String = "", mp As String = "", t3 As Double, t2 As Double, col As Integer, nt As String = "", gd As String = ""
            m.PropFrame.GetRectangle(prop, fn, mp, t3, t2, col, nt, gd)
            Dim tm(8) As Double
            m.FrameObj.GetTransformationMatrix(map.Frames(ei), tm, True)
            'columns of the 3x3 matrix = local axes 1, 2, 3 in global coordinates (row-major Value(0..8)): local 2 = (tm(1), tm(4), tm(7))
            Dim l2 As String = "(" & F(tm(1), "F2") & ";" & F(tm(4), "F2") & ";" & F(tm(7), "F2") & ")"
            w.WriteLine(String.Join(",", e.Name, map.Frames(ei), prop, F(x1), F(z1), F(x2), F(z2), F(Math.Sqrt((x2 - x1) ^ 2 + (z2 - z1) ^ 2)), F(t3 * 1000, "F0"), F(t2 * 1000, "F0"), l2, F(e.Section.Depth * 1000, "F0"), F(e.Section.Width * 1000, "F0")))
        Next
        Dim np As Integer, pn() As String = Nothing
        m.PointObj.GetNameList(np, pn)
        Dim nfr As Integer, frn() As String = Nothing
        m.FrameObj.GetNameList(nfr, frn)
        P("V1(a) " & cd.Name & ": ETABS points " & np & ", frames " & nfr & " (FEM nodes " & fm.Nodes.Count & ", elements " & fm.Elements.Count & "); max coordinate difference " & F(maxDx, "G3") & " m")
    End Sub

    '----------------------------------------------------------------------------------------------------------------------------------
    Class ForceStats
        Public MaxRef(2) As Double, MaxDiff(2) As Double, Worst(2) As String
        Public MaxRelEnd(2) As Double, WorstRelEnd(2) As String
    End Class

    'V1 (b): reactions; V2-a forces: station forces of all elements, all cases
    Sub CompareForces(m As cSapModel, cd As CaseDef, map As EtabsModelMap, tag As String)
        Dim a = cd.Analysis, fm = a.Frame.Model
        m.Results.Setup.DeselectAllCasesAndCombosForOutput()
        For Each c In {"SW", "G", "Q"} : m.Results.Setup.SetCaseSelectedForOutput(c, True) : Next
        For Each c In {"U1", "U2"} : m.Results.Setup.SetComboSelectedForOutput(c, True) : Next
        'reactions
        Dim nr As Integer, lc() As String = Nothing, stp() As String = Nothing, sn() As Double = Nothing, fx() As Double = Nothing, fy() As Double = Nothing, fz() As Double = Nothing, mx() As Double = Nothing, my() As Double = Nothing, mz() As Double = Nothing, gx As Double, gy As Double, gz As Double
        Chk(m.Results.BaseReact(nr, lc, stp, sn, fx, fy, fz, mx, my, mz, gx, gy, gz), "BaseReact")
        Using w As New StreamWriter(Path.Combine(outDir, tag & "_reactions.csv"))
            w.WriteLine("case,ETABS_Fz_kN,FEM_kN,diff_pct,ETABS_Fx_kN,ETABS_Fy_kN")
            For i As Integer = 0 To nr - 1
                If Not CaseNames.Contains(lc(i)) Then Continue For
                Dim fac = Factors(lc(i))
                Dim fem As Double = fac(0) * a.VerticalReaction("SW") + fac(1) * a.VerticalReaction("G") + fac(2) * a.VerticalReaction("Q")
                Dim dp As Double = 100.0 * (fz(i) - fem) / fem
                w.WriteLine(String.Join(",", lc(i), F(fz(i), "F4"), F(fem, "F4"), F(dp, "F5"), F(fx(i), "F4"), F(fy(i), "F4")))
                P("V1(b) " & tag & " reaction " & lc(i) & ": ETABS Fz " & F(fz(i), "F3") & " kN, FEM " & F(fem, "F3") & " kN, diff " & F(dp, "F4") & " %  (Fx " & F(fx(i), "F3") & ", Fy " & F(fy(i), "F3") & ")")
            Next
        End Using
        'forces
        Dim stats As New Dictionary(Of String, ForceStats)
        For Each c In CaseNames : stats(c) = New ForceStats : Next
        Using w As New StreamWriter(Path.Combine(outDir, tag & "_station_forces.csv"))
            w.WriteLine("element,type,case,x_m,P_ETABS_kN,P_FEM_kN,V2_ETABS_kN,V2_FEM_kN,M3_ETABS_kNm,M3_FEM_kNm")
            Using we As New StreamWriter(Path.Combine(outDir, tag & "_end_forces.csv"))
                we.WriteLine("element,type,case,end,P_ETABS_kN,P_FEM_kN,dP_pct,V2_ETABS_kN,V2_FEM_kN,dV2_pct,M3_ETABS_kNm,M3_FEM_kNm,dM3_pct")
                For ei As Integer = 0 To fm.Elements.Count - 1
                    Dim n As Integer, obj() As String = Nothing, os() As Double = Nothing, el() As String = Nothing, es() As Double = Nothing, lcs() As String = Nothing, st() As String = Nothing, sno() As Double = Nothing
                    Dim pP() As Double = Nothing, v2() As Double = Nothing, v3() As Double = Nothing, t() As Double = Nothing, m2() As Double = Nothing, m3() As Double = Nothing
                    Chk(m.Results.FrameForce(map.Frames(ei), eItemTypeElm.ObjectElm, n, obj, os, el, es, lcs, st, sno, pP, v2, v3, t, m2, m3), "FrameForce " & ei)
                    Dim typ As String = If(a.Frame.IsColumn(ei), "column", "beam")
                    For Each c In CaseNames
                        Dim idx = Enumerable.Range(0, n).Where(Function(i) lcs(i) = c).ToList()
                        If idx.Count = 0 Then P("WARNING no ETABS forces for " & fm.Elements(ei).Name & " " & c) : Continue For
                        Dim xs = idx.Select(Function(i) os(i)).ToArray()
                        Dim fe = FemStations(a, ei, c, xs)
                        Dim s = stats(c)
                        For j As Integer = 0 To idx.Count - 1
                            Dim i = idx(j)
                            Dim got() As Double = {pP(i), v2(i), m3(i)}, ref() As Double = {fe(j)(0), fe(j)(1), fe(j)(5)}
                            w.WriteLine(String.Join(",", fm.Elements(ei).Name, typ, c, F(xs(j), "F4"), F(got(0), "F4"), F(ref(0), "F4"), F(got(1), "F4"), F(ref(1), "F4"), F(got(2), "F4"), F(ref(2), "F4")))
                            For k As Integer = 0 To 2
                                s.MaxRef(k) = Math.Max(s.MaxRef(k), Math.Abs(ref(k)))
                                Dim dv As Double = Math.Abs(got(k) - ref(k))
                                If dv > s.MaxDiff(k) Then s.MaxDiff(k) = dv : s.Worst(k) = fm.Elements(ei).Name & " x=" & F(xs(j), "F2")
                            Next
                        Next
                        'ends
                        For Each jj In {0, idx.Count - 1}
                            Dim i = idx(jj)
                            Dim got() As Double = {pP(i), v2(i), m3(i)}, ref() As Double = {fe(jj)(0), fe(jj)(1), fe(jj)(5)}
                            Dim rel(2) As Double
                            For k As Integer = 0 To 2
                                rel(k) = If(Math.Abs(ref(k)) > 0.001, 100.0 * (got(k) - ref(k)) / Math.Abs(ref(k)), 0.0)
                            Next
                            we.WriteLine(String.Join(",", fm.Elements(ei).Name, typ, c, If(jj = 0, "I", "J"), F(got(0), "F4"), F(ref(0), "F4"), F(rel(0), "F5"), F(got(1), "F4"), F(ref(1), "F4"), F(rel(1), "F5"), F(got(2), "F4"), F(ref(2), "F4"), F(rel(2), "F5")))
                        Next
                    Next
                Next
            End Using
        End Using
        Dim qn() As String = {"P", "V2", "M3"}
        For Each c In CaseNames
            Dim s = stats(c)
            P("V2-a forces " & tag & " " & c & ": " & String.Join(" | ", Enumerable.Range(0, 3).Select(Function(k) qn(k) & " max " & F(s.MaxRef(k), "F2") & " maxdiff " & F(s.MaxDiff(k), "F4") & " (" & F(100.0 * s.MaxDiff(k) / Math.Max(s.MaxRef(k), 1.0E-9), "F4") & " % of max; " & s.Worst(k) & ")")))
        Next
    End Sub

    '----------------------------------------------------------------------------------------------------------------------------------
    'program column D/C of one element (Ex2 ColumnDesignGravity.CheckGroup inner loop, per element). mode: "code" (storey classification of the evaluator),
    '"ns" (all storeys non-sway), "nomag" (delta = 1), "sway" (limit 22 in every storey)
    Function ProgColumnDcr(a As Analysis2D, ei As Integer, cr As ColumnGravityResult, nonSway As Boolean(), mode As String, ByRef gov As String) As Double
        Dim el = a.Frame.Model.Elements(ei)
        Dim b As Double = cr.B, h As Double = cr.H, fc As Double = cr.Fc
        Dim sx = ColumnDesign.MakeSection(b, h, fc, cr.NBars, cr.BarDia)
        Dim sy = ColumnDesign.MakeSection(h, b, fc, cr.NBars, cr.BarDia, False, True)
        Dim cvx As New ColumnDesign.ColCurve(sx), cvy As New ColumnDesign.ColCurve(sy)
        Dim lu As Double = (el.Length - el.OffsetI - el.OffsetJ) * 1000.0
        Dim best As Double = 0.0
        For ci2 As Integer = 0 To a.Combos.Count - 1
            If Not a.Combos(ci2).Main Then Continue For
            Dim fv = a.Face(ci2)(ei)
            Dim sus As Double = a.SustainedAxial(ci2)(ei) * 1000.0
            For endI As Integer = 0 To 1
                Dim pu As Double = If(endI = 0, fv(0), -fv(6)) * 1000.0
                Dim m2 As Double = Math.Abs(If(endI = 0, fv(4), fv(10))) * 1.0E6
                Dim m3 As Double = Math.Abs(If(endI = 0, fv(5), fv(11))) * 1.0E6
                Dim beta As Double = If(pu > 0.0, Math.Abs(sus) / pu, 0.0)
                Dim d3 As Double, d2 As Double
                Select Case mode
                    Case "nomag" : d3 = 1.0 : d2 = 1.0
                    Case "sway"
                        d3 = ColumnDesign.DeltaNs(fc, h, b, lu, pu, beta, -fv(5) * 1.0E6, fv(11) * 1.0E6)
                        d2 = ColumnDesignGravity.DeltaNsNonSway(fc, b, h, lu, pu, beta, -fv(4) * 1.0E6, fv(10) * 1.0E6)
                    Case Else
                        Dim ns As Boolean = mode = "ns" OrElse nonSway Is Nothing OrElse nonSway(el.Story)
                        d3 = If(ns, ColumnDesignGravity.DeltaNsNonSway(fc, h, b, lu, pu, beta, -fv(5) * 1.0E6, fv(11) * 1.0E6), ColumnDesign.DeltaNs(fc, h, b, lu, pu, beta, -fv(5) * 1.0E6, fv(11) * 1.0E6))
                        d2 = ColumnDesignGravity.DeltaNsNonSway(fc, b, h, lu, pu, beta, -fv(4) * 1.0E6, fv(10) * 1.0E6)
                End Select
                Dim dc = ColumnDesign.DcrWithEffects(cvx, cvy, pu, m3, m2, d3, d2)
                If dc > best Then best = dc : gov = a.Combos(ci2).Name.Split(" "c)(0) & If(endI = 0, " bottom", " top") & " Pu=" & F(pu / 1000.0, "F1") & " M3=" & F(m3 / 1.0E6, "F2") & " d3=" & F(d3, "F3") & " beta=" & F(beta, "F3")
            Next
        Next
        Return best
    End Function

    Class ColRow
        Public Name As String, Group As String, Story As Integer
        Public Etabs As Double, EtabsCombo As String, EtabsLoc As Double
        Public EtabsAv As Double
    End Class

    Function ReadColumns(m As cSapModel, map As EtabsModelMap, a As Analysis2D) As Dictionary(Of Integer, ColRow)
        Dim nI As Integer, fn() As String = Nothing, opt() As Integer = Nothing, loc() As Double = Nothing, pc() As String = Nothing, pa() As Double = Nothing, pr() As Double = Nothing, vc() As String = Nothing, av() As Double = Nothing, vc2() As String = Nothing, av2() As Double = Nothing, es() As String = Nothing, ws() As String = Nothing
        Dim rr As Integer = m.DesignConcrete.GetSummaryResultsColumn("All", nI, fn, opt, loc, pc, pa, pr, vc, av, vc2, av2, es, ws, eItemType.Group)
        P("GetSummaryResultsColumn ret=" & rr & " items=" & nI & If(nI > 0, " option(0)=" & opt(0), ""))
        Dim rev As New Dictionary(Of String, Integer)
        For Each kv In map.Frames : rev(kv.Value) = kv.Key : Next
        Dim res As New Dictionary(Of Integer, ColRow)
        For i As Integer = 0 To nI - 1
            If Not rev.ContainsKey(fn(i)) Then Continue For
            Dim ei = rev(fn(i))
            If Not res.ContainsKey(ei) Then res(ei) = New ColRow With {.Name = a.Frame.Model.Elements(ei).Name, .Group = a.Frame.Model.Elements(ei).Group, .Story = a.Frame.Model.Elements(ei).Story, .Etabs = -1}
            Dim r = res(ei)
            If pr(i) > r.Etabs Then r.Etabs = pr(i) : r.EtabsCombo = pc(i) : r.EtabsLoc = loc(i)
            r.EtabsAv = Math.Max(r.EtabsAv, Math.Max(av(i), av2(i)) * 1000.0)
            If es(i) <> "" AndAlso es(i) <> "No Message" Then P("   ETABS column " & fn(i) & " error: " & es(i))
            If ws(i) <> "" AndAlso ws(i) <> "No Message" AndAlso i Mod 3 = 0 Then P("   ETABS column " & fn(i) & " warning: " & ws(i))
        Next
        Return res
    End Function

    'ETABS display table as a list of records (field -> value)
    Function GetTable(m As cSapModel, key As String) As List(Of Dictionary(Of String, String))
        Dim res As New List(Of Dictionary(Of String, String))
        Dim fk() As String = {""}
        Dim tv As Integer = 0, fin() As String = Nothing, nrec As Integer = 0, tdat() As String = Nothing
        If m.DatabaseTables.GetTableForDisplayArray(key, fk, "All", tv, fin, nrec, tdat) <> 0 OrElse fin Is Nothing Then Return res
        For i As Integer = 0 To nrec - 1
            Dim d As New Dictionary(Of String, String)
            For k As Integer = 0 To fin.Length - 1 : d(fin(k)) = tdat(i * fin.Length + k) : Next
            res.Add(d)
        Next
        Return res
    End Function

    'largest delta (in-plane d3, out-of-plane d2) the program applies in an element (all main combinations, both ends) for the given mode
    Function ProgMaxDelta(a As Analysis2D, ei As Integer, cr As ColumnGravityResult, nonSway As Boolean(), mode As String) As Double()
        Dim el = a.Frame.Model.Elements(ei)
        Dim b As Double = cr.B, h As Double = cr.H, fc As Double = cr.Fc
        Dim lu As Double = el.Length * 1000.0
        Dim mx() As Double = {1.0, 1.0}
        For ci2 As Integer = 0 To a.Combos.Count - 1
            If Not a.Combos(ci2).Main Then Continue For
            Dim fv = a.Face(ci2)(ei)
            Dim sus As Double = a.SustainedAxial(ci2)(ei) * 1000.0
            For endI As Integer = 0 To 1
                Dim pu As Double = If(endI = 0, fv(0), -fv(6)) * 1000.0
                Dim beta As Double = If(pu > 0.0, Math.Abs(sus) / pu, 0.0)
                Dim ns As Boolean = mode = "ns" OrElse nonSway Is Nothing OrElse nonSway(el.Story)
                Dim d3 As Double = If(mode = "nomag", 1.0, If(ns, ColumnDesignGravity.DeltaNsNonSway(fc, h, b, lu, pu, beta, -fv(5) * 1.0E6, fv(11) * 1.0E6), ColumnDesign.DeltaNs(fc, h, b, lu, pu, beta, -fv(5) * 1.0E6, fv(11) * 1.0E6)))
                Dim d2 As Double = If(mode = "nomag", 1.0, ColumnDesignGravity.DeltaNsNonSway(fc, b, h, lu, pu, beta, -fv(4) * 1.0E6, fv(10) * 1.0E6))
                mx(0) = Math.Max(mx(0), d3) : mx(1) = Math.Max(mx(1), d2)
            Next
        Next
        Return mx
    End Function

    'column comparison of one ETABS design run: CD1 (ETABS D/C > 1 OR ETABS status O/S, program <= 1), CD2, CD3
    Sub CompareColumns(m As cSapModel, cd As CaseDef, map As EtabsModelMap, run As String, progMode As String)
        Dim a = cd.Analysis
        Dim rows = ReadColumns(m, map, a)
        'ETABS status / messages per column (O/S flags, e.g. ACI 6.2.6 delta > 1.4)
        Dim stat As New Dictionary(Of String, String), warn As New Dictionary(Of String, String)
        For Each r In GetTable(m, "Concrete Column PMM Envelope - ACI 318-14")
            If r.ContainsKey("Status") AndAlso r("Status") <> "" Then stat(r("UniqueName")) = r("Status") & " [" & r("Location") & " " & r("PMMCombo") & "]"
        Next
        For Each r In GetTable(m, "Concrete Column Design Summary - ACI 318-14")
            If r.ContainsKey("WarnMsg") AndAlso r("WarnMsg") <> "No Message" Then warn(r("UniqueName")) = r("WarnMsg")
            If r.ContainsKey("ErrMsg") AndAlso r("ErrMsg") <> "No Message" Then warn(r("UniqueName")) = r("ErrMsg")
        Next
        Dim cd1 As Integer = 0, nOS As Integer = 0, minR As Double = 1.0E9, maxR As Double = 0, maxE As Double = 0, maxPg As Double = 0, n As Integer = 0
        Using w As New StreamWriter(Path.Combine(outDir, cd.Name & "_columns_" & run & ".csv"))
            w.WriteLine("element,group,storey,storeyQ,b_mm,h_mm,nBars,ETABS_PMM,ETABS_combo,ETABS_loc_m,ETABS_status,ETABS_message,program_DC_" & progMode & ",ratio_prog_ETABS,program_DC_code,program_DC_ns,program_DC_nomag,program_DC_sway22,program_maxDelta_inplane_code,program_maxDelta_outofplane,program_governing,ETABS_Av_mm2_per_mm")
            For Each kv In rows.OrderBy(Function(k) k.Key)
                Dim ei = kv.Key, r = kv.Value
                Dim cr = cd.Result.ColRes(r.Group)
                Dim g1 As String = "", g0 As String = ""
                Dim pv As Double = ProgColumnDcr(a, ei, cr, cd.Result.NonSway, progMode, g1)
                Dim pCode = ProgColumnDcr(a, ei, cr, cd.Result.NonSway, "code", g0), pNs = ProgColumnDcr(a, ei, cr, cd.Result.NonSway, "ns", g0), pNo = ProgColumnDcr(a, ei, cr, cd.Result.NonSway, "nomag", g0), pSw = ProgColumnDcr(a, ei, cr, cd.Result.NonSway, "sway", g0)
                Dim dl = ProgMaxDelta(a, ei, cr, cd.Result.NonSway, "code")
                Dim os As Boolean = stat.ContainsKey(r.Name) AndAlso stat(r.Name).StartsWith("O/S")
                Dim ratio As Double = If(r.Etabs > 0.02 AndAlso Not os, pv / r.Etabs, Double.NaN)
                If (r.Etabs > 1.0 OrElse os) AndAlso pv <= 1.0 Then cd1 += 1
                If os Then nOS += 1
                If r.Etabs > 0.02 AndAlso Not os Then minR = Math.Min(minR, ratio) : maxR = Math.Max(maxR, ratio) : n += 1
                If Not os Then maxE = Math.Max(maxE, r.Etabs)
                maxPg = Math.Max(maxPg, pv)
                Dim q As Double = If(cd.Result.StoreyQ Is Nothing, Double.NaN, cd.Result.StoreyQ(r.Story))
                w.WriteLine(String.Join(",", r.Name, r.Group, r.Story, F(q, "F4"), F(cr.B, "F0"), F(cr.H, "F0"), cr.NBars, F(r.Etabs, "F4"), r.EtabsCombo, F(r.EtabsLoc, "F3"), If(stat.ContainsKey(r.Name), stat(r.Name), "ok"), If(warn.ContainsKey(r.Name), warn(r.Name).Replace(",", ";"), ""), F(pv, "F4"), F(ratio, "F4"), F(pCode, "F4"), F(pNs, "F4"), F(pNo, "F4"), F(pSw, "F4"), F(dl(0), "F3"), F(dl(1), "F3"), g1.Replace(",", ";"), F(r.EtabsAv, "F5")))
            Next
        End Using
        Dim cd3 As Double = 100.0 * (maxE - maxPg) / maxE
        P("COLUMNS " & cd.Name & " run " & run & " (program mode " & progMode & "): compared " & n & ", ETABS O/S " & nOS & "; CD1 (ETABS>1 or O/S, program<=1) = " & cd1 & "; CD2 min ratio program/ETABS = " & F(minR, "F4") & " (max " & F(maxR, "F4") & "); CD3 max D/C (non-O/S) ETABS " & F(maxE, "F4") & " program " & F(maxPg, "F4") & " -> unsafe-side difference " & F(Math.Max(cd3, 0.0), "F2") & " %")
    End Sub

    '----------------------------------------------------------------------------------------------------------------------------------
    Class BeamRow
        Public Loc As Double, TopReq, BotReq, TopArea, BotArea, TopMin, BotMin, VArea, VReq, VMin As Double
        Public TopCombo, BotCombo, VCombo As String
    End Class

    Sub CompareBeams(m As cSapModel, cd As CaseDef, map As EtabsModelMap, Optional suffix As String = "")
        Dim a = cd.Analysis, fm = a.Frame.Model
        Dim nI As Integer, fn() As String = Nothing, loc() As Double = Nothing
        Dim tc() As String = Nothing, ta() As Double = Nothing, tar() As Double = Nothing, tam() As Double = Nothing, tap() As Double = Nothing
        Dim bc() As String = Nothing, ba() As Double = Nothing, bar() As Double = Nothing, bam() As Double = Nothing, bap() As Double = Nothing
        Dim vc() As String = Nothing, va() As Double = Nothing, var() As Double = Nothing, vam() As Double = Nothing, vap() As Double = Nothing
        Dim tlc() As String = Nothing, tla() As Double = Nothing, ttc() As String = Nothing, tta() As Double = Nothing, es() As String = Nothing, ws() As String = Nothing
        Dim rr As Integer = m.DesignConcrete.GetSummaryResultsBeam_2("All", nI, fn, loc, tc, ta, tar, tam, tap, bc, ba, bar, bam, bap, vc, va, var, vam, vap, tlc, tla, ttc, tta, es, ws, eItemType.Group)
        P("GetSummaryResultsBeam_2 ret=" & rr & " rows=" & nI)
        Dim rev As New Dictionary(Of String, Integer)
        For Each kv In map.Frames : rev(kv.Value) = kv.Key : Next
        Dim byEl As New Dictionary(Of Integer, List(Of BeamRow))
        For i As Integer = 0 To nI - 1
            If Not rev.ContainsKey(fn(i)) Then Continue For
            Dim ei = rev(fn(i))
            If Not byEl.ContainsKey(ei) Then byEl(ei) = New List(Of BeamRow)
            byEl(ei).Add(New BeamRow With {.Loc = loc(i), .TopReq = tar(i) * 1.0E6, .BotReq = bar(i) * 1.0E6, .TopArea = ta(i) * 1.0E6, .BotArea = ba(i) * 1.0E6, .TopMin = tam(i) * 1.0E6, .BotMin = bam(i) * 1.0E6,
                                             .VArea = va(i) * 1000.0, .VReq = var(i) * 1000.0, .VMin = vam(i) * 1000.0, .TopCombo = tc(i), .BotCombo = bc(i), .VCombo = vc(i)})
            If es(i) <> "" AndAlso es(i) <> "No Message" Then P("   ETABS beam " & fn(i) & " @" & F(loc(i), "F2") & " error: " & es(i))
        Next
        Const fy As Double = 420.0, phi As Double = 0.75
        'station table (formula-level test: same station forces, program formulas, ETABS results)
        Using w As New StreamWriter(Path.Combine(outDir, cd.Name & "_beam_stations" & suffix & ".csv"))
            w.WriteLine("element,group,x_m,ETABS_TopReq_mm2,prog_TopReq_mm2,ETABS_BotReq_mm2,prog_BotReq_mm2,ETABS_TopArea,ETABS_BotArea,ETABS_TopMin,ETABS_BotMin,ETABS_Av_s_mm2pmm,ETABS_VReq,ETABS_VMin,prog_Av_s_station_mm2pmm,Vu_FEM_kN,ETABS_Vc_implied_kN,prog_Vc_kN,ETABS_Vcombo")
            For Each kv In byEl.OrderBy(Function(k) k.Key)
                Dim ei = kv.Key, e = fm.Elements(ei)
                Dim br = cd.Result.BeamRes(e.Group)
                Dim b As Double = br.B, d As Double = br.D, fc As Double = br.Fc
                Dim xs = kv.Value.Select(Function(r) r.Loc).ToArray()
                Dim f1 = FemStations(a, ei, "U1", xs), f2 = FemStations(a, ei, "U2", xs)
                Dim vcP As Double = AciBeam.Vc(b, d, fc)
                Dim avMin As Double = AciBeam.AvMinPerS(b, fc, fy)
                For j As Integer = 0 To xs.Length - 1
                    Dim r = kv.Value(j)
                    Dim mTop As Double = Math.Max(Math.Max(-f1(j)(5), -f2(j)(5)), 0.0) * 1.0E6, mBot As Double = Math.Max(Math.Max(f1(j)(5), f2(j)(5)), 0.0) * 1.0E6
                    Dim vu As Double = Math.Max(Math.Abs(f1(j)(1)), Math.Abs(f2(j)(1))) * 1000.0
                    Dim pTop As Double = If(mTop > 0, AciBeam.RequiredAs(mTop, b, d, fc, fy), 0.0), pBot As Double = If(mBot > 0, AciBeam.RequiredAs(mBot, b, d, fc, fy), 0.0)
                    Dim vsReq As Double = Math.Max(vu / phi - vcP, 0.0)
                    Dim pAv As Double = If(vu > 0.5 * phi * vcP, Math.Max(vsReq / (fy * d), avMin), vsReq / (fy * d))
                    Dim vcImpl As Double = If(r.VArea > avMin * 1.001, (vu / phi - r.VArea * fy * d) / 1000.0, Double.NaN)
                    w.WriteLine(String.Join(",", e.Name, e.Group, F(r.Loc, "F4"), F(r.TopReq, "F1"), F(pTop, "F1"), F(r.BotReq, "F1"), F(pBot, "F1"), F(r.TopArea, "F1"), F(r.BotArea, "F1"), F(r.TopMin, "F1"), F(r.BotMin, "F1"), F(r.VArea, "F5"), F(r.VReq, "F5"), F(r.VMin, "F5"), F(pAv, "F5"), F(vu / 1000.0, "F3"), F(vcImpl, "F3"), F(vcP / 1000.0, "F3"), r.VCombo))
                Next
            Next
        End Using
        'group-level gates B1 (required As) and B2 (Av/s), and D/C proxies
        Using w As New StreamWriter(Path.Combine(outDir, cd.Name & "_beam_groups" & suffix & ".csv"))
            w.WriteLine("group,b_mm,h_mm,d_mm,prog_MuTop_kNm,prog_MuBot_kNm,prog_AsTopReq,ETABS_TopReq_max,B1_top,prog_AsBotReq,ETABS_BotReq_max,B1_bot,prog_AsTop_prov,ETABS_TopArea_max,prog_AsBot_prov,ETABS_BotArea_max,prog_DCflex,ETABSreq_over_progProv_top,ETABSreq_over_progProv_bot,prog_Vu_d_kN,prog_Av_s,ETABS_Av_s_max_node,B2_node,ETABS_Av_s_at_d,B2_at_d,prog_s_mm,prog_Av_s_provided,prog_DCshear,ETABSnode_over_progProv,ETABSd_over_progProv,stirrupGov")
            For Each g In a.Frame.BeamGroups
                Dim br = cd.Result.BeamRes(g), bs = cd.Result.BeamShr(g)
                Dim els = byEl.Keys.Where(Function(k) fm.Elements(k).Group = g).ToList()
                Dim eTop = els.Max(Function(k) byEl(k).Max(Function(r) r.TopReq)), eBot = els.Max(Function(k) byEl(k).Max(Function(r) r.BotReq))
                Dim eTopA = els.Max(Function(k) byEl(k).Max(Function(r) r.TopArea)), eBotA = els.Max(Function(k) byEl(k).Max(Function(r) r.BotArea))
                Dim eAvNode = els.Max(Function(k) byEl(k).Max(Function(r) r.VArea))
                'ETABS Av/s at x = d and L - d (linear interpolation between the ETABS stations)
                Dim eAvD As Double = 0
                For Each k In els
                    Dim lst = byEl(k).OrderBy(Function(r) r.Loc).ToList()
                    Dim L As Double = lst.Last().Loc, dm As Double = br.D / 1000.0
                    For Each x In {dm, L - dm}
                        eAvD = Math.Max(eAvD, Interp(lst.Select(Function(r) r.Loc).ToArray(), lst.Select(Function(r) r.VArea).ToArray(), x))
                    Next
                Next
                Dim pTopReq As Double = AciBeam.RequiredAs(br.MuTop, br.B, br.D, br.Fc, 420.0), pBotReq As Double = AciBeam.RequiredAs(br.MuBot, br.B, br.D, br.Fc, 420.0)
                Dim avProv As Double = 2.0 * Math.PI * 100.0 / 4.0 / bs.S
                w.WriteLine(String.Join(",", g, F(br.B, "F0"), F(br.H, "F0"), F(br.D, "F0"), F(br.MuTop / 1.0E6, "F3"), F(br.MuBot / 1.0E6, "F3"), F(pTopReq, "F1"), F(eTop, "F1"), F(pTopReq / eTop, "F4"), F(pBotReq, "F1"), F(eBot, "F1"), F(pBotReq / eBot, "F4"), F(br.AsTop, "F1"), F(eTopA, "F1"), F(br.AsBot, "F1"), F(eBotA, "F1"), F(br.DcFlex, "F4"), F(eTop / br.AsTop, "F4"), F(eBot / br.AsBot, "F4"),
                                             F(bs.Vu / 1000.0, "F3"), F(bs.AvPerS, "F5"), F(eAvNode, "F5"), F(bs.AvPerS / eAvNode, "F4"), F(eAvD, "F5"), F(bs.AvPerS / eAvD, "F4"), F(bs.S, "F1"), F(avProv, "F5"), F(bs.DcShear, "F4"), F(eAvNode / avProv, "F4"), F(eAvD / avProv, "F4"), bs.StirrupGov))
                P("BEAMS" & suffix & " " & cd.Name & " " & g & ": B1 top " & F(pTopReq / eTop, "F4") & " bot " & F(pBotReq / eBot, "F4") & " | B2 Av/s program " & F(bs.AvPerS, "F4") & " / ETABS node " & F(eAvNode, "F4") & " = " & F(bs.AvPerS / eAvNode, "F4") & ", / ETABS at d " & F(eAvD, "F4") & " = " & F(bs.AvPerS / eAvD, "F4") & " | stirrups " & bs.StirrupGov & " s=" & F(bs.S, "F1"))
            Next
        End Using
    End Sub

    Function Interp(xs As Double(), ys As Double(), x As Double) As Double
        If x <= xs(0) Then Return ys(0)
        For k As Integer = 0 To xs.Length - 2
            If x <= xs(k + 1) Then
                Dim t As Double = (x - xs(k)) / Math.Max(xs(k + 1) - xs(k), 1.0E-12)
                Return ys(k) + t * (ys(k + 1) - ys(k))
            End If
        Next
        Return ys(ys.Length - 1)
    End Function

    '----------------------------------------------------------------------------------------------------------------------------------
    Sub DumpConcreteTables(m As cSapModel, tag As String)
        Dim ntb As Integer, tkey() As String = Nothing, tnm() As String = Nothing, timp() As Integer = Nothing
        m.DatabaseTables.GetAvailableTables(ntb, tkey, tnm, timp)
        Dim dir As String = Path.Combine(outDir, "tables_" & tag)
        Directory.CreateDirectory(dir)
        For ti As Integer = 0 To ntb - 1
            Dim k As String = tkey(ti)
            If Not (k.StartsWith("Concrete") OrElse k.Contains("Overwrites") OrElse k.Contains("Preferences")) Then Continue For
            Dim fk() As String = {""}
            Dim tv As Integer = 0, fin() As String = Nothing, nrec As Integer = 0, tdat() As String = Nothing
            Dim rt As Integer = m.DatabaseTables.GetTableForDisplayArray(k, fk, "All", tv, fin, nrec, tdat)
            If rt <> 0 OrElse fin Is Nothing Then Continue For
            Dim fname As String = String.Join("_", k.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_").Replace("/", "_") & ".csv"
            Using w As New StreamWriter(Path.Combine(dir, fname))
                w.WriteLine(String.Join(",", fin))
                For i As Integer = 0 To nrec - 1
                    w.WriteLine(String.Join(",", tdat.Skip(i * fin.Length).Take(fin.Length).Select(Function(s) If(s Is Nothing, "", s.Replace(",", ";")))))
                Next
            End Using
            P("   table dumped: " & k & " (" & nrec & " records)")
        Next
    End Sub

    Sub DumpDesignSettings(m As cSapModel, map As EtabsModelMap, a As Analysis2D, tag As String)
        Dim pn() As String = {"", "Number of interaction curves", "Number of interaction points", "Consider minimum eccentricity", "Design for B/C capacity ratio", "Seismic design category", "Design system Omega0", "Design system Rho", "Design system Sds", "Consider ICC_ESR 2017", "Phi tension controlled", "Phi compression controlled tied", "Phi compression controlled spiral", "Phi shear and/or torsion", "Phi shear seismic", "Phi joint shear", "Pattern live load factor", "Utilization factor limit", "Multi-response case design"}
        Dim pv As Double
        For it As Integer = 1 To 18
            If m.DesignConcrete.ACI318_14.GetPreference(it, pv) = 0 Then P("PREF " & tag & " " & it & " " & pn(it) & " = " & pv.ToString(ci))
        Next
        Dim ovn() As String = {"", "Framing type", "Live load reduction factor", "Unbraced length ratio major", "Unbraced length ratio minor", "K major", "K minor", "Cm major", "Cm minor", "Db major", "Db minor", "Ds major", "Ds minor", "Consider minimum eccentricity"}
        For Each nm In {"E1", "E9", "E15", "E5", "E6"}
            Dim ei As Integer = a.Frame.Model.Elements.FindIndex(Function(x) x.Name = nm)
            If ei < 0 Then Continue For
            Dim s As New List(Of String)
            For it As Integer = 1 To 13
                Dim pd As Boolean
                If m.DesignConcrete.ACI318_14.GetOverwrite(map.Frames(ei), it, pv, pd) = 0 Then s.Add(ovn(it) & "=" & pv.ToString(ci) & If(pd, "(prog)", ""))
            Next
            P("OVERWRITES " & tag & " " & nm & ": " & String.Join("; ", s))
        Next
    End Sub

    'design run: framing types per column storey (4 = NonSway, 3 = Sway Ordinary), unbraced length ratio 1 (lu = axis length = program), K = 1, Cm = 1 (program PCR-2); nomag: Dns = 1
    'NOTE (2026-11-16, PCR-21/22; NOT yet applied, this tool was neither rebuilt nor run in that package): Ex2Model now uses lu = clear height
    '(ColumnDesignGravity.ClearHeight: L - h_b(below)/2 - h_b(above)/2, Ö1 rule). In the next ETABS V2-a re-validation set the unbraced length ratio
    '(items 3 and 4, major AND minor) explicitly to ClearHeight(a.Frame, ei) / e.Length (ETABS program-determined value is 1.0 = axis length without
    'end offsets, run D) and read it back (WriteOverwrites). Also: ProgColumnDcr / ProgMaxDelta below still use the axis length and the pre-PCR-22 sway
    'branch (frozen DeltaNs with limit 22); replace by ColumnDesignGravity.CheckGroup (stab = Result.Stability) or mirror PCR-22 (no exemption in
    'Q > 0.05 storeys, delta_s on M2s) and report the ACI 6.2.6 ratio (PCR-20) next to the ETABS "Delta_ns exceeds 1.4" message.
    Sub SetColumnOverwrites(m As cSapModel, map As EtabsModelMap, a As Analysis2D, swayStorey As Boolean(), nomag As Boolean)
        Dim fm = a.Frame.Model
        For ei As Integer = 0 To fm.Elements.Count - 1
            Dim e = fm.Elements(ei)
            Dim isSway As Boolean = swayStorey IsNot Nothing AndAlso swayStorey(e.Story)
            Chk(m.DesignConcrete.ACI318_14.SetOverwrite(map.Frames(ei), 1, If(isSway, 3.0, 4.0), eItemType.Objects), "SetOverwrite framing " & e.Name)
            If Not a.Frame.IsColumn(ei) Then Continue For
            For item As Integer = 3 To 4 : Chk(m.DesignConcrete.ACI318_14.SetOverwrite(map.Frames(ei), item, 1.0, eItemType.Objects), "SetOverwrite lu " & item) : Next
            For item As Integer = 5 To 8 : Chk(m.DesignConcrete.ACI318_14.SetOverwrite(map.Frames(ei), item, 1.0, eItemType.Objects), "SetOverwrite K/Cm " & item) : Next
            For item As Integer = 9 To 12 : Chk(m.DesignConcrete.ACI318_14.SetOverwrite(map.Frames(ei), item, If(nomag, 1.0, 0.0), eItemType.Objects), "SetOverwrite D " & item) : Next
        Next
    End Sub

    '----------------------------------------------------------------------------------------------------------------------------------
    Function MakeCase(name As String, x As Integer()) As CaseDef
        Dim spec As New FrameSpec2D With {.Groups = 9}
        Dim prob As New Ex2Problem(spec, New PriceSet With {.Fibre = 90.0}, False, New AciPlainShear(), 1.0)
        Dim r = prob.Evaluate(x)
        Dim cd As New CaseDef With {.Name = name, .Analysis = r.Analysis, .Result = r}
        P(name & ": x = " & String.Join(",", x) & "; program cost " & F(r.Cost.Total, "F2") & " TL, T = " & F(r.T, "G4") & "; storey Q = " & String.Join(" / ", r.StoreyQ.Skip(1).Select(Function(q) F(q, "F4"))))
        For Each kv In r.ColRes : P("   col " & kv.Key & " " & F(kv.Value.B, "F0") & "x" & F(kv.Value.H, "F0") & " bars " & kv.Value.NBars & " D/C " & F(kv.Value.DcrMax, "F4")) : Next
        For Each kv In r.BeamRes : P("   beam " & kv.Key & " " & F(kv.Value.B, "F0") & "x" & F(kv.Value.H, "F0") & " AsTop " & F(kv.Value.AsTop, "F1") & " AsBot " & F(kv.Value.AsBot, "F1") & " | shear Vu(d) " & F(r.BeamShr(kv.Key).Vu / 1000, "F2") & " Av/s " & F(r.BeamShr(kv.Key).AvPerS, "F4") & " s " & F(r.BeamShr(kv.Key).S, "F1") & " " & r.BeamShr(kv.Key).StirrupGov) : Next
        Return cd
    End Function

    Function ReadX(path As String) As Integer()
        Dim t = File.ReadAllText(path).Trim().Split(","c).Where(Function(s) s.Trim() <> "x").Select(Function(s) Integer.Parse(s.Trim())).ToArray()
        Return t
    End Function

    Sub RunCase(m As cSapModel, cd As CaseDef, design As Boolean)
        P("================ case " & cd.Name & " ================")
        Dim map As New EtabsModelMap
        BuildEtabs(m, cd, map)
        Using w As New StreamWriter(Path.Combine(outDir, cd.Name & "_geometry.csv"))
            CheckGeometry(m, cd, map, w)
        End Using
        Chk(m.File.Save(Path.Combine(workDir, "ex2_" & cd.Name & ".edb")), "Save")
        Chk(m.Analyze.SetRunCaseFlag("Modal", False, False), "run flag Modal off")
        Chk(m.Analyze.RunAnalysis(), "RunAnalysis")
        CompareForces(m, cd, map, cd.Name)
        If Not design Then Return
        Chk(m.DesignConcrete.SetCode("ACI 318-14"), "DesignConcrete.SetCode")
        Dim nc As Integer, names() As String = Nothing
        m.RespCombo.GetNameList(nc, names)
        For Each nm In names : m.DesignConcrete.SetComboStrength(nm, nm = "U1" OrElse nm = "U2") : Next
        P("design combinations in model: " & String.Join(", ", names))
        DumpDesignSettings(m, map, cd.Analysis, cd.Name & "_default")
        'no live-load patterning in ETABS either (spec §3 [K b], E3): Pattern live load factor (preference 16) = 0
        Chk(m.DesignConcrete.ACI318_14.SetPreference(16, 0.0), "SetPreference PatLLF 0")
        'run A: all members NonSway (spec §10 V2-a), lu ratio 1, K 1, Cm 1, delta program-determined
        SetColumnOverwrites(m, map, cd.Analysis, Nothing, False)
        DumpDesignSettings(m, map, cd.Analysis, cd.Name & "_runA")
        P("StartDesign A ret=" & m.DesignConcrete.StartDesign())
        CompareColumns(m, cd, map, "A_nonsway", "ns")
        CompareColumns(m, cd, map, "A_nonsway_vs_code", "code")
        CompareBeams(m, cd, map)
        DumpConcreteTables(m, cd.Name & "_A")
        'diagnostic A75: ETABS default pattern live load factor 0.75 (beams only)
        Chk(m.DesignConcrete.ACI318_14.SetPreference(16, 0.75), "SetPreference PatLLF 0.75")
        P("StartDesign A75 ret=" & m.DesignConcrete.StartDesign())
        CompareBeams(m, cd, map, "_patLLF075")
        Chk(m.DesignConcrete.ACI318_14.SetPreference(16, 0.0), "SetPreference PatLLF 0")
        'diagnostic S: seismic design category B instead of the default D (no effect expected in the gravity-only NonSway design)
        Dim sdc0 As Double
        m.DesignConcrete.ACI318_14.GetPreference(5, sdc0)
        Chk(m.DesignConcrete.ACI318_14.SetPreference(5, 2.0), "SetPreference SDC B")
        P("StartDesign S ret=" & m.DesignConcrete.StartDesign())
        CompareColumns(m, cd, map, "S_sdcB", "ns")
        CompareBeams(m, cd, map, "_sdcB")
        Chk(m.DesignConcrete.ACI318_14.SetPreference(5, sdc0), "SetPreference SDC back")
        'run B: Sway Ordinary in the storeys the program classifies as sway (Q > 0.05)
        Dim sw As Boolean() = cd.Result.NonSway.Select(Function(b) Not b).ToArray()
        SetColumnOverwrites(m, map, cd.Analysis, sw, False)
        P("StartDesign B ret=" & m.DesignConcrete.StartDesign())
        CompareColumns(m, cd, map, "B_swayQ", "code")
        DumpConcreteTables(m, cd.Name & "_B")
        'run C: NonSway with Dns = Ds = 1 (no magnification) against the program without magnification
        SetColumnOverwrites(m, map, cd.Analysis, Nothing, True)
        P("StartDesign C ret=" & m.DesignConcrete.StartDesign())
        CompareColumns(m, cd, map, "C_nomag", "nomag")
        DumpConcreteTables(m, cd.Name & "_C")
        'run D: ETABS defaults for the column slenderness items (program-determined lu, K, Cm, delta), NonSway; reads the program-determined lu ratio
        For ei As Integer = 0 To cd.Analysis.Frame.Model.Elements.Count - 1
            If Not cd.Analysis.Frame.IsColumn(ei) Then Continue For
            For item As Integer = 3 To 12 : m.DesignConcrete.ACI318_14.SetOverwrite(map.Frames(ei), item, 0.0, eItemType.Objects) : Next
        Next
        P("StartDesign D ret=" & m.DesignConcrete.StartDesign())
        DumpDesignSettings(m, map, cd.Analysis, cd.Name & "_runD_defaults")
        CompareColumns(m, cd, map, "D_etabsdefaults", "ns")
        DumpConcreteTables(m, cd.Name & "_D")
        Chk(m.File.Save(Path.Combine(workDir, "ex2_" & cd.Name & ".edb")), "Save after design")
    End Sub

    'V1 (d): thesis EDB (a COPY in the work folder; Sources is read-only)
    Sub ReadThesisEdb(m As cSapModel, src As String)
        P("================ thesis EDB (copy) ================")
        Dim dst As String = Path.Combine(workDir, "thesis_copy", "ornek2_thesis.EDB")
        Directory.CreateDirectory(Path.GetDirectoryName(dst))
        File.Copy(src, dst, True)
        P("source " & src & " (" & New FileInfo(src).Length & " bytes) copied to " & dst)
        Dim r As Integer = m.File.OpenFile(dst)
        P("OpenFile ret=" & r)
        Chk(m.SetPresentUnits(eUnits.kN_m_C), "SetPresentUnits")
        Dim np As Integer, pn() As String = Nothing
        m.PointObj.GetNameList(np, pn)
        Dim xs As New SortedSet(Of Double), ys As New SortedSet(Of Double), zs As New SortedSet(Of Double)
        For Each nm In pn
            Dim x As Double, y As Double, z As Double
            m.PointObj.GetCoordCartesian(nm, x, y, z)
            xs.Add(Math.Round(x, 3)) : ys.Add(Math.Round(y, 3)) : zs.Add(Math.Round(z, 3))
        Next
        P("thesis points " & np & "; X = " & String.Join(" ", xs.Select(Function(v) F(v, "F3"))) & "; Y = " & String.Join(" ", ys.Select(Function(v) F(v, "F3"))) & "; Z = " & String.Join(" ", zs.Select(Function(v) F(v, "F3"))))
        Dim nf As Integer, fnm() As String = Nothing
        m.FrameObj.GetNameList(nf, fnm)
        Dim secs As New Dictionary(Of String, Integer)
        For Each nm In fnm
            Dim prop As String = "", sa As String = ""
            m.FrameObj.GetSection(nm, prop, sa)
            secs(prop) = If(secs.ContainsKey(prop), secs(prop) + 1, 1)
        Next
        P("thesis frames " & nf & "; sections: " & String.Join("; ", secs.Select(Function(kv) kv.Key & " x" & kv.Value)))
        For Each s In secs.Keys
            Dim fn As String = "", mp As String = "", t3 As Double, t2 As Double, col As Integer, nt As String = "", gd As String = ""
            If m.PropFrame.GetRectangle(s, fn, mp, t3, t2, col, nt, gd) = 0 Then P("   section " & s & ": " & mp & " T3 " & F(t3 * 1000, "F0") & " T2 " & F(t2 * 1000, "F0") & " mm")
        Next
        Dim nlp As Integer, lpn() As String = Nothing
        m.LoadPatterns.GetNameList(nlp, lpn)
        For Each l In lpn
            Dim swm As Double
            m.LoadPatterns.GetSelfWTMultiplier(l, swm)
            P("   pattern " & l & " self weight multiplier " & F(swm, "F2"))
        Next
        Dim nd As Integer, frn() As String = Nothing, lp() As String = Nothing, mt() As Integer = Nothing, cs() As String = Nothing, dr() As Integer = Nothing, rd1() As Double = Nothing, rd2() As Double = Nothing, d1() As Double = Nothing, d2() As Double = Nothing, v1() As Double = Nothing, v2() As Double = Nothing
        If m.FrameObj.GetLoadDistributed("All", nd, frn, lp, mt, cs, dr, rd1, rd2, d1, d2, v1, v2, eItemType.Group) = 0 Then
            Dim sums As New Dictionary(Of String, List(Of Double))
            For i As Integer = 0 To nd - 1
                Dim k As String = lp(i) & " dir" & dr(i)
                If Not sums.ContainsKey(k) Then sums(k) = New List(Of Double)
                sums(k).Add(v1(i))
            Next
            For Each kv In sums : P("   distributed loads " & kv.Key & ": " & kv.Value.Count & " items, values " & String.Join(" ", kv.Value.Distinct().Select(Function(v) F(v, "F2"))) & " kN/m") : Next
        End If
        Dim nc As Integer, cn() As String = Nothing
        m.RespCombo.GetNameList(nc, cn)
        For Each c In cn
            Dim n As Integer, ct() As eCNameType = Nothing, cnm() As String = Nothing, sf() As Double = Nothing
            m.RespCombo.GetCaseList(c, n, ct, cnm, sf)
            P("   combo " & c & ": " & String.Join(" + ", Enumerable.Range(0, n).Select(Function(i) F(sf(i), "F2") & " " & cnm(i))))
        Next
    End Sub

    Function Main(args() As String) As Integer
        outDir = args(0) : workDir = args(1)
        Directory.CreateDirectory(outDir) : Directory.CreateDirectory(workDir)
        Dim xPilot = ReadX(args(2)), xK1 = ReadX(args(3))
        Dim thesis As String = If(args.Length > 4, args(4), "")
        P("EtabsEx2 start " & DateTime.Now.ToString("s"))
        For Each t In {GetType(Ex2Problem), GetType(FemModel), GetType(PriceSet), GetType(ColumnDesign)}
            Using fs = File.OpenRead(t.Assembly.Location)
                Using sha = System.Security.Cryptography.SHA256.Create()
                    P(Path.GetFileName(t.Assembly.Location) & " sha256=" & BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", "").ToLowerInvariant())
                End Using
            End Using
        Next
        'T1: source Table 1 sections, 5 groups (V1 b, c and V2-a forces)
        Dim spec5 As New FrameSpec2D With {.Groups = 5}
        Dim defs5 As New Dictionary(Of String, RcSectionDef)
        For Each kv In Ex2Problem.SourceTable1() : defs5(kv.Key) = New RcSectionDef With {.B = kv.Value(0), .H = kv.Value(1), .Fc = 30.0} : Next
        Dim t1 As New CaseDef With {.Name = "T1", .Analysis = RcCombos2D.Analyze(spec5, defs5), .Defs = defs5}
        P("T1 FEM vertical reactions: SW " & F(t1.Analysis.VerticalReaction("SW"), "F4") & " G " & F(t1.Analysis.VerticalReaction("G"), "F4") & " Q " & F(t1.Analysis.VerticalReaction("Q"), "F4") & " U1 " & F(t1.Analysis.VerticalReaction(t1.Analysis.Combos(0)), "F4") & " U2 " & F(t1.Analysis.VerticalReaction(t1.Analysis.Combos(1)), "F4") & " kN")
        Dim pilot = MakeCase("PILOT", xPilot), k1 = MakeCase("K1", xK1)

        Dim helper As cHelper = New Helper()
        Dim obj As cOAPI = helper.CreateObjectProgID("CSI.ETABS.API.ETABSObject")
        Chk(obj.ApplicationStart(), "ApplicationStart")
        Dim m As cSapModel = obj.SapModel
        Dim ver As String = "", vn As Double = 0
        m.GetVersion(ver, vn)
        P("ETABS version " & ver & " (" & vn.ToString(ci) & ")")
        Try
            RunCase(m, t1, False)
            RunCase(m, pilot, True)
            RunCase(m, k1, True)
            If thesis <> "" Then ReadThesisEdb(m, thesis)
        Catch ex As Exception
            P("EXCEPTION " & ex.GetType().Name & ": " & ex.Message & vbCrLf & ex.StackTrace)
        Finally
            Try : obj.ApplicationExit(False) : Catch : End Try
            File.WriteAllText(Path.Combine(outDir, "etabs_ex2_log.txt"), log.ToString())
        End Try
        Return 0
    End Function
End Module
