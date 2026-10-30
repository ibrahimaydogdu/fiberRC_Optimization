Imports System
Imports System.Collections.Generic
Imports FemCore
Imports RcCore

'Global checks of the frame: storey drift (TBDY-2018 4.9.1, Eq. 4.33/4.34a) and strong-column weak-beam (ACI 318M-14 18.7.3.2, Mnc >= 6/5 Mnb; decision IA 2026-10-30).
Public Class DriftResult
    Public MaxRatio As Double          'lambda * delta / h, worst storey and direction
    Public Lambda As Double
    Public T1 As Double
    Public Story, Dir As Integer
    Public G As Double                 'ratio / (0.008 kappa) - 1
End Class

Public Class JointResult
    Public Name As String
    Public SumMrColumns, SumMrBeams As Double    'N.mm
    Public Exempt As Boolean
    Public G As Double                           '1.2 SumMrBeams / SumMrColumns - 1
End Class

Public Module Checks
    'delta_i = (R/I) (u_i - u_{i-1}) from the reduced response spectrum displacements (magnitudes: difference of magnitudes);
    'lambda = Sae(T1, DD-3) / Sae(T1, DD-2); limit 0.008 kappa (kappa = 1, infill walls adjacent: Eq. 4.34a)
    Public Function Drift(fa As FrameAnalysis, spec As FrameSpec, dd2 As TbdyParams, dd3 As TbdyParams) As DriftResult
        Dim r As New DriftResult
        r.T1 = fa.Modal.Period(0)
        r.Lambda = Tbdy.Sae(dd3, r.T1) / Tbdy.Sae(dd2, r.T1)
        Dim m = fa.Cracked
        For d As Integer = 0 To 1
            Dim rr = If(d = 0, fa.RSx, fa.RSy)
            For k As Integer = 1 To spec.NStory
                Dim worst As Double = 0.0
                For j As Integer = 0 To spec.NyBays
                    For i As Integer = 0 To spec.NxBays
                        Dim up As Double = Math.Abs(rr.U(m.NodeAt(i, j, k) * 6 + d))
                        Dim lo As Double = Math.Abs(rr.U(m.NodeAt(i, j, k - 1) * 6 + d))
                        worst = Math.Max(worst, up - lo)
                    Next
                Next
                Dim ratio As Double = r.Lambda * (dd2.R / dd2.I) * worst / spec.StoryH
                If ratio > r.MaxRatio Then r.MaxRatio = ratio : r.Story = k : r.Dir = d
            Next
        Next
        r.G = r.MaxRatio / 0.008 - 1.0
        Return r
    End Function

    'Strong column - weak beam at every joint below the roof, both frame directions (ACI 18.7.3.2: sum Mnc >= 6/5 sum Mnb).
    'Mnc: nominal strengths (f'c, fy, phi = 1) at the smallest axial compression of the seismic combinations (the direction of the lateral force giving the lowest strength); Mnb: nominal strength of the beams from the provided steel.
    'Beam side: both beams: Mn,top + Mn,bottom (one hogging, one sagging); one beam: larger of the two. No exemption (ACI has none; 18.7.3.3 not used).
    Public Function StrongColumn(fa As FrameAnalysis, spec As FrameSpec, cols As Dictionary(Of String, ColumnGroupResult), beams As Dictionary(Of String, BeamGroupResult), fibs As Dictionary(Of String, BeamFibre)) As List(Of JointResult)
        Dim res As New List(Of JointResult)
        Dim m = fa.Gross
        Dim mm = m.Model
        For k As Integer = 1 To spec.NStory
            For j As Integer = 0 To spec.NyBays
                For i As Integer = 0 To spec.NxBays
                    Dim node As Integer = m.NodeAt(i, j, k)
                    For dir As Integer = 0 To 1
                        Dim jr As New JointResult With {.Name = "J" & i & j & k & If(dir = 0, "x", "y")}
                        Dim below As Integer = -1, above As Integer = -1
                        For ei As Integer = 0 To mm.Elements.Count - 1
                            Dim e = mm.Elements(ei)
                            If Math.Abs(e.Axes(0, 2)) <= 0.9 Then Continue For
                            If e.NJ = node Then below = ei
                            If e.NI = node Then above = ei
                        Next
                        Dim exemptA As Boolean = True
                        For Each ci In {below, above}
                            If ci < 0 Then Continue For                  'roof joint: only the column below
                            Dim e = mm.Elements(ci)
                            Dim cg = cols(e.Group)
                            Dim ndMin As Double = Double.MaxValue, ndMax As Double = 0.0
                            For Each cs In fa.Combos
                                If Not cs.Seismic Then Continue For
                                For q As Integer = 0 To cs.Forces.Count - 1
                                    Dim f = cs.Forces(q)(ci)
                                    Dim pu As Double = If(e.NJ = node, -f(6), f(0)) * 1000.0
                                    ndMin = Math.Min(ndMin, pu) : ndMax = Math.Max(ndMax, pu)
                                Next
                            Next
                            If ndMax > 0.1 * cg.B * cg.H * cg.Fc Then exemptA = False      'exemption only if even the largest Nd is small (conservative reading of 7.3.5 a)
                            Dim sec = If(dir = 0, ColumnDesign.MakeSection(cg.B, cg.H, cg.Fc, cg.NBars, cg.BarDia, False), ColumnDesign.MakeSection(cg.H, cg.B, cg.Fc, cg.NBars, cg.BarDia, False, True))
                            'lowest flexural strength over the range of factored axial forces (18.7.3.2): the minimum of the two ends of the range (Mn(P) is concave)
                            jr.SumMrColumns += Math.Min(ColumnShear.MrAtP(sec, ndMin), ColumnShear.MrAtP(sec, ndMax))
                        Next
                        Dim mrs As New List(Of Double)
                        For ei As Integer = 0 To mm.Elements.Count - 1
                            Dim e = mm.Elements(ei)
                            If Math.Abs(e.Axes(0, 2)) > 0.9 OrElse e.Story <> k Then Continue For
                            If e.NI <> node AndAlso e.NJ <> node Then Continue For
                            Dim alongX As Boolean = Math.Abs(mm.Nodes(e.NJ).X - mm.Nodes(e.NI).X) > 0.001
                            If alongX <> (dir = 0) Then Continue For
                            Dim br = beams(e.Group)
                            Dim mrT As Double, mrB As Double                                    'nominal strengths f'c, fy (fibre part: gamma_F = 1)
                            If fibs IsNot Nothing AndAlso fibs.ContainsKey(e.Group) AndAlso fibs(e.Group) IsNot Nothing AndAlso fibs(e.Group).Params IsNot Nothing Then
                                Dim fp0 = fibs(e.Group).Params
                                Dim q As New SfrcParams With {.FR1 = fp0.FR1, .FR3 = fp0.FR3, .GammaF = 1.0, .Linear = fp0.Linear, .EpsFu = fp0.EpsFu, .WMaxLimit = fp0.WMaxLimit, .Cmod3 = fp0.Cmod3, .WuOverride = fp0.WuOverride}
                                mrT = SfrcBeam.Flexure(br.B, br.H, br.D, br.AsTop, br.Fc, 420.0, q, 0.7 * br.H).Mn
                                mrB = SfrcBeam.Flexure(br.B, br.H, br.D, br.AsBot, br.Fc, 420.0, q, 0.7 * br.H).Mn
                            Else
                                mrT = AciBeam.Flexure(br.B, br.D, br.AsTop, br.Fc, 420.0).Mn
                                mrB = AciBeam.Flexure(br.B, br.D, br.AsBot, br.Fc, 420.0).Mn
                            End If
                            mrs.Add(mrT) : mrs.Add(mrB)
                        Next
                        If mrs.Count = 4 Then
                            jr.SumMrBeams = Math.Max(mrs(0) + mrs(3), mrs(1) + mrs(2))
                        ElseIf mrs.Count = 2 Then
                            jr.SumMrBeams = Math.Max(mrs(0), mrs(1))
                        End If
                        jr.Exempt = exemptA
                        jr.Exempt = False
                        jr.G = If(jr.SumMrColumns <= 0.0, 1.0E6, 1.2 * jr.SumMrBeams / jr.SumMrColumns - 1.0)
                        res.Add(jr)
                    Next
                Next
            Next
        Next
        Return res
    End Function
