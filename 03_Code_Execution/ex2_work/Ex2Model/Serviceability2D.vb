Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports FemCore
Imports FemRcModel
Imports RcCore

'ACI 318M-14 6.6.4.3(b) / 6.6.4.4.1 stability index per storey (second package item 8, 2026-11-16):
'Q = Sum Pu * Delta_o / (Vus * lc). Delta_o / Vus from a unit lateral load (1 kN per floor, split equally over the floor nodes) on a model with the
'6.6.3.1.1(a) stiffnesses (columns 0.70 Ig, beams 0.35 Ig, area 1.0 Ag; A26) as R6.6.4.6.2 states the Q analysis uses them. lc = storey height (axis).
'Sum Pu = sum of the column axial compressions at the bottom of the storey, largest over the design combinations of the gross analysis.
Public Module Stability2D
    Public Const CrackColumn As Double = 0.7
    Public Const CrackBeam As Double = 0.35
    Public Const QLimit As Double = 0.05

    'Q(k), k = 1 .. NStory (index 0 unused)
    Public Function StoreyQ(spec As FrameSpec2D, defs As Dictionary(Of String, RcSectionDef), a As Analysis2D) As Double()
        Dim secs As New Dictionary(Of String, FemSection)
        For Each kv In defs
            secs(kv.Key) = RcFrameBuilder.MakeSection(kv.Key, kv.Value.B, kv.Value.H, kv.Value.Fc, spec.ConcreteUnitWeight, If(kv.Key.StartsWith("C"), CrackColumn, CrackBeam))
        Next
        Dim fm = RcFrameBuilder2D.Build(spec, secs)
        Dim m = fm.Model
        m.Patterns.Clear()
        Dim pH As New FemPattern With {.Name = "H", .SelfWeightMultiplier = 0.0}
        For lev As Integer = 1 To spec.NStory
            For i As Integer = 0 To spec.NCols - 1
                Dim nl As New FemNodalLoad With {.Node = fm.NodeAt(i, lev)}
                nl.F(0) = 1.0 / spec.NCols
                pH.NodalLoads.Add(nl)
            Next
        Next
        m.Patterns.Add(pH)
        Dim fs As New FemStatic(m)
        Dim rr = fs.Solve()
        If rr.Failed Then Throw New InvalidOperationException("Ex2 Q model: " & rr.Message)
        Dim ux(spec.NStory) As Double
        For lev As Integer = 0 To spec.NStory
            Dim s As Double = 0.0
            For i As Integer = 0 To spec.NCols - 1 : s += rr.U(0)(fm.NodeAt(i, lev) * 6) : Next
            ux(lev) = s / spec.NCols
        Next
        Dim q(spec.NStory) As Double
        Dim am = a.Frame.Model
        For k As Integer = 1 To spec.NStory
            Dim vus As Double = spec.NStory - k + 1.0          'kN
            Dim d0 As Double = Math.Abs(ux(k) - ux(k - 1))     'm
            Dim sumP As Double = 0.0
            For ci As Integer = 0 To a.Combos.Count - 1
                If Not a.Combos(ci).Main Then Continue For
                Dim s As Double = 0.0
                For ei As Integer = 0 To am.Elements.Count - 1
                    If a.Frame.IsColumn(ei) AndAlso am.Elements(ei).Story = k Then s += a.Face(ci)(ei)(0)
                Next
                sumP = Math.Max(sumP, s)
            Next
            q(k) = sumP * d0 / (vus * spec.StoryH)
        Next
        Return q
    End Function

    'PCR-22 (2026-11-16; internal Critic 1-b, external reviewer item 1, ETABS V2-a BULGU-1): sway storeys (Q > 0.05) are designed with ACI 6.6.4.6:
    'delta_s = 1 / (1 - Q) >= 1 (6.6.4.6.2(a)), applicable only while delta_s <= 1.5 (Q <= 1/3; otherwise (b)/(c) would be needed -> constraint g7);
    'M2 = M2ns + delta_s M2s (6.6.4.6.1). M2s = moments of the unbraced (gross) frame under the NET storey sway forces H_k = -(sum of the U1 restraint
    'reactions of floor k in a braced analysis with every floor node restrained), H_k spread equally over the floor nodes (as the Q load); M2ns = M - M2s.
    'Net forces (not the individual node reactions) are used so that the symmetric spreading of the floor nodes by beam axial strain is not counted as sway.
    'Run only when at least one storey is sway. For the symmetric frame and loading of Ex2, H_k = 0 and M2s = 0 (reported as MsMaxKNm); the code is general.
    Public Const QMaxMethodA As Double = 1.0 / 3.0

    Public Function Assess(spec As FrameSpec2D, defs As Dictionary(Of String, RcSectionDef), a As Analysis2D) As StoreyStability
        Dim s As New StoreyStability
        s.Q = StoreyQ(spec, defs, a)
        Dim n As Integer = spec.NStory
        s.NonSway = s.Q.Select(Function(q) q <= QLimit).ToArray()
        ReDim s.DeltaS(n) : ReDim s.DriftMm(n) : ReDim s.MsMaxKNm(n)
        For k As Integer = 0 To n
            If s.NonSway(k) Then
                s.DeltaS(k) = 1.0
            Else
                s.DeltaS(k) = If(s.Q(k) >= 1.0, Double.PositiveInfinity, Math.Max(1.0 / (1.0 - s.Q(k)), 1.0))
            End If
        Next
        'gravity storey drift (gross analysis, main combinations): |mean ux(level k) - mean ux(level k-1)| (floor-average translation, as in Q), mm
        Dim m = a.Frame.Model
        Dim np As Integer = m.Patterns.Count
        For Each c In a.Combos
            If Not c.Main Then Continue For
            Dim fac = a.Factors(c)
            Dim avg(n) As Double
            For lev As Integer = 0 To n
                For i As Integer = 0 To spec.NCols - 1
                    For p As Integer = 0 To np - 1
                        If fac(p) <> 0.0 Then avg(lev) += fac(p) * a.R.U(p)(a.Frame.NodeAt(i, lev) * 6) / spec.NCols
                    Next
                Next
            Next
            For k As Integer = 1 To n
                s.DriftMm(k) = Math.Max(s.DriftMm(k), Math.Abs(avg(k) - avg(k - 1)) * 1000.0)
            Next
        Next
        If s.NonSway.Skip(1).Any(Function(b) Not b) Then
            s.Braced = RcCombos2D.Analyze(spec, defs, a.WithVariants, a.Patterned, True)
            Dim bm = s.Braced.Frame.Model
            'unbraced gross frame, one lateral pattern per level (1 kN spread equally over the floor nodes)
            Dim fmS = RcFrameBuilder2D.Build(spec, RcCombos2D.MakeSections(spec, defs))
            fmS.Model.Patterns.Clear()
            For lev As Integer = 1 To n
                Dim pH As New FemPattern With {.Name = "H" & lev, .SelfWeightMultiplier = 0.0}
                For i As Integer = 0 To spec.NCols - 1
                    Dim nl As New FemNodalLoad With {.Node = fmS.NodeAt(i, lev)}
                    nl.F(0) = 1.0 / spec.NCols
                    pH.NodalLoads.Add(nl)
                Next
                fmS.Model.Patterns.Add(pH)
            Next
            Dim rS = New FemStatic(fmS.Model).Solve()
            If rS.Failed Then Throw New InvalidOperationException("Ex2 sway model: " & rS.Message)
            s.StoreyH = New List(Of Double())
            For ci As Integer = 0 To a.Combos.Count - 1
                Dim hk(n) As Double
                Dim sf(m.Elements.Count - 1)() As Double
                If a.Combos(ci).Main Then
                    Dim fac = s.Braced.Factors(a.Combos(ci))
                    For lev As Integer = 1 To n
                        For i As Integer = 0 To spec.NCols - 1
                            For p As Integer = 0 To bm.Patterns.Count - 1
                                If fac(p) <> 0.0 Then hk(lev) -= fac(p) * s.Braced.R.Reaction(p)(fmS.NodeAt(i, lev) * 6)
                            Next
                        Next
                    Next
                End If
                For ei As Integer = 0 To m.Elements.Count - 1
                    sf(ei) = New Double(11) {}
                    For lev As Integer = 1 To n
                        If hk(lev) = 0.0 Then Continue For
                        Dim f = rS.FaceForce(lev - 1)(ei)
                        For q As Integer = 0 To 11 : sf(ei)(q) += hk(lev) * f(q) : Next
                    Next
                    If a.Frame.IsColumn(ei) AndAlso a.Combos(ci).Main Then
                        Dim k As Integer = m.Elements(ei).Story
                        For Each idx In {4, 5, 10, 11} : s.MsMaxKNm(k) = Math.Max(s.MsMaxKNm(k), Math.Abs(sf(ei)(idx))) : Next
                    End If
                Next
                s.SwayFace.Add(sf) : s.StoreyH.Add(hk)
            Next
        End If
        Return s
    End Function
