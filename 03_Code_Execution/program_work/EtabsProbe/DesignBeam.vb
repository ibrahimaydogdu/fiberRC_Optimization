Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports ETABSv1
Imports FemCore
Imports FemRcModel
Imports RcCore

'V2c: ETABS Concrete Frame Design (ACI 318-14) beam results against the program's plain-RC beam formulas, beam by beam and combination by combination.
'Flexure: required As (top, bottom) from the same member forces; ETABS = GetSummaryResultsBeam_2 (TopAreaReq, BotAreaReq, TopArea, BotArea); program = RcCore.AciBeam.RequiredAs
'(phi iteration, single reinforcement) with d = h - 60 mm. Shear (Av/s): gravity combination U1 against ETABS with framing type Ordinary; seismic combinations against ETABS Special
'(capacity shear V_p + V_D+L, ACI 18.6.5) recomputed with the program's AciBeam.Flexure (1.25 fy, phi = 1) on the ETABS reinforcement envelope. Environment: DESIGN=2.
Module DesignBeamProbe
    Dim ci As CultureInfo = CultureInfo.InvariantCulture
    Const CoverCentroid As Double = 60.0, Fy As Double = 420.0, PhiS As Double = 0.75

    Dim ProvA As Double = If(Environment.GetEnvironmentVariable("PROV") Is Nothing, 0.002, Double.Parse(Environment.GetEnvironmentVariable("PROV"), CultureInfo.InvariantCulture))

    Class BeamRes
        Public TopReq, BotReq, TopArea, BotArea, TopMin, BotMin, VArea, VMin As Double      'ETABS, mm2 and mm2/mm
        Public Rows As Integer
    End Class

    Function Ratio(a As Double, b As Double) As String
        If b < 1.0E-9 Then Return "  -  "
        Return (a / b).ToString("F3", ci)
    End Function

    'sum of gravity-pattern face shears (simple-beam shear), N
    Function SimpleShear(fa As FrameAnalysis, ei As Integer, terms() As Tuple(Of String, Double)) As Double
        Dim m = fa.Gross.Model
        Dim w As Double = 0.0
        For Each t In terms
            Dim f = fa.Rg.FaceForce(m.PatternIndex(t.Item1))(ei)
            w += t.Item2 * (f(1) + f(7))
        Next
        Return Math.Abs(w) / 2.0 * 1000.0
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
            Dim defs = Program.Defs()
            For Each s In fm.Sections
                Chk(m.PropMaterial.SetOConcrete_1(s.MaterialName, defs(s.Name).Fc * 1000.0, False, 1.0, 2, 0, 0.002, 0.003, -0.1), "SetOConcrete_1 " & s.MaterialName)
            Next
            Chk(m.PropMaterial.SetMaterial("R420", eMatType.Rebar), "SetMaterial rebar")
            Chk(m.PropMaterial.SetORebar_1("R420", 420000.0, 500000.0, 462000.0, 550000.0, 1, 0, 0.01, 0.09, -0.1, False), "SetORebar_1")
            For Each s In fm.Sections
                If Not s.Name.StartsWith("B") Then Continue For
                Chk(m.PropFrame.SetRebarBeam(s.Name, "R420", "R420", CoverCentroid / 1000.0, CoverCentroid / 1000.0, ProvA, ProvA, ProvA, ProvA), "SetRebarBeam " & s.Name)
            Next
            If cracked Then
                Program.SetCracked(m, fa.Cracked.Model)
                Program.SetMassSourceAndRs(m, fa, "SPD")
            End If
            Dim cl = DesignProbe.Combos(tp).Where(Function(c) (c.ProgramIndex = 0) <> cracked).ToList()
            For Each c In cl
                Chk(m.RespCombo.Add(c.Name, 0), "RespCombo.Add " & c.Name)
                For Each t In c.Terms
                    Dim ty As eCNameType = eCNameType.LoadCase
                    Chk(m.RespCombo.SetCaseList(c.Name, ty, t.Item1, t.Item2), "SetCaseList " & c.Name & " " & t.Item1)
                Next
            Next
            Dim dir As String = Path.Combine(Path.GetTempPath(), "etabs_v2")
            Directory.CreateDirectory(dir)
            Chk(m.File.Save(Path.Combine(dir, If(cracked, "ex1_v2c_cracked.edb", "ex1_v2c_gross.edb"))), "Save")
            If Not cracked Then Chk(m.Analyze.SetRunCaseFlag("Modal", False, False), "run flag Modal off")
            Chk(m.Analyze.RunAnalysis(), "RunAnalysis")
            Chk(m.DesignConcrete.SetCode("ACI 318-14"), "DesignConcrete.SetCode")
            'framing type: Ordinary for the gravity combination, Special for the seismic combinations
            For ei As Integer = 0 To fm.Elements.Count - 1
                If fm.Elements(ei).Name.StartsWith("B") Then Chk(m.DesignConcrete.ACI318_14.SetOverwrite(frames(ei), 1, If(cracked, 1.0, 3.0), eItemType.Objects), "SetOverwrite framing " & frames(ei))
            Next
            Dim nc As Integer, names() As String = Nothing
            m.RespCombo.GetNameList(nc, names)
            For Each c In cl
                For Each nm In names
                    m.DesignConcrete.SetComboStrength(nm, nm = c.Name)
                Next
                Dim rd As Integer = m.DesignConcrete.StartDesign()
                P("=== beam design combo " & c.Name & " (framing " & If(cracked, "special", "ordinary") & ", StartDesign ret=" & rd & ")")
                Dim nI As Integer, fn() As String = Nothing, loc() As Double = Nothing
                Dim tc() As String = Nothing, ta() As Double = Nothing, tar() As Double = Nothing, tam() As Double = Nothing, tap() As Double = Nothing
                Dim bc() As String = Nothing, ba() As Double = Nothing, bar() As Double = Nothing, bam() As Double = Nothing, bap() As Double = Nothing
                Dim vc() As String = Nothing, va() As Double = Nothing, var() As Double = Nothing, vam() As Double = Nothing, vap() As Double = Nothing
                Dim tlc() As String = Nothing, tla() As Double = Nothing, ttc() As String = Nothing, tta() As Double = Nothing, es() As String = Nothing, ws() As String = Nothing
                Dim rr As Integer = m.DesignConcrete.GetSummaryResultsBeam_2("All", nI, fn, loc, tc, ta, tar, tam, tap, bc, ba, bar, bam, bap, vc, va, var, vam, vap, tlc, tla, ttc, tta, es, ws, eItemType.Group)
                P("GetSummaryResultsBeam_2 ret=" & rr & " rows=" & nI)
                Dim res As New Dictionary(Of String, BeamRes)
                For i As Integer = 0 To nI - 1
                    If Not res.ContainsKey(fn(i)) Then res(fn(i)) = New BeamRes
                    Dim r = res(fn(i))
                    r.Rows += 1
                    r.TopReq = Math.Max(r.TopReq, tar(i) * 1.0E6) : r.BotReq = Math.Max(r.BotReq, bar(i) * 1.0E6)
                    r.TopArea = Math.Max(r.TopArea, ta(i) * 1.0E6) : r.BotArea = Math.Max(r.BotArea, ba(i) * 1.0E6)
                    r.TopMin = Math.Max(r.TopMin, tam(i) * 1.0E6) : r.BotMin = Math.Max(r.BotMin, bam(i) * 1.0E6)
                    r.VArea = Math.Max(r.VArea, va(i) * 1.0E3) : r.VMin = Math.Max(r.VMin, vam(i) * 1.0E3)
                Next
                Dim cs = fa.Combos(c.ProgramIndex)
                Dim gTerms = c.Terms.Where(Function(t) t.Item1 = "G" OrElse t.Item1 = "Q" OrElse t.Item1 = "S").ToList()
                Dim sT As New List(Of Double), sB As New List(Of Double), sTf As New List(Of Double), sBf As New List(Of Double), sV As New List(Of Double)
                Dim rows As New List(Of String), nB As Integer = 0
                Dim minPT As Double = 1.0E9, maxPT As Double = 0, minPB As Double = 1.0E9, maxPB As Double = 0
                Dim minV As Double = 1.0E9, maxV As Double = 0, nBTop As Integer = 0, nBBot As Integer = 0, nCmp As Integer = 0
                For ei As Integer = 0 To fm.Elements.Count - 1
                    Dim e = fm.Elements(ei)
                    If Not e.Name.StartsWith("B") Then Continue For
                    Dim fr = frames(ei)
                    If Not res.ContainsKey(fr) Then Continue For
                    Dim d = defs(e.Section.Name), r = res(fr)
                    Dim h As Double = d.H, dd As Double = h - CoverCentroid
                    'program demands of this combination (same convention as BeamDesign.Demands)
                    Dim top As Double = 0, bot As Double = 0
                    For k As Integer = 0 To cs.Forces.Count - 1
                        Dim f = cs.Forces(k)(ei)
                        Dim mI As Double = -f(5) * 1.0E6, mJ As Double = f(11) * 1.0E6, mm As Double = cs.Mid(k)(ei)(5) * 1.0E6
                        top = Math.Max(top, Math.Max(-mI, -mJ))
                        bot = Math.Max(bot, Math.Max(Math.Max(mI, mJ), mm))
                    Next
                    Dim pTop As Double = If(top > 0, AciBeam.RequiredAs(top, d.B, dd, d.Fc, Fy), 0.0)
                    Dim pBot As Double = If(bot > 0, AciBeam.RequiredAs(bot, d.B, dd, d.Fc, Fy), 0.0)
                    If cracked Then pBot = Math.Max(pBot, 0.5 * pTop)                                  'ACI 18.6.3.2 (and TBDY 7.4.2.3): bottom >= 50 % of top at the joint
                    Dim aMin As Double = AciBeam.MinAs(d.B, dd, d.Fc, Fy)
                    Dim pTopF As Double = Math.Max(pTop, Math.Min(aMin, 4.0 / 3.0 * pTop))        'ACI 9.6.1.3: At least min(As,min ; 4/3 As,req)
                    Dim pBotF As Double = Math.Max(pBot, Math.Min(aMin, 4.0 / 3.0 * pBot))
                    'shear
                    Dim pV As Double
                    Dim ln As Double = (e.Length - e.OffsetI - e.OffsetJ) * 1000.0
                    Dim avMin As Double = AciBeam.AvMinPerS(d.B, d.Fc, Fy)
                    If Not cracked Then
                        Dim f = cs.Forces(0)(ei)
                        Dim vu As Double = Math.Max(Math.Abs(f(1)), Math.Abs(f(7))) * 1000.0
                        Dim vcc As Double = AciBeam.Vc(d.B, dd, d.Fc)
                        pV = If(vu <= PhiS * vcc / 2.0, 0.0, Math.Max((vu / PhiS - vcc) / (Fy * dd), avMin))
                    Else
                        Dim asT As Double = Math.Max(r.TopArea, 1.0), asB As Double = Math.Max(r.BotArea, 1.0)
                        Dim mprT As Double = AciBeam.Flexure(d.B, dd, asT, d.Fc, 1.25 * Fy).Mn
                        Dim mprB As Double = AciBeam.Flexure(d.B, dd, asB, d.Fc, 1.25 * Fy).Mn
                        Dim vp As Double = (mprT + mprB) / ln
                        Dim vg As Double = SimpleShear(fa, ei, gTerms.ToArray())
                        Dim ve As Double = vp + vg
                        pV = Math.Max(ve / PhiS / (Fy * dd), avMin)                                'Vc = 0 within lo of the joint (ACI 18.6.5.2)
                    End If
                    nCmp += 1
                    If r.TopReq > 50 Then minPT = Math.Min(minPT, pTop / r.TopReq) : maxPT = Math.Max(maxPT, pTop / r.TopReq) : If pTop >= r.TopReq * 0.999 Then nBTop += 1
                    If r.BotReq > 50 Then minPB = Math.Min(minPB, pBot / r.BotReq) : maxPB = Math.Max(maxPB, pBot / r.BotReq) : If pBot >= r.BotReq * 0.999 Then nBBot += 1
                    If r.VArea > 1.0E-6 Then minV = Math.Min(minV, pV / r.VArea) : maxV = Math.Max(maxV, pV / r.VArea)
                    If rows.Count < 14 Then rows.Add(e.Name.PadRight(14) & fr.PadRight(6) & " top req ETABS " & r.TopReq.ToString("F0", ci) & " prog " & pTop.ToString("F0", ci) & " (" & Ratio(pTop, r.TopReq) & ")  bot req ETABS " & r.BotReq.ToString("F0", ci) & " prog " & pBot.ToString("F0", ci) & " (" & Ratio(pBot, r.BotReq) & ")  | final top ETABS " & r.TopArea.ToString("F0", ci) & " prog " & pTopF.ToString("F0", ci) & " bot ETABS " & r.BotArea.ToString("F0", ci) & " prog " & pBotF.ToString("F0", ci) & " | Av/s ETABS " & r.VArea.ToString("F4", ci) & " prog " & pV.ToString("F4", ci) & " (" & Ratio(pV, r.VArea) & ")")
                Next
                For Each rw In rows : P(rw) : Next
                P("SUMMARY " & c.Name & ": beams " & nCmp & "; program/ETABS required top As min " & minPT.ToString("F3", ci) & " max " & maxPT.ToString("F3", ci) & " (program >= ETABS in " & nBTop & "), bottom min " & minPB.ToString("F3", ci) & " max " & maxPB.ToString("F3", ci) & " (>= in " & nBBot & "); Av/s program/ETABS min " & minV.ToString("F3", ci) & " max " & maxV.ToString("F3", ci))
            Next
        Catch ex As Exception
            P("EXCEPTION " & ex.GetType().Name & ": " & ex.Message & vbCrLf & ex.StackTrace)
        Finally
            Try : obj.ApplicationExit(False) : Catch : End Try
        End Try
    End Sub
End Module
