Imports System
Imports System.Collections.Generic
Imports FemCore
Imports RcCore

'Global TBDY-2018 checks of the frame: storey drift (4.9.1, Eq. 4.33/4.34a) and strong-column weak-beam (7.3.5, Eq. 7.3).
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

    'Strong column - weak beam at every joint below the roof, both frame directions (Eq. 7.3: sum Mr,col >= 1.2 sum Mr,beam).
    'Mr of the columns: TS 500 strengths at the smallest compression Nd of the seismic combinations (TBDY 7.3.5.1); Mr of the beams from the provided steel.
    'Beam side: both beams: Mr,top + Mr,bottom (one hogging, one sagging); one beam: larger of the two. Exemption: Nd <= 0.10 Ac fck in both columns (7.3.5 a).
    Public Function StrongColumn(fa As FrameAnalysis, spec As FrameSpec, cols As Dictionary(Of String, ColumnGroupResult), beams As Dictionary(Of String, BeamGroupResult), fib As BeamFibre) As List(Of JointResult)
        Dim res As New List(Of JointResult)
        Dim m = fa.Gross
        Dim mm = m.Model
        For k As Integer = 1 To spec.NStory - 1
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
                            Dim e = mm.Elements(ci)
                            Dim cg = cols(e.Group)
                            Dim ndMin As Double = Double.MaxValue, ndMax As Double = 0.0
                            For Each cs In fa.Combos
                                If Not cs.Seismic Then Continue For
                                For q As Integer = 0 To cs.Forces.Count - 1
                                    Dim f = cs.Forces(q)(ci)
                                    Dim pu As Double = If(e.NJ = node, -f(6), f(0)) * 1000.0
                                    ndMin = Math.Min(ndMin, Math.Max(pu, 0.0)) : ndMax = Math.Max(ndMax, pu)
                                Next
                            Next
                            If ndMax > 0.1 * cg.B * cg.H * cg.Fc Then exemptA = False      'exemption only if even the largest Nd is small (conservative reading of 7.3.5 a)
                            Dim sec = If(dir = 0, ColumnDesign.MakeSection(cg.B, cg.H, cg.Fc, cg.NBars, cg.BarDia, True), ColumnDesign.MakeSection(cg.H, cg.B, cg.Fc, cg.NBars, cg.BarDia, True))
                            jr.SumMrColumns += ColumnShear.MrAtP(sec, ndMin)
                        Next
                        Dim mrs As New List(Of Double)
                        For ei As Integer = 0 To mm.Elements.Count - 1
                            Dim e = mm.Elements(ei)
                            If Math.Abs(e.Axes(0, 2)) > 0.9 OrElse e.Story <> k Then Continue For
                            If e.NI <> node AndAlso e.NJ <> node Then Continue For
                            Dim alongX As Boolean = Math.Abs(mm.Nodes(e.NJ).X - mm.Nodes(e.NI).X) > 0.001
                            If alongX <> (dir = 0) Then Continue For
                            Dim br = beams(e.Group)
                            Dim fcd As Double = Ts500.Fcd(br.Fc), fyd As Double = Ts500.Fyd(420.0)
                            Dim mrT As Double = AciBeam.Flexure(br.B, br.D, br.AsTop, fcd, fyd).Mn
                            Dim mrB As Double = AciBeam.Flexure(br.B, br.D, br.AsBot, fcd, fyd).Mn
                            mrs.Add(mrT) : mrs.Add(mrB)
                        Next
                        If mrs.Count = 4 Then
                            jr.SumMrBeams = Math.Max(mrs(0) + mrs(3), mrs(1) + mrs(2))
                        ElseIf mrs.Count = 2 Then
                            jr.SumMrBeams = Math.Max(mrs(0), mrs(1))
                        End If
                        jr.Exempt = exemptA
                        jr.G = If(exemptA OrElse jr.SumMrColumns <= 0.0, 0.0, 1.2 * jr.SumMrBeams / jr.SumMrColumns - 1.0)
                        If jr.SumMrColumns <= 0.0 AndAlso Not exemptA Then jr.G = 1.0E6
                        res.Add(jr)
                    Next
                Next
            Next
        Next
        Return res
    End Function
End Module

'Beam-column joint shear (TBDY-2018 7.5, Eq. 7.11-7.13; extraction G9 + A11 C1f).
Public Class JointShearResult
    Public Name As String
    Public Ve, Limit As Double          'N
    Public Confined As Boolean
    Public G As Double                  'Ve / limit - 1
End Class

Public Module JointShear
    'Vkol = min(Va, Vu) of the columns below and above the joint: ASSUMPTION: the capacity-design shears Ve of 7.3.7 (ColumnShear); at the roof the column above is missing -> Vkol = 0 (conservative).
    Public Function Check(fa As FrameAnalysis, spec As FrameSpec, cols As Dictionary(Of String, ColumnGroupResult), colShear As Dictionary(Of String, ColumnShearResult), beams As Dictionary(Of String, BeamGroupResult)) As List(Of JointShearResult)
        Dim res As New List(Of JointShearResult)
        Dim m = fa.Gross
        Dim mm = m.Model
        For k As Integer = 1 To spec.NStory
            For j As Integer = 0 To spec.NyBays
                For i As Integer = 0 To spec.NxBays
                    Dim node As Integer = m.NodeAt(i, j, k)
                    For dir As Integer = 0 To 1
                        Dim r As New JointShearResult With {.Name = "J" & i & j & k & If(dir = 0, "x", "y")}
                        Dim vBelow As Double = 0.0, vAbove As Double = 0.0
                        Dim cg As ColumnGroupResult = Nothing
                        For ei As Integer = 0 To mm.Elements.Count - 1
                            Dim e = mm.Elements(ei)
                            If Math.Abs(e.Axes(0, 2)) <= 0.9 Then Continue For
                            If e.NJ = node Then cg = cols(e.Group) : vBelow = colShear(e.Group).Ve(dir)
                            If e.NI = node Then vAbove = colShear(e.Group).Ve(dir)
                        Next
                        Dim vkol As Double = Math.Min(vBelow, vAbove)
                        Dim tops As New List(Of Double), bots As New List(Of Double), bws As New List(Of Double)
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
                        Next
                        Dim as12 As Double
                        If tops.Count = 2 Then as12 = Math.Max(tops(0) + bots(1), tops(1) + bots(0)) Else If tops.Count = 1 Then as12 = Math.Max(tops(0), bots(0))
                        r.Ve = Math.Max(1.25 * 420.0 * as12 - vkol, 0.0)
                        'column dimensions: h along the beam direction (X: depth H, Y: width B), b perpendicular
                        Dim hC As Double = If(dir = 0, cg.H, cg.B), bC As Double = If(dir = 0, cg.B, cg.H)
                        Dim bwMax As Double = 0.0, bwMin As Double = Double.MaxValue
                        For Each w In bws : bwMax = Math.Max(bwMax, w) : bwMin = Math.Min(bwMin, w) : Next
                        r.Confined = (nAllSides = 4) AndAlso bwMin >= 0.75 * bC
                        Dim bj As Double = If(bwMax >= bC / 2.0, bC, 2.0 * Math.Min((bC - bwMax) / 2.0, (bC - bwMax) / 2.0))
                        bj = Math.Min(bj, bwMax + hC)
                        r.Limit = If(r.Confined, 1.7, 1.0) * bj * hC * Math.Sqrt(cg.Fc)
                        r.G = If(r.Limit > 0.0, r.Ve / r.Limit - 1.0, 1.0E6)
                        res.Add(r)
                    Next
                Next
            Next
        Next
        Return res
    End Function
End Module