End Module

Public Class StoreyStability
    Public Q As Double()              'ACI 6.6.4.4.1, index 1..N (0 unused)
    Public NonSway As Boolean()       'Q <= 0.05 (ACI 6.6.4.3(b))
    Public DeltaS As Double()         'sway storeys 1 / (1 - Q) (ACI 6.6.4.6.2(a)); non-sway storeys 1
    Public DriftMm As Double()        'gravity storey drift, largest over the column lines and the main combinations (mm; gross analysis)
    Public MsMaxKNm As Double()       'largest |M_s| at the column ends of the storey (kN m); 0 when no storey is sway (not computed)
    Public Braced As Analysis2D       'sway-prevented analysis (every floor node U1 restrained; same combinations); Nothing when every storey is non-sway
    Public SwayFace As New List(Of Double()())   '[combo][element](12): face forces of the sway part (net storey forces on the unbraced frame); empty when Braced is Nothing
    Public StoreyH As List(Of Double())          '[combo][level]: net storey sway force H_k (kN)
End Class

'ACI 318M-14 24.2 calculated deflection REPORT (second package item 9; IA 2026-11-16 servis (a): not a constraint). Read 2026-11-16 (PDF 401-403):
'24.2.3.5 Ie = (Mcr/Ma)^3 Ig + [1 - (Mcr/Ma)^3] Icr <= Ig, Mcr = fr Ig / yt, fr = 0.62 sqrt(f'c) (19.2.3.1); 24.2.3.6 continuous beams: Ie = average of
'the critical positive and negative sections; 24.2.4.1.1 lambda = xi / (1 + 50 rho'), xi = 2.0 (>= 5 years), rho' = 0 taken (compression steel ignored,
'conservative); Table 24.2.2: l/480 (supporting elements likely to be damaged: part of the total deflection after attachment = long-term deflection of the
'sustained load + immediate deflection of the additional live load), l/240 (not likely to be damaged), l/360 (immediate L, floors).
'Assumptions [VARSAYIM]: sustained load = D (SW + G); live load fully transient; all of D is applied before attachment is NOT assumed (whole long-term
'deflection counted, conservative); Ie per beam GROUP from the group's largest service moments; columns gross; plain-concrete stiffness for SFRC beams
'(fibre effect on stiffness ignored); l = axis span; deflection relative to the chord, Hermite end-rotation formula + w l^4 / (384 E Ie).
Public Class DeflectionRow
    Public Element As String, Group As String
    Public SpanM As Double
    Public IeOverIgD, IeOverIgDL As Double
    Public DeltaD, DeltaDL, DeltaL, DeltaAfter As Double      'mm (positive = downward)
    Public LimAfter480, LimTotal240, LimL360 As Double          'mm