End Module

'Beam-column joint shear, ACI 318M-14 18.8.2.1 / 18.8.4 (decision IA 2026-10-30): Vu = 1.25 fy (As1 + As2) - Vcol, phi Vn with Vn = 1.7 / 1.2 / 1.0 sqrt(f'c) Aj (Table 18.8.4.1), phi = 0.85 (21.2.1).
Public Class JointShearResult
    Public Name As String
    Public Ve, Limit As Double          'N (Limit = phi Vn)
    Public Confined As Boolean
    Public G As Double                  'Vu / (phi Vn) - 1
End Class

Public Module JointShear
    'Column shear Vcol = (Mpr,L + Mpr,R) / H (ETABS CFD ACI 318-14 3.6.1, inflection points at mid-height of the columns above and below); at the roof the column above is missing -> Vcol = 0 (ETABS: shear from the top column taken as zero).
    Public Function Check(fa As FrameAnalysis, spec As FrameSpec, cols As Dictionary(Of String, ColumnGroupResult), colShear As Dictionary(Of String, ColumnShearResult), beams As Dictionary(Of String, BeamGroupResult), fibs As Dictionary(Of String, BeamFibre)) As List(Of JointShearResult)
        Dim res As New List(Of JointShearResult)
        Dim m = fa.Gross
        Dim mm = m.Model
        For k As Integer = 1 To spec.NStory
            For j As Integer = 0 To spec.NyBays
                For i As Integer = 0 To spec.NxBays
                    Dim node As Integer = m.NodeAt(i, j, k)
                    For dir As Integer = 0 To 1
                        Dim r As New JointShearResult With {.Name = "J" & i & j & k & If(dir = 0, "x", "y")}
                        Dim hasAbove As Boolean = False
                        Dim cg As ColumnGroupResult = Nothing
                        For ei As Integer = 0 To mm.Elements.Count - 1
                            Dim e = mm.Elements(ei)
                            If Math.Abs(e.Axes(0, 2)) <= 0.9 Then Continue For
                            If e.NJ = node Then cg = cols(e.Group)
                            If e.NI = node Then hasAbove = True
                        Next
                        Dim tops As New List(Of Double), bots As New List(Of Double), bws As New List(Of Double), mTop As New List(Of Double), mBot As New List(Of Double)
                        Dim nAllSides As Integer = 0
                        For ei As Integer = 0 To mm.Elements.Count - 1
                            Dim e = mm.Elements(ei)
                            If Math.Abs(e.Axes(0, 2)) > 0.9 OrElse e.Story <> k Then Continue For
                            If e.NI <> node AndAlso e.NJ <> node Then Continue For
                            nAllSides += 1
                            Dim alongX As Boolean = Math.Abs(mm.Nodes(e.NJ).X - mm.Nodes(e.NI).X) > 0.001
                            If alongX <> (dir = 0) Then Continue For
                            Dim br = beams(e.Group)
                            tops.Add(br.AsTop) : bots.Add(br.AsBot) : bws.Add(br.B)
                            Dim fb As BeamFibre = If(fibs IsNot Nothing AndAlso fibs.ContainsKey(e.Group), fibs(e.Group), Nothing)
                            mTop.Add(BeamShear.MrOf(br, br.AsTop, fb)) : mBot.Add(BeamShear.MrOf(br, br.AsBot, fb))
                        Next
                        Dim as12 As Double, mpr12 As Double
                        If tops.Count = 2 Then
                            Dim p1 As Double = tops(0) + bots(1), p2 As Double = tops(1) + bots(0)
                            as12 = Math.Max(p1, p2)
                            mpr12 = If(p1 >= p2, mTop(0) + mBot(1), mTop(1) + mBot(0))
                        ElseIf tops.Count = 1 Then
                            as12 = Math.Max(tops(0), bots(0))
                            mpr12 = Math.Max(mTop(0), mBot(0))
                        End If
                        Dim vkol As Double = If(hasAbove, mpr12 / (spec.StoryH * 1000.0), 0.0)
                        r.Ve = Math.Max(1.25 * 420.0 * as12 - vkol, 0.0)
                        'column dimensions: h along the beam direction (X: depth H, Y: width B), b perpendicular
                        Dim hC As Double = If(dir = 0, cg.H, cg.B), bC As Double = If(dir = 0, cg.B, cg.H)
                        Dim bwMax As Double = 0.0, bwMin As Double = Double.MaxValue
                        For Each w In bws : bwMax = Math.Max(bwMax, w) : bwMin = Math.Min(bwMin, w) : Next
                        'effective joint width 18.8.4.3: column width, not more than beam width + joint depth (centred beams: the second limit equals the column width)
                        Dim bj As Double = Math.Min(bC, bwMax + hC)
                        'confinement of the joint faces 18.8.4.2: a face is confined when the beam width >= 3/4 of the effective joint width
                        Dim faceCountX As Integer = 0, faceCountY As Integer = 0
                        For ei As Integer = 0 To mm.Elements.Count - 1
                            Dim e2 = mm.Elements(ei)
                            If Math.Abs(e2.Axes(0, 2)) > 0.9 OrElse e2.Story <> k Then Continue For
                            If e2.NI <> node AndAlso e2.NJ <> node Then Continue For
                            Dim bw2 As Double = beams(e2.Group).B
                            Dim alongX2 As Boolean = Math.Abs(mm.Nodes(e2.NJ).X - mm.Nodes(e2.NI).X) > 0.001
                            If alongX2 Then
                                If bw2 >= 0.75 * cg.B Then faceCountX += 1
                            Else
                                If bw2 >= 0.75 * cg.H Then faceCountY += 1
                            End If
                        Next
                        Dim nConf As Integer = faceCountX + faceCountY
                        r.Confined = nConf = 4
                        Dim coef As Double = If(nConf = 4, 1.7, If(nConf = 3 OrElse faceCountX = 2 OrElse faceCountY = 2, 1.2, 1.0))
                        r.Limit = 0.85 * coef * Math.Sqrt(cg.Fc) * bj * hC
                        r.G = If(r.Limit > 0.0, r.Ve / r.Limit - 1.0, 1.0E6)
                        res.Add(r)
                    Next
                Next
            Next
        Next
        Return res
    End Function
