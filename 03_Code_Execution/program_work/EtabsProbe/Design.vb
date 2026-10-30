Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports ETABSv1
Imports FemCore
Imports FemRcModel
Imports RcCore

'V2b: ETABS Concrete Frame Design (ACI 318-14) column PMM ratios against the program's column D/C, element by element and combination by combination.
'The same member forces enter both (FemCore forces agree with ETABS to 0.002 %, V2a). Columns get a symmetric perimeter layout (n bars per face, 20 mm bars, 40 mm cover to the ties,
'10 mm ties, centroid 60 mm) in ETABS and in the program section. Program D/C = RcCore.AciColumn.BiaxialDcr (linear sum); variant with minimum eccentricity (0.6 in + 0.03 h, one
'direction at a time) as ETABS applies it. ETABS: radial ray ratio OL/OC (CFD-ACI-318-14 Chap. 3.4.2.3). Environment: DESIGN=1.
Module DesignProbe
    Dim ci As CultureInfo = CultureInfo.InvariantCulture
    Const Dia As Double = 20.0, TieD As Double = 10.0, CoverTie As Double = 40.0, CoverCentroid As Double = 60.0
    Dim Rho As Double = If(Environment.GetEnvironmentVariable("RHO") Is Nothing, 0.015, Double.Parse(Environment.GetEnvironmentVariable("RHO"), CultureInfo.InvariantCulture))     'production reinforcement ratio of the test columns

    Function BarsPerFace(group As String) As Integer
        Return If(group.EndsWith("_S12"), 5, 4)
    End Function

    Function SymSection(Bb As Double, Hb As Double, fc As Double, n As Integer) As ColumnSection
        Dim cov As Double = CoverTie + TieD + Dia / 2.0
        Dim ab As Double = Math.PI * Dia * Dia / 4.0
        Dim ys(n - 1) As Double, ar(n - 1) As Double
        For k As Integer = 0 To n - 1
            ys(k) = cov + (Hb - 2.0 * cov) * k / (n - 1)
            ar(k) = If(k = 0 OrElse k = n - 1, n, 2) * ab
        Next
        Return New ColumnSection With {.B = Bb, .H = Hb, .Fc = fc, .Fy = 420.0, .LayerY = ys, .LayerAs = ar}
    End Function

    'radial capacity ratio OL/OC of the program's own (linear-sum) interaction: load point scaled by lam until BiaxialDcr = 1, ratio = 1/lam
    Function Radial(sx As ColumnSection, sy As ColumnSection, pu As Double, m3 As Double, m2 As Double) As Double
        If Math.Abs(pu) < 1.0 AndAlso m3 < 1.0 AndAlso m2 < 1.0 Then Return 0.0
        Dim hi As Double = 50.0
        If pu > 0 Then hi = Math.Min(hi, AciColumn.DesignPnMax(sx) / pu)
        Dim lo As Double = 0.0
        If AciColumn.BiaxialDcr(sx, sy, hi * pu, hi * m3, hi * m2) < 1.0 Then Return 1.0 / hi
        For i As Integer = 0 To 40
            Dim mid As Double = 0.5 * (lo + hi)
            If AciColumn.BiaxialDcr(sx, sy, mid * pu, mid * m3, mid * m2) < 1.0 Then lo = mid Else hi = mid
        Next
        Return 1.0 / (0.5 * (lo + hi))
    End Function

    Class ComboDef
        Public Name As String
        Public Terms As New List(Of Tuple(Of String, Double))
        Public ProgramIndex As Integer       'index in fa.Combos
    End Class

    Function Combos(tp As TbdyParams) As List(Of ComboDef)
        Dim ez As Double = 0.3 * (2.0 / 3.0) * tp.SDS
        Dim l As New List(Of ComboDef)
        l.Add(New ComboDef With {.Name = "U1", .ProgramIndex = 0, .Terms = New List(Of Tuple(Of String, Double)) From {Tuple.Create("G", 1.4), Tuple.Create("Q", 1.6), Tuple.Create("S", 1.6)}})
        Dim ix As Integer = 1
        For Each ex In {Tuple.Create("EX", 1.0, "EY", 0.3, "X+0.3Y"), Tuple.Create("EY", 1.0, "EX", 0.3, "0.3X+Y")}
            Dim c1 As New ComboDef With {.Name = "E411_" & ex.Item5, .ProgramIndex = ix}
            c1.Terms.AddRange({Tuple.Create("G", 1.0 + ez), Tuple.Create("Q", 1.0), Tuple.Create("S", 0.2), Tuple.Create(ex.Item1, ex.Item2), Tuple.Create(ex.Item3, ex.Item4)})
            Dim c2 As New ComboDef With {.Name = "E412_" & ex.Item5, .ProgramIndex = ix + 1}
            c2.Terms.AddRange({Tuple.Create("G", 0.9 - ez), Tuple.Create(ex.Item1, ex.Item2), Tuple.Create(ex.Item3, ex.Item4)})
            l.Add(c1) : l.Add(c2) : ix += 2
        Next
        Return l
    End Function

    Sub Run(fa As FrameAnalysis, spec As FrameSpec, tp As TbdyParams, cracked As Boolean, P As Action(Of String), Chk As Action(Of Integer, String))
        Dim helper As cHelper = New Helper()
        Dim obj As cOAPI = helper.CreateObjectProgID("CSI.ETABS.API.ETABSObject")
        Chk(obj.ApplicationStart(), "ApplicationStart")
        Dim m As cSapModel = obj.SapModel
        Try
            Dim pts As New Dictionary(Of Integer, String), frames As New Dictionary(Of Integer, String)
            Dim fm = fa.Gross.Model
            Program.BuildEtabs(m, fm, pts, frames)
            'concrete strength and rebar
            Dim defs = Program.Defs()
            For Each s In fm.Sections
                Chk(m.PropMaterial.SetOConcrete_1(s.MaterialName, defs(s.Name).Fc * 1000.0, False, 1.0, 2, 0, 0.002, 0.003, -0.1), "SetOConcrete_1 " & s.MaterialName)
            Next
            Chk(m.PropMaterial.SetMaterial("R420", eMatType.Rebar), "SetMaterial rebar")
            Chk(m.PropMaterial.SetORebar_1("R420", 420000.0, 500000.0, 462000.0, 550000.0, 1, 0, 0.01, 0.09, -0.1, False), "SetORebar_1")
            Dim okSize As String = ""
            For Each s In fm.Sections
                If Not s.Name.StartsWith("C") Then Continue For
                Dim dsec = defs(s.Name)
                Dim nTot As Integer = ColumnDesign.BarCount(dsec.B, dsec.H, Rho, Dia)
                Dim nW As Integer, nD As Integer
                ColumnDesign.PerimeterCounts(dsec.B, dsec.H, nTot, nW, nD)
                Dim nr3 As Integer = If(Environment.GetEnvironmentVariable("SWAPNR") = "1", nD, nW), nr2 As Integer = If(Environment.GetEnvironmentVariable("SWAPNR") = "1", nW, nD)
                P("column " & s.Name & ": " & dsec.B & "x" & dsec.H & " N=" & nTot & " (width side " & nW & ", depth side " & nD & ") ETABS NR3=" & nr3 & " NR2=" & nr2)
                Dim ret As Integer = -1
                For Each rs In {"20", "#20", "Ø20", "20mm"}
                    For Each ts In {"10", "#10", "Ø10", "10mm"}
                        ret = m.PropFrame.SetRebarColumn(s.Name, "R420", "R420", 1, 1, CoverTie / 1000.0, 0, nr3, nr2, rs, ts, 0.15, 2, 2, False)
                        If ret = 0 Then okSize = rs & "/" & ts : Exit For
                    Next
                    If ret = 0 Then Exit For
                Next
                Chk(ret, "SetRebarColumn " & s.Name)
            Next
            P("rebar size names accepted: " & okSize)
            Dim jointMode As Boolean = Environment.GetEnvironmentVariable("JOINT") = "1"
            Dim beamRes As New Dictionary(Of String, BeamGroupResult)
            If jointMode Then
                'beam reinforcement of the program design is assigned BEFORE the analysis (the model is locked afterwards)
                For Each g In fa.Gross.BeamGroups
                    Dim bdef = defs(g)
                    beamRes(g) = BeamDesign.DesignGroup(fa, g, bdef.B, bdef.H, bdef.Fc, Nothing)
                    P("beam group " & g & ": As top " & beamRes(g).AsTop.ToString("F0", ci) & " bot " & beamRes(g).AsBot.ToString("F0", ci) & " mm2 (provided to ETABS)")
                    Chk(m.PropFrame.SetRebarBeam(g, "R420", "R420", CoverCentroid / 1000.0, CoverCentroid / 1000.0, beamRes(g).AsTop * 1.0E-6, beamRes(g).AsTop * 1.0E-6, beamRes(g).AsBot * 1.0E-6, beamRes(g).AsBot * 1.0E-6), "SetRebarBeam " & g)
                Next
            End If
            If cracked Then
                Program.SetCracked(m, fa.Cracked.Model)
                Program.SetMassSourceAndRs(m, fa, "SPD")
            End If
            'combinations
            Dim cl = Combos(tp).Where(Function(c) (c.ProgramIndex = 0) <> cracked).ToList()
            For Each c In cl
                Chk(m.RespCombo.Add(c.Name, 0), "RespCombo.Add " & c.Name)
                For Each t In c.Terms
                    Dim ty As eCNameType = eCNameType.LoadCase
                    Chk(m.RespCombo.SetCaseList(c.Name, ty, t.Item1, t.Item2), "SetCaseList " & c.Name & " " & t.Item1)
                Next
            Next
            Dim dir As String = Path.Combine(Path.GetTempPath(), "etabs_v2")
            Directory.CreateDirectory(dir)
            Chk(m.File.Save(Path.Combine(dir, If(cracked, "ex1_v2b_cracked.edb", "ex1_v2b_gross.edb"))), "Save")
            If Not cracked Then Chk(m.Analyze.SetRunCaseFlag("Modal", False, False), "run flag Modal off")
            Chk(m.Analyze.RunAnalysis(), "RunAnalysis")
            Chk(m.DesignConcrete.SetCode("ACI 318-14"), "DesignConcrete.SetCode")
            'Z8: dump of the ETABS design preferences (1-18) and the overwrites (1-13) of one column and one beam, as the program uses them (not changed by the probe)
            If Environment.GetEnvironmentVariable("NPTS") <> Nothing Then Chk(m.DesignConcrete.ACI318_14.SetPreference(2, Double.Parse(Environment.GetEnvironmentVariable("NPTS"), ci)), "SetPreference interaction points")
            If Environment.GetEnvironmentVariable("NCURV") <> Nothing Then Chk(m.DesignConcrete.ACI318_14.SetPreference(1, Double.Parse(Environment.GetEnvironmentVariable("NCURV"), ci)), "SetPreference interaction curves")
            Dim pv As Double, pdv As Boolean
            Dim pn() As String = {"", "Number of interaction curves", "Number of interaction points", "Consider minimum eccentricity", "Design for B/C capacity ratio", "Seismic design category", "Design system Omega0", "Design system Rho", "Design system Sds", "Consider ICC_ESR 2017", "Phi tension controlled", "Phi compression controlled tied", "Phi compression controlled spiral", "Phi shear and/or torsion", "Phi shear seismic", "Phi joint shear", "Pattern live load factor", "Utilization factor limit", "Multi-response case design"}
            For it As Integer = 1 To 18
                If m.DesignConcrete.ACI318_14.GetPreference(it, pv) = 0 Then P("PREF " & it & " " & pn(it) & " = " & pv.ToString(ci))
            Next
            Dim ovn() As String = {"", "Framing type", "Live load reduction factor", "Unbraced length ratio major", "Unbraced length ratio minor", "K major", "K minor", "Cm major", "Cm minor", "Db major", "Db minor", "Ds major", "Ds minor", "Consider minimum eccentricity"}
            For ei2 As Integer = 0 To fm.Elements.Count - 1
                If ei2 <> 0 AndAlso Not fm.Elements(ei2).Name.StartsWith("B") Then Continue For
                If ei2 <> 0 AndAlso fm.Elements(ei2).Name <> fm.Elements.First(Function(x) x.Name.StartsWith("B")).Name Then Continue For
                For it As Integer = 1 To 13
                    Dim pd2 As Boolean
                    If m.DesignConcrete.ACI318_14.GetOverwrite(frames(ei2), it, pv, pd2) = 0 Then P("OVERWRITE " & fm.Elements(ei2).Name & " " & it & " " & ovn(it) & " = " & pv.ToString(ci) & If(pd2, " (program determined)", ""))
                Next
            Next
            'R3: same slenderness inputs in ETABS and in the program: unbraced length ratio = clear height / element length (ACI 6.2.5.1), K = 1, Cm = 1 (explicit overwrites)
            For ei As Integer = 0 To fm.Elements.Count - 1
                Dim ec = fm.Elements(ei)
                If Not ec.Name.StartsWith("C") Then Continue For
                Dim lr As Double = (ec.Length - ec.OffsetI - ec.OffsetJ) / ec.Length
                If cracked Then Chk(m.DesignConcrete.ACI318_14.SetOverwrite(frames(ei), 1, 1.0, eItemType.Objects), "SetOverwrite column framing type special")
                For item As Integer = 3 To 4 : Chk(m.DesignConcrete.ACI318_14.SetOverwrite(frames(ei), item, lr, eItemType.Objects), "SetOverwrite lu " & item) : Next
                For item As Integer = 5 To 8 : Chk(m.DesignConcrete.ACI318_14.SetOverwrite(frames(ei), item, 1.0, eItemType.Objects), "SetOverwrite K/Cm " & item) : Next
            Next
            If Environment.GetEnvironmentVariable("NOMAG") = "1" Then
                'V2d diagnostic: switch off the non-sway / sway moment magnification (overwrite items 9-12 = 1) to isolate the slenderness effect
                For ei As Integer = 0 To fm.Elements.Count - 1
                    If Not fm.Elements(ei).Name.StartsWith("C") Then Continue For
                    For item As Integer = 9 To 12 : Chk(m.DesignConcrete.ACI318_14.SetOverwrite(frames(ei), item, 1.0, eItemType.Objects), "SetOverwrite " & item) : Next
                Next
            End If
            Dim colRes As New Dictionary(Of String, ColumnGroupResult), colSh As New Dictionary(Of String, ColumnShearResult)
            If jointMode Then
                For Each g In fa.Gross.ColumnGroups
                    Dim cdef = defs(g)
                    colRes(g) = ColumnDesign.CheckGroup(fa, g, cdef.B, cdef.H, cdef.Fc, Rho, Dia)
                Next
                For Each g In fa.Gross.ColumnGroups
                    colSh(g) = ColumnShear.Design(fa, colRes(g), colRes, beamRes, Nothing)
                Next
            End If
            Dim nc As Integer, names() As String = Nothing
            m.RespCombo.GetNameList(nc, names)
            P("combinations in model: " & String.Join(", ", names))
            For Each c In cl
                For Each nm In names
                    m.DesignConcrete.SetComboStrength(nm, nm = c.Name)
                Next
                Dim rd As Integer = m.DesignConcrete.StartDesign()
                P("=== design combo " & c.Name & " (StartDesign ret=" & rd & ")")
                Dim nI As Integer, fn() As String = Nothing, opt() As Integer = Nothing, loc() As Double = Nothing, pc() As String = Nothing, pa() As Double = Nothing, pr() As Double = Nothing, vc() As String = Nothing, av() As Double = Nothing, vc2() As String = Nothing, av2() As Double = Nothing, es() As String = Nothing, ws() As String = Nothing
                Dim rr As Integer = m.DesignConcrete.GetSummaryResultsColumn("All", nI, fn, opt, loc, pc, pa, pr, vc, av, vc2, av2, es, ws, eItemType.Group)
                P("GetSummaryResultsColumn ret=" & rr & " items=" & nI)
                Dim byFrame As New Dictionary(Of String, Double), byCombo As New Dictionary(Of String, String)
                Dim byAv As New Dictionary(Of String, Double)
                Dim sumAv0 As Double = 0, sumAv1 As Double = 0, minAv0 As Double = 1.0E9, maxAv0 As Double = 0, minAv1 As Double = 1.0E9, maxAv1 As Double = 0, nAv As Integer = 0
                For i As Integer = 0 To nI - 1
                    If opt(i) <> 1 Then Continue For
                    If Not byFrame.ContainsKey(fn(i)) OrElse pr(i) > byFrame(fn(i)) Then byFrame(fn(i)) = pr(i) : byCombo(fn(i)) = pc(i)
                    Dim avi As Double = Math.Max(av(i), av2(i)) * 1000.0           'm2/m -> mm2/mm
                    If Not byAv.ContainsKey(fn(i)) OrElse avi > byAv(fn(i)) Then byAv(fn(i)) = avi
                Next
                Dim sumR0 As Double = 0, sumR1 As Double = 0, minR1 As Double = 1.0E9, maxR1 As Double = 0, n10 As Integer = 0
                Dim wE As Double = 0, wP0 As Double = 0, wP1 As Double = 0, nCmp As Integer = 0, nCons As Integer = 0, nCons1 As Integer = 0
                Dim rows As New List(Of String)
                Dim nCons2 As Integer = 0, minR2 As Double = 1.0E9, maxR2 As Double = 0, n10p As Integer = 0, wP2 As Double = 0
                For ei As Integer = 0 To fm.Elements.Count - 1
                    Dim e = fm.Elements(ei)
                    If Not e.Name.StartsWith("C") Then Continue For
                    Dim fr As String = frames(ei)
                    If Not byFrame.ContainsKey(fr) Then Continue For
                    Dim sec = e.Section
                    Dim d = defs(sec.Name), nb As Integer = BarsPerFace(sec.Name)
                    Dim nTot2 As Integer = ColumnDesign.BarCount(d.B, d.H, Rho, Dia)
                    Dim sx = ColumnDesign.MakeSection(d.B, d.H, d.Fc, nTot2, Dia), sy = ColumnDesign.MakeSection(d.H, d.B, d.Fc, nTot2, Dia, False, True)      'production layout (both directions from the same perimeter pattern)
                    Dim cvx As New ColumnDesign.ColCurve(sx), cvy As New ColumnDesign.ColCurve(sy)
                    Dim cs = fa.Combos(c.ProgramIndex)
                    Dim p0 As Double = 0, p1 As Double = 0, p2 As Double = 0
                    Dim loads As New List(Of Double())      'pu N, m3, m2 N.mm
                    For endI As Integer = 0 To 1
                        If cs.Seismic AndAlso cs.Forces.Count = 2 Then
                            Dim fp = cs.Forces(0)(ei), fq = cs.Forces(1)(ei)
                            Dim ip As Integer = If(endI = 0, 0, 6), i2 As Integer = If(endI = 0, 4, 10), i3 As Integer = If(endI = 0, 5, 11)
                            Dim sg As Double = If(endI = 0, 1.0, -1.0)
                            Dim gP As Double = sg * 0.5 * (fp(ip) + fq(ip)), eP As Double = sg * 0.5 * (fp(ip) - fq(ip))
                            Dim g2 As Double = 0.5 * (fp(i2) + fq(i2)), e2 As Double = 0.5 * (fp(i2) - fq(i2))
                            Dim g3 As Double = 0.5 * (fp(i3) + fq(i3)), e3 As Double = 0.5 * (fp(i3) - fq(i3))
                            For Each s1 In {-1.0, 1.0}
                                For Each s2 In {-1.0, 1.0}
                                    For Each s3 In {-1.0, 1.0}
                                        Dim puS As Double = (gP + s1 * eP) * 1000.0
                                        Dim luS As Double = (e.Length - e.OffsetI - e.OffsetJ) * 1000.0
                                        Dim pgS As Double = (1.0 + fa.Ez) * fa.Rg.FaceForce(fm.PatternIndex("G"))(ei)(0) * 1000.0
                                        Dim bS As Double = If(puS > 0.0, Math.Abs(pgS) / puS, 0.0)
                                        loads.Add({puS, Math.Abs(g3 + s3 * e3) * 1.0E6, Math.Abs(g2 + s2 * e2) * 1.0E6, ColumnDesign.DeltaNs(d.Fc, d.H, d.B, luS, puS, bS, 1.0, 1.0), ColumnDesign.DeltaNs(d.Fc, d.B, d.H, luS, puS, bS, 1.0, 1.0)})
                                    Next
                                Next
                            Next
                        Else
                            For k As Integer = 0 To cs.Forces.Count - 1
                                Dim f = cs.Forces(k)(ei)
                                Dim puN As Double = If(endI = 0, f(0), -f(6)) * 1000.0
                                Dim lu As Double = (e.Length - e.OffsetI - e.OffsetJ) * 1000.0
                                Dim pgI As Double = 1.4 * fa.Rg.FaceForce(fm.PatternIndex("G"))(ei)(0) * 1000.0
                                Dim beta As Double = If(puN > 0.0, Math.Abs(pgI) / puN, 0.0)
                                loads.Add({puN, Math.Abs(If(endI = 0, f(5), f(11))) * 1.0E6, Math.Abs(If(endI = 0, f(4), f(10))) * 1.0E6, ColumnDesign.DeltaNs(d.Fc, d.H, d.B, lu, puN, beta, -f(5) * 1.0E6, f(11) * 1.0E6), ColumnDesign.DeltaNs(d.Fc, d.B, d.H, lu, puN, beta, -f(4) * 1.0E6, f(10) * 1.0E6)})
                            Next
                        End If
                    Next
                    For Each ld In loads
                        Dim pu As Double = ld(0), m3 As Double = ld(1), m2 As Double = ld(2)
                        p0 = Math.Max(p0, Radial(sx, sy, pu, m3, m2))
                        p2 = Math.Max(p2, ColumnDesign.DcrWithEffects(cvx, cvy, pu, m3, m2, ld(3), ld(4)))        'production constraint (ColumnDesign.CheckGroup), no e_min, linear sum
                        Dim emin As Double = If(pu > 0, pu * (15.24 + 0.03 * d.H), 0.0)
                        Dim emin2 As Double = If(pu > 0, pu * (15.24 + 0.03 * d.B), 0.0)
                        p1 = Math.Max(p1, Math.Max(Radial(sx, sy, pu, ld(3) * Math.Max(m3, emin), ld(4) * m2), Radial(sx, sy, pu, ld(3) * m3, ld(4) * Math.Max(m2, emin2))))
                    Next
                    If c.ProgramIndex <> 0 AndAlso byAv.ContainsKey(fr) AndAlso byAv(fr) > 1.0E-6 Then
                        'ACI 18.7.6: Ve = 2 Mpr / L (ETABS: overwritten length factor -> full span), Mpr at the worst of the combination axial forces, phi = 0.75; Vc = 0 when Pu < Ag f'c / 20
                        Dim spr = ColumnDesign.MakeSection(d.B, d.H, d.Fc, nTot2, Dia) : spr.Fy = 1.25 * 420.0
                        Dim pLo As Double = loads.Min(Function(x) x(0)), pHi As Double = loads.Max(Function(x) x(0))
                        Dim mpr As Double = ColumnShear.MprMax(spr, pLo, pHi)
                        Dim vE As Double = If(jointMode AndAlso colSh.ContainsKey(e.Group), Math.Max(colSh(e.Group).Ve(0), colSh(e.Group).Ve(1)) * ((e.Length - e.OffsetI - e.OffsetJ) / e.Length), 2.0 * mpr / (e.Length * 1000.0))      'production Ve (beam-limited, clear ln) converted to the full-span length ETABS uses with the overwritten length factor
                        Dim agc As Double = d.B * d.H
                        Dim vcz As Boolean = pLo < agc * d.Fc / 20.0
                        Dim res2(1) As Double
                        For vv As Integer = 0 To 1
                            Dim dd2 As Double = If(vv = 0, d.H - 50.0, 0.8 * d.H)
                            Dim vc2x As Double = If(vcz, 0.0, 0.17 * (1.0 + Math.Max(pLo, 0.0) / (14.0 * agc)) * Math.Sqrt(d.Fc) * d.B * dd2)
                            res2(vv) = Math.Max(vE / 0.75 - vc2x, 0.0) / (420.0 * dd2)
                        Next
                        nAv += 1
                        sumAv0 += res2(0) / byAv(fr) : sumAv1 += res2(1) / byAv(fr)
                        minAv0 = Math.Min(minAv0, res2(0) / byAv(fr)) : maxAv0 = Math.Max(maxAv0, res2(0) / byAv(fr))
                        minAv1 = Math.Min(minAv1, res2(1) / byAv(fr)) : maxAv1 = Math.Max(maxAv1, res2(1) / byAv(fr))
                    End If
                    Dim er As Double = byFrame(fr)
                    rows.Add(e.Name.PadRight(14) & fr.PadRight(6) & " ETABS " & er.ToString("F3", ci) & " [" & byCombo(fr) & "]  program radial " & p0.ToString("F3", ci) & "  program+emin " & p1.ToString("F3", ci) & "  PRODUCTION(e_min+delta_ns) " & p2.ToString("F3", ci))
                    nCmp += 1
                    If p0 >= er - 1.0E-9 Then nCons += 1
                    If p1 >= er - 1.0E-9 Then nCons1 += 1
                    If p2 >= er - 1.0E-9 Then nCons2 += 1
                    If er > 0.02 Then minR2 = Math.Min(minR2, p2 / er) : maxR2 = Math.Max(maxR2, p2 / er) : If Math.Abs(p2 / er - 1.0) <= 0.10 Then n10p += 1
                    wP2 = Math.Max(wP2, p2)
                    If er > 0.02 Then
                        sumR0 += p0 / er : sumR1 += p1 / er : minR1 = Math.Min(minR1, p1 / er) : maxR1 = Math.Max(maxR1, p1 / er)
                        If Math.Abs(p1 / er - 1.0) <= 0.10 Then n10 += 1
                    End If
                    wE = Math.Max(wE, er) : wP0 = Math.Max(wP0, p0) : wP1 = Math.Max(wP1, p1)
                Next
                For Each r In rows : P(r) : Next
                P("SUMMARY " & c.Name & ": columns " & nCmp & ", max D/C ETABS " & wE.ToString("F3", ci) & ", program " & wP0.ToString("F3", ci) & ", program+emin " & wP1.ToString("F3", ci) & "; program >= ETABS in " & nCons & " / " & nCmp & " (with emin " & nCons1 & ")")
                P("RATIO program/ETABS (columns with ETABS D/C>0.02): mean no-emin " & (sumR0 / Math.Max(nCmp, 1)).ToString("F3", ci) & ", mean with emin " & (sumR1 / Math.Max(nCmp, 1)).ToString("F3", ci) & ", min " & minR1.ToString("F3", ci) & ", max " & maxR1.ToString("F3", ci) & ", within +-10 % (emin): " & n10 & " / " & nCmp)
                If nAv > 0 Then P("COLSHEAR " & c.Name & ": columns " & nAv & "; Av/s program(d = h - 50)/ETABS mean " & (sumAv0 / nAv).ToString("F3", ci) & " min " & minAv0.ToString("F3", ci) & " max " & maxAv0.ToString("F3", ci) & "; program(d = 0.8 h)/ETABS mean " & (sumAv1 / nAv).ToString("F3", ci) & " min " & minAv1.ToString("F3", ci) & " max " & maxAv1.ToString("F3", ci))
                P("PRODUCTION " & c.Name & ": max D/C ETABS " & wE.ToString("F3", ci) & ", production " & wP2.ToString("F3", ci) & "; production >= ETABS in " & nCons2 & " / " & nCmp & "; ratio production/ETABS min " & minR2.ToString("F3", ci) & " max " & maxR2.ToString("F3", ci) & "; within +-10 % " & n10p & " / " & nCmp)
            Next
            If jointMode AndAlso cracked Then
                For Each nm In names : m.DesignConcrete.SetComboStrength(nm, True) : Next
                Chk(m.DesignConcrete.StartDesign(), "StartDesign all seismic combos")
                Dim nJ As Integer = 0
                Dim jf As New List(Of String), js1 As New List(Of Double), js2 As New List(Of Double), bc1 As New List(Of Double), bc2 As New List(Of Double), lj1 As New List(Of String)
                For ei0 As Integer = 0 To fm.Elements.Count - 1
                    If Not fm.Elements(ei0).Name.StartsWith("C") Then Continue For
                    Dim n1 As Integer, f1() As String = Nothing, a1() As String = Nothing, b1() As Double = Nothing, a2() As String = Nothing, b2() As Double = Nothing, a3() As String = Nothing, b3() As Double = Nothing, a4() As String = Nothing, b4() As Double = Nothing, e1() As String = Nothing, w1() As String = Nothing
                    Dim rj1 As Integer = m.DesignConcrete.GetSummaryResultsJoint(frames(ei0), n1, f1, a1, b1, a2, b2, a3, b3, a4, b4, e1, w1, eItemType.Objects)
                    If rj1 = 0 AndAlso n1 > 0 Then
                        jf.Add(f1(0)) : js1.Add(b1(0)) : js2.Add(b2(0)) : bc1.Add(b3(0)) : bc2.Add(b4(0)) : lj1.Add(a1(0)) : nJ += 1
                    End If
                Next
                P("GetSummaryResultsJoint (per column) items=" & nJ)
                For Each tk In {"Concrete Column Shear Envelope - ACI 318-14", "Concrete Joint Design Summary - ACI 318-14", "Concrete Joint Envelope - ACI 318-14"}
                    Dim fk() As String = {""}
                    Dim tv As Integer = 0, fin() As String = Nothing, nrec As Integer = 0, tdat() As String = Nothing
                    Dim rt As Integer = m.DatabaseTables.GetTableForDisplayArray(tk, fk, "All", tv, fin, nrec, tdat)
                    P("DISPLAYTABLE " & tk & " ret=" & rt & " records=" & nrec & " fields=" & If(fin Is Nothing, "-", String.Join("|", fin)))
                    If tdat IsNot Nothing AndAlso fin IsNot Nothing Then
                        For i As Integer = 0 To Math.Min(nrec, 14) - 1
                            P("   " & String.Join("|", tdat.Skip(i * fin.Length).Take(fin.Length)))
                        Next
                    End If
                Next
                Dim ntb As Integer, tkey() As String = Nothing, tnm() As String = Nothing, timp() As Integer = Nothing
                m.DatabaseTables.GetAvailableTables(ntb, tkey, tnm, timp)
                For ti As Integer = 0 To ntb - 1
                    If tnm(ti).IndexOf("Joint", StringComparison.OrdinalIgnoreCase) >= 0 AndAlso tnm(ti).IndexOf("Concrete", StringComparison.OrdinalIgnoreCase) >= 0 Then P("TABLE " & tkey(ti) & " | " & tnm(ti))
                    If tnm(ti).IndexOf("Concrete Column", StringComparison.OrdinalIgnoreCase) >= 0 OrElse tnm(ti).IndexOf("Beam/Column", StringComparison.OrdinalIgnoreCase) >= 0 Then P("TABLE " & tkey(ti) & " | " & tnm(ti))
                Next
                Dim vtv As Integer = 0, vfin() As String = Nothing, vn As Integer = 0, vdat() As String = Nothing
                Dim vrt As Integer = m.DatabaseTables.GetTableForDisplayArray("Concrete Column Shear Envelope - ACI 318-14", New String() {""}, "All", vtv, vfin, vn, vdat)
                Dim vByEl As New Dictionary(Of String, Double()), vcombo As New Dictionary(Of String, String)
                If vrt = 0 AndAlso vdat IsNot Nothing Then
                    Dim iU As Integer = Array.IndexOf(vfin, "UniqueName"), iM As Integer = Array.IndexOf(vfin, "VMajor"), im2 As Integer = Array.IndexOf(vfin, "VMinor"), iC As Integer = Array.IndexOf(vfin, "VMajCombo")
                    For r0 As Integer = 0 To vn - 1
                        Dim nmv As String = vdat(r0 * vfin.Length + iU)
                        Dim v1 As Double = Double.Parse(vdat(r0 * vfin.Length + iM), ci), v2 As Double = Double.Parse(vdat(r0 * vfin.Length + im2), ci)
                        If Not vByEl.ContainsKey(nmv) Then vByEl(nmv) = New Double() {0.0, 0.0}
                        vByEl(nmv)(0) = Math.Max(vByEl(nmv)(0), v1) : vByEl(nmv)(1) = Math.Max(vByEl(nmv)(1), v2)
                        If vByEl(nmv)(0) = v1 Then vcombo(nmv) = vdat(r0 * vfin.Length + iC)
                    Next
                End If
                Dim sumVr As Double = 0, minVr As Double = 1.0E9, maxVr As Double = 0, nVr As Integer = 0
                For ei As Integer = 0 To fm.Elements.Count - 1
                    Dim ec = fm.Elements(ei)
                    If Not ec.Name.StartsWith("C") OrElse Not vByEl.ContainsKey(ec.Name) Then Continue For
                    Dim vProd As Double = Math.Max(colSh(ec.Group).Ve(0), colSh(ec.Group).Ve(1)) / 1000.0        'kN, production design shear of the group (clear length, group maximum)
                    Dim vEt As Double = Math.Max(vByEl(ec.Name)(0), vByEl(ec.Name)(1))
                    If vEt < 1.0E-6 Then Continue For
                    Dim rt As Double = vProd / vEt
                    nVr += 1 : sumVr += rt : minVr = Math.Min(minVr, rt) : maxVr = Math.Max(maxVr, rt)
                    If True Then P("COLVE " & ec.Name & " design shear ETABS " & vEt.ToString("F1", ci) & " kN [" & If(vcombo.ContainsKey(ec.Name), vcombo(ec.Name), "") & "] production " & vProd.ToString("F1", ci) & " kN ratio " & rt.ToString("F3", ci))
                Next
                If nVr > 0 Then P("COLVESUM columns " & nVr & ": production Ve / ETABS design shear: mean " & (sumVr / nVr).ToString("F3", ci) & " min " & minVr.ToString("F3", ci) & " max " & maxVr.ToString("F3", ci))
                Dim jsProg = JointShear.Check(fa, spec, colRes, colSh, beamRes, Nothing)
                Dim scProg = Checks.StrongColumn(fa, spec, colRes, beamRes, Nothing)
                Dim nm2 As New Dictionary(Of String, Integer)
                For ei As Integer = 0 To fm.Elements.Count - 1 : nm2(frames(ei)) = ei : Next
                Dim wJsE As Double = 0, wJsP As Double = 0, wBcE As Double = 0, wBcP As Double = 0, cntJ As Integer = 0
                For i As Integer = 0 To nJ - 1
                    If Not nm2.ContainsKey(jf(i)) Then Continue For
                    Dim en = fm.Elements(nm2(jf(i))).Name                      'C{i}{j}{k}: top joint = node (i, j, k)
                    If Not en.StartsWith("C") OrElse en.Length < 4 Then Continue For
                    Dim jn As String = "J" & en.Substring(1, 3)
                    Dim pjx = jsProg.FirstOrDefault(Function(x) x.Name = jn & "x"), pjy = jsProg.FirstOrDefault(Function(x) x.Name = jn & "y")
                    Dim pbx = scProg.FirstOrDefault(Function(x) x.Name = jn & "x"), pby = scProg.FirstOrDefault(Function(x) x.Name = jn & "y")
                    Dim jsE As Double = Math.Max(js1(i), js2(i)), bcE As Double = Math.Min(bc1(i), bc2(i))        'ETABS: sum Mnc / sum Mnb, the smaller direction is the critical one
                    Dim jsPv As Double = If(pjx Is Nothing, 0.0, Math.Max(pjx.G + 1.0, If(pjy Is Nothing, 0.0, pjy.G + 1.0)))
                    Dim rx As Double = If(pbx Is Nothing, 0.0, 1.2 / (pbx.G + 1.0)), ry As Double = If(pby Is Nothing, 1.0E9, 1.2 / (pby.G + 1.0))
                    Dim bcPv As Double = Math.Min(rx, ry)                          'program: sum Mnc / sum Mnb (g + 1 = 1.2 sum Mnb / sum Mnc)
                    P("JOINT " & en & " joint shear ratio ETABS " & jsE.ToString("F3", ci) & " program " & jsPv.ToString("F3", ci) & " | sum Mnc/sum Mnb ETABS " & bc1(i).ToString("F3", ci) & " / " & bc2(i).ToString("F3", ci) & " program x " & rx.ToString("F3", ci) & " y " & ry.ToString("F3", ci))
                    If pjx IsNot Nothing Then cntJ += 1 : wJsE = Math.Max(wJsE, jsE) : wJsP = Math.Max(wJsP, jsPv) : wBcE = If(wBcE = 0.0, bcE, Math.Min(wBcE, bcE)) : wBcP = If(wBcP = 0.0, bcPv, Math.Min(wBcP, bcPv))
                Next
                P("JOINTSUM columns " & cntJ & ": max joint shear ratio ETABS " & wJsE.ToString("F3", ci) & " program " & wJsP.ToString("F3", ci) & "; min sum Mnc/sum Mnb ETABS " & wBcE.ToString("F3", ci) & " program " & wBcP.ToString("F3", ci))
            End If
        Catch ex As Exception
            P("EXCEPTION " & ex.GetType().Name & ": " & ex.Message & vbCrLf & ex.StackTrace)
        Finally
            Try : obj.ApplicationExit(False) : Catch : End Try
        End Try
    End Sub
End Module