End Class

Public Module Deflection2D
    Public Const Xi As Double = 2.0

    Private Function Icr(b As Double, d As Double, As_ As Double, n As Double) As Double
        'singly reinforced cracked transformed section (mm4): b x^2 / 2 = n As (d - x)
        Dim A As Double = b / 2.0, B_ As Double = n * As_, C As Double = -n * As_ * d
        Dim x As Double = (-B_ + Math.Sqrt(B_ * B_ - 4.0 * A * C)) / (2.0 * A)
        Return b * x ^ 3 / 3.0 + n * As_ * (d - x) ^ 2
    End Function

    Public Function Ie(b As Double, h As Double, d As Double, As_ As Double, fc As Double, Ma As Double) As Double
        Dim ig As Double = b * h ^ 3 / 12.0
        Dim mcr As Double = 0.62 * Math.Sqrt(fc) * ig / (h / 2.0)
        If Ma <= mcr Then Return ig
        Dim n As Double = 200000.0 / (4700.0 * Math.Sqrt(fc))
        Dim r3 As Double = (mcr / Ma) ^ 3
        Return Math.Min(ig, r3 * ig + (1.0 - r3) * Icr(b, d, As_, n))
    End Function

    'service analysis with given beam Ie/Ig per group; factors on SW, G, Q
    Private Function Run(spec As FrameSpec2D, d As Ex2Design, ratio As Dictionary(Of String, Double), fSW As Double, fG As Double, fQ As Double, ByRef fm As FrameModel2D) As StaticResults
        Dim secs As New Dictionary(Of String, FemSection)
        For Each kv In d.Columns : secs(kv.Key) = RcFrameBuilder.MakeSection(kv.Key, kv.Value.B, kv.Value.H, kv.Value.Fc, spec.ConcreteUnitWeight, 1.0) : Next
        For Each kv In d.Beams : secs(kv.Key) = RcFrameBuilder.MakeSection(kv.Key, kv.Value.B, kv.Value.H, kv.Value.Fc, spec.ConcreteUnitWeight, If(ratio Is Nothing, 1.0, ratio(kv.Key))) : Next
        fm = RcFrameBuilder2D.Build(spec, secs)
        Return New FemStatic(fm.Model).Solve()
    End Function

    'service moments (kN m): largest hogging and sagging over the group's beam stations for the factors given
    Private Sub ServiceMoments(a As Analysis2D, g As String, fSW As Double, fG As Double, fQ As Double, ByRef neg As Double, ByRef pos As Double)
        Dim m = a.Frame.Model
        Dim pI() As Integer = {m.PatternIndex("SW"), m.PatternIndex("G"), m.PatternIndex("Q")}
        neg = 0.0 : pos = 0.0
        For ei As Integer = 0 To m.Elements.Count - 1
            If m.Elements(ei).Group <> g Then Continue For
            Dim xs = a.StationX(ei)
            Dim s0 = a.Fs.StationForces(a.R, pI(0), ei, xs), s1 = a.Fs.StationForces(a.R, pI(1), ei, xs), s2 = a.Fs.StationForces(a.R, pI(2), ei, xs)
            For j As Integer = 0 To xs.Length - 1
                Dim m3 As Double = fSW * s0(j)(5) + fG * s1(j)(5) + fQ * s2(j)(5)
                neg = Math.Max(neg, -m3) : pos = Math.Max(pos, m3)
            Next
        Next
    End Sub

    'mid-span deflection relative to the chord (m, downward positive) of a horizontal element under a UDL w (kN/m, downward), E I (kN m2)
    Public Function MidspanDeflection(u()() As Double, fm As FrameModel2D, ei As Integer, w As Double, ei_ As Double) As Double
        Dim e = fm.Model.Elements(ei)
        Dim L As Double = e.Length
        Dim thI As Double = -u(0)(e.NI * 6 + 4), thJ As Double = -u(0)(e.NJ * 6 + 4)        'dUz/dx = -Ry (beam along +X)
        Dim vRel As Double = L / 8.0 * (thI - thJ) - w * L ^ 4 / (384.0 * ei_)
        Return -vRel
    End Function

    Public Function Report(spec As FrameSpec2D, d As Ex2Design, res As Ex2Result) As List(Of DeflectionRow)
        Dim a = res.Analysis
        Dim rD As New Dictionary(Of String, Double), rDL As New Dictionary(Of String, Double)
        For Each g In a.Frame.BeamGroups
            Dim br = res.BeamRes(g), bd = d.Beams(g)
            Dim neg As Double, pos As Double
            Dim ig As Double = bd.B * bd.H ^ 3 / 12.0
            ServiceMoments(a, g, 1.0, 1.0, 0.0, neg, pos)
            rD(g) = 0.5 * (Ie(bd.B, bd.H, br.D, br.AsTop, bd.Fc, neg * 1.0E6) + Ie(bd.B, bd.H, br.D, br.AsBot, bd.Fc, pos * 1.0E6)) / ig
            ServiceMoments(a, g, 1.0, 1.0, 1.0, neg, pos)
            rDL(g) = 0.5 * (Ie(bd.B, bd.H, br.D, br.AsTop, bd.Fc, neg * 1.0E6) + Ie(bd.B, bd.H, br.D, br.AsBot, bd.Fc, pos * 1.0E6)) / ig
        Next
        Dim fmD As FrameModel2D = Nothing, fmDL As FrameModel2D = Nothing
        Dim rrD = Run(spec, d, rD, 1, 1, 0, fmD), rrDL = Run(spec, d, rDL, 1, 1, 1, fmDL)
        Dim mD = fmD.Model
        Dim pSW As Integer = mD.PatternIndex("SW"), pG As Integer = mD.PatternIndex("G"), pQ As Integer = mD.PatternIndex("Q")
        Dim rows As New List(Of DeflectionRow)
        For ei As Integer = 0 To mD.Elements.Count - 1
            If fmD.IsColumn(ei) Then Continue For
            Dim e = mD.Elements(ei), bd = d.Beams(e.Group)
            Dim ecm As Double = 4700.0 * Math.Sqrt(bd.Fc) * 1000.0             'kN/m2 (same Ec as RcFrameBuilder.MakeSection)
            Dim ig As Double = bd.B * bd.H ^ 3 / 12.0 * 1.0E-12                'm4
            Dim wSW As Double = spec.ConcreteUnitWeight * bd.B * bd.H * 1.0E-6
            Dim uD()() As Double = {Combine(rrD.U, pSW, pG, -1, mD)}
            Dim uDL()() As Double = {Combine(rrDL.U, pSW, pG, pQ, fmDL.Model)}
            Dim dD As Double = MidspanDeflection(uD, fmD, ei, wSW + spec.DeadLine, ecm * ig * rD(e.Group))
            Dim dDL As Double = MidspanDeflection(uDL, fmDL, ei, wSW + spec.DeadLine + spec.LiveLine, ecm * ig * rDL(e.Group))
            Dim row As New DeflectionRow With {.Element = e.Name, .Group = e.Group, .SpanM = e.Length, .IeOverIgD = rD(e.Group), .IeOverIgDL = rDL(e.Group)}
            row.DeltaD = dD * 1000.0 : row.DeltaDL = dDL * 1000.0 : row.DeltaL = row.DeltaDL - row.DeltaD
            row.DeltaAfter = Xi * row.DeltaD + row.DeltaL                      'lambda = xi / (1 + 50 rho'), rho' = 0
            row.LimAfter480 = e.Length * 1000.0 / 480.0 : row.LimTotal240 = e.Length * 1000.0 / 240.0 : row.LimL360 = e.Length * 1000.0 / 360.0
            rows.Add(row)
        Next
        Return rows
    End Function

    'PCR-26b (IA 2026-11-19, "Ie env"): ACI 24.2 deflection under the ACI 6.4.2 live-load patterns (all spans + RcCombos2D.PatternCombos) with ONE Ie/Ig per beam
    'group = the smallest Ie/Ig of the group over all load cases (envelope variant of ex2_analysis2_extra.ps1 DeflPat; conservative, >= the Report quantity).
    'Live-load patterns by superposition of per-element unit patterns (one extra factorisation). Returns, per beam element (model order), the worst
    'dAfter / (l/480) over all cases; dAfter = 2 dD + dL (dD: SW + G with the D-state Ie).
    Public Function PatternedEnvelope(spec As FrameSpec2D, d As Ex2Design, res As Ex2Result) As List(Of Double)
        Dim a = res.Analysis, m = a.Frame.Model
        Dim beamEls As New List(Of Integer)
        For ei As Integer = 0 To m.Elements.Count - 1
            If Not a.Frame.IsColumn(ei) Then beamEls.Add(ei)
        Next
        Dim pSW As Integer = m.PatternIndex("SW"), pG As Integer = m.PatternIndex("G")
        Dim mBase As New Dictionary(Of Integer, Double()), mQ As New Dictionary(Of Long, Double())
        For Each ei In beamEls
            Dim xs = a.StationX(ei)
            Dim s0 = a.Fs.StationForces(a.R, pSW, ei, xs), s1 = a.Fs.StationForces(a.R, pG, ei, xs)
            Dim b(xs.Length - 1) As Double
            For j As Integer = 0 To xs.Length - 1 : b(j) = s0(j)(5) + s1(j)(5) : Next
            mBase(ei) = b
            For Each e2 In beamEls
                Dim sq = a.Fs.StationForces(a.R, m.PatternIndex("Q_E" & e2), ei, xs)
                Dim q(xs.Length - 1) As Double
                For j As Integer = 0 To xs.Length - 1 : q(j) = sq(j)(5) : Next
                mQ(CLng(ei) * 100000L + e2) = q
            Next
        Next
        Dim cases As New List(Of List(Of Integer))
        cases.Add(New List(Of Integer)(beamEls))
        For Each c In RcCombos2D.PatternCombos(a.Frame) : cases.Add(New List(Of Integer)(c.QOn)) : Next
        Dim ratioOf = Function(g As String, qon As List(Of Integer)) As Double
                          Dim bd = d.Beams(g), br = res.BeamRes(g)
                          Dim neg As Double = 0.0, pos As Double = 0.0
                          For Each ei In beamEls
                              If m.Elements(ei).Group <> g Then Continue For
                              Dim b = mBase(ei)
                              For j As Integer = 0 To b.Length - 1
                                  Dim m3 As Double = b(j)
                                  For Each e2 In qon : m3 += mQ(CLng(ei) * 100000L + e2)(j) : Next
                                  neg = Math.Max(neg, -m3) : pos = Math.Max(pos, m3)
                              Next
                          Next
                          Dim igL As Double = bd.B * bd.H ^ 3 / 12.0
                          Return 0.5 * (Ie(bd.B, bd.H, br.D, br.AsTop, bd.Fc, neg * 1.0E6) + Ie(bd.B, bd.H, br.D, br.AsBot, bd.Fc, pos * 1.0E6)) / igL
                      End Function
        Dim rD As New Dictionary(Of String, Double), rEnv As New Dictionary(Of String, Double)
        For Each g In a.Frame.BeamGroups
            rD(g) = ratioOf(g, New List(Of Integer))
            Dim mn As Double = Double.MaxValue
            For Each cs In cases : mn = Math.Min(mn, ratioOf(g, cs)) : Next
            rEnv(g) = mn
        Next
        Dim fmD As FrameModel2D = Nothing
        Dim rrD = Run(spec, d, rD, 1, 1, 0, fmD)
        Dim uD()() As Double = {Combine(rrD.U, fmD.Model.PatternIndex("SW"), fmD.Model.PatternIndex("G"), -1, fmD.Model)}
        'envelope-Ie model with one unit live pattern per beam element
        Dim secs As New Dictionary(Of String, FemSection)
        For Each kv In d.Columns : secs(kv.Key) = RcFrameBuilder.MakeSection(kv.Key, kv.Value.B, kv.Value.H, kv.Value.Fc, spec.ConcreteUnitWeight, 1.0) : Next
        For Each kv In d.Beams : secs(kv.Key) = RcFrameBuilder.MakeSection(kv.Key, kv.Value.B, kv.Value.H, kv.Value.Fc, spec.ConcreteUnitWeight, rEnv(kv.Key)) : Next
        Dim fmE = RcFrameBuilder2D.Build(spec, secs)
        For Each ei In beamEls
            Dim pq As New FemPattern With {.Name = "QD_E" & ei, .SelfWeightMultiplier = 0.0}
            pq.DistLoads.Add(New FemDistLoad With {.Element = ei, .Dir = 10, .IsGlobal = True, .Relative = True, .D1 = 0, .D2 = 1, .V1 = spec.LiveLine, .V2 = spec.LiveLine})
            fmE.Model.Patterns.Add(pq)
        Next
        Dim rrE = New FemStatic(fmE.Model).Solve()
        If rrE.Failed Then Throw New InvalidOperationException("deflection envelope model failed")
        Dim iSW As Integer = fmE.Model.PatternIndex("SW"), iG As Integer = fmE.Model.PatternIndex("G")
        Dim worst As New Dictionary(Of Integer, Double)
        For Each ei In beamEls : worst(ei) = Double.NegativeInfinity : Next
        For Each cs In cases
            Dim n As Integer = rrE.U(iSW).Length
            Dim u(n - 1) As Double
            For k As Integer = 0 To n - 1 : u(k) = rrE.U(iSW)(k) + rrE.U(iG)(k) : Next
            For Each e2 In cs
                Dim uq = rrE.U(fmE.Model.PatternIndex("QD_E" & e2))
                For k As Integer = 0 To n - 1 : u(k) += uq(k) : Next
            Next
            Dim uL()() As Double = {u}
            For Each ei In beamEls
                Dim e = m.Elements(ei), bd = d.Beams(e.Group)
                Dim ecm As Double = 4700.0 * Math.Sqrt(bd.Fc) * 1000.0
                Dim igE As Double = bd.B * bd.H ^ 3 / 12.0 * 1.0E-12
                Dim wSW As Double = spec.ConcreteUnitWeight * bd.B * bd.H * 1.0E-6
                Dim dD As Double = MidspanDeflection(uD, fmD, ei, wSW + spec.DeadLine, ecm * igE * rD(e.Group)) * 1000.0
                Dim dDL As Double = MidspanDeflection(uL, fmE, ei, wSW + spec.DeadLine + If(cs.Contains(ei), spec.LiveLine, 0.0), ecm * igE * rEnv(e.Group)) * 1000.0
                Dim dA As Double = Xi * dD + (dDL - dD)
                worst(ei) = Math.Max(worst(ei), dA / (e.Length * 1000.0 / 480.0))
            Next
        Next
        Return beamEls.Select(Function(ei) worst(ei)).ToList()
    End Function

    'sum of pattern displacement vectors (patterns SW, G and optionally Q, factor 1)
    Private Function Combine(u()() As Double, iSW As Integer, iG As Integer, iQ As Integer, m As FemModel) As Double()
        Dim n As Integer = u(iSW).Length
        Dim r(n - 1) As Double
        For k As Integer = 0 To n - 1
            r(k) = u(iSW)(k) + u(iG)(k) + If(iQ >= 0, u(iQ)(k), 0.0)
        Next
        Return r
    End Function
End Module