End Module


'ACI 318M-14 6.6.4.3 stability index per storey and frame direction: Q = sum(Pu) Delta_o / (Vus lc) <= 0.05 (second-order sway effects may be ignored).
'Pu: TBDY Eq. 4.11 gravity part (1 + ez) G + Q + 0.2 S of the columns of the storey; Delta_o: elastic storey drift at the corner of the response-spectrum case, amplified by D of the deflection amplification
'(conservative reading); Vus: CQC storey shear.
Public Module Stability
    Public Function MaxQ(fa As FrameAnalysis, spec As FrameSpec, dd2 As TbdyParams) As Double
        Dim mc = fa.Cracked.Model
        Dim pg = mc.PatternIndex("G"), pq = mc.PatternIndex("Q"), ps = mc.PatternIndex("S")
        Dim worst As Double = 0.0
        For d As Integer = 0 To 1
            Dim rr = If(d = 0, fa.RSx, fa.RSy)
            For k As Integer = 1 To spec.NStory
                Dim zb As Double = (k - 1) * spec.StoryH
                Dim sumP As Double = 0.0
                For ei As Integer = 0 To mc.Elements.Count - 1
                    Dim el = mc.Elements(ei)
                    If Math.Abs(el.Axes(0, 2)) <= 0.9 Then Continue For
                    If Math.Abs(mc.Nodes(el.NI).Z - zb) > 0.01 Then Continue For
                    Dim f As Double = (1.0 + fa.Ez) * fa.Rc.FaceForce(pg)(ei)(0) + fa.Rc.FaceForce(pq)(ei)(0) + 0.2 * fa.Rc.FaceForce(ps)(ei)(0)
                    sumP += Math.Abs(f)
                Next
                Dim n1 As Integer = fa.Cracked.NodeAt(0, 0, k)
                Dim n0 As Integer = If(k = 1, -1, fa.Cracked.NodeAt(0, 0, k - 1))
                Dim u1 As Double = Math.Abs(rr.U(n1 * 6 + d)), u0 As Double = If(n0 < 0, 0.0, Math.Abs(rr.U(n0 * 6 + d)))
                Dim vus As Double = rr.StoryShear(k - 1)
                If vus > 0.0 Then worst = Math.Max(worst, sumP * dd2.D * (u1 - u0) / (vus * spec.StoryH))
            Next
        Next
        Return worst
    End Function
End Module
