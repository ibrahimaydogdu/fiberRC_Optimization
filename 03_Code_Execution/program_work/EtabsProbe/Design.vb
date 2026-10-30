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
    Const Dia As Double = 20.0, TieD As Double = 10.0, CoverTie As Double = 40.0

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
                Dim n As Integer = BarsPerFace(s.Name)
                Dim ret As Integer = -1
                For Each rs In {"20", "#20", "Ø20", "20mm"}
                    For Each ts In {"10", "#10", "Ø10", "10mm"}
                        ret = m.PropFrame.SetRebarColumn(s.Name, "R420", "R420", 1, 1, CoverTie / 1000.0, 0, n, n, rs, ts, 0.15, 2, 2, False)
                        If ret = 0 Then okSize = rs & "/" & ts : Exit For
                    Next
                    If ret = 0 Then Exit For
                Next
                Chk(ret, "SetRebarColumn " & s.Name)
            Next
            P("rebar size names accepted: " & okSize)
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
            If Environment.GetEnvironmentVariable("NOMAG") = "1" Then
                'V2d diagnostic: switch off the non-sway / sway moment magnification (overwrite items 9-12 = 1) to isolate the slenderness effect
                For ei As Integer = 0 To fm.Elements.Count - 1
                    If Not fm.Elements(ei).Name.StartsWith("C") Then Continue For
                    For item As Integer = 9 To 12 : Chk(m.DesignConcrete.ACI318_14.SetOverwrite(frames(ei), item, 1.0, eItemType.Objects), "SetOverwrite " & item) : Next
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
                For i As Integer = 0 To nI - 1
                    If opt(i) <> 1 Then Continue For
                    If Not byFrame.ContainsKey(fn(i)) OrElse pr(i) > byFrame(fn(i)) Then byFrame(fn(i)) = pr(i) : byCombo(fn(i)) = pc(i)
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
                    Dim sx = SymSection(d.B, d.H, d.Fc, nb), sy = SymSection(d.H, d.B, d.Fc, nb)
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
                                        loads.Add({(gP + s1 * eP) * 1000.0, Math.Abs(g3 + s3 * e3) * 1.0E6, Math.Abs(g2 + s2 * e2) * 1.0E6, 1.0, 1.0})
                                    Next
                                Next
                            Next
                        Else
                            For k As Integer = 0 To cs.Forces.Count - 1
                                Dim f = cs.Forces(k)(ei)
                                Dim puN As Double = If(endI = 0, f(0), -f(6)) * 1000.0
                                Dim lu As Double = ColumnDesign.LuRatio * e.Length * 1000.0
                                Dim pgI As Double = 1.4 * fa.Rg.FaceForce(fm.PatternIndex("G"))(ei)(0) * 1000.0
                                Dim beta As Double = If(puN > 0.0, Math.Abs(pgI) / puN, 0.0)
                                loads.Add({puN, Math.Abs(If(endI = 0, f(5), f(11))) * 1.0E6, Math.Abs(If(endI = 0, f(4), f(10))) * 1.0E6, ColumnDesign.DeltaNs(d.Fc, d.H, d.B, lu, puN, beta, -f(5) * 1.0E6, f(11) * 1.0E6), ColumnDesign.DeltaNs(d.Fc, d.B, d.H, lu, puN, beta, -f(4) * 1.0E6, f(10) * 1.0E6)})
                            Next
                        End If
                    Next
                    For Each ld In loads
                        Dim pu As Double = ld(0), m3 As Double = ld(1), m2 As Double = ld(2)
                        p0 = Math.Max(p0, Radial(sx, sy, pu, m3, m2))
                        p2 = Math.Max(p2, ColumnDesign.DcrWithEffects(sx, sy, pu, m3, m2, ld(3), ld(4)))        'production constraint (ColumnDesign.CheckGroup), no e_min, linear sum
                        Dim emin As Double = If(pu > 0, pu * (15.24 + 0.03 * d.H), 0.0)
                        Dim emin2 As Double = If(pu > 0, pu * (15.24 + 0.03 * d.B), 0.0)
                        p1 = Math.Max(p1, Math.Max(Radial(sx, sy, pu, ld(3) * Math.Max(m3, emin), ld(4) * m2), Radial(sx, sy, pu, ld(3) * m3, ld(4) * Math.Max(m2, emin2))))
                    Next
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
                P("PRODUCTION " & c.Name & ": max D/C ETABS " & wE.ToString("F3", ci) & ", production " & wP2.ToString("F3", ci) & "; production >= ETABS in " & nCons2 & " / " & nCmp & "; ratio production/ETABS min " & minR2.ToString("F3", ci) & " max " & maxR2.ToString("F3", ci) & "; within +-10 % " & n10p & " / " & nCmp)
            Next
        Catch ex As Exception
            P("EXCEPTION " & ex.GetType().Name & ": " & ex.Message & vbCrLf & ex.StackTrace)
        Finally
            Try : obj.ApplicationExit(False) : Catch : End Try
        End Try
    End Sub
End Module
