Imports System
Imports System.Collections.Generic
Imports FemCore
Imports RcCore

'TS 500 design strengths for the TBDY capacity-design moments Mr (TBDY page 131 / printed 110: Mri, Mra, Mru "fcd ve fyd'ye gore hesaplanan tasima gucu momenti"; no phi factor).
'fcd = fck/1.5, fyd = fyk/1.15, k1 = 0.85 - 0.006 (fck - 25) in [0.70, 0.85]: TS 500 text is NOT in the project (BULUNAMADI) -> values come from the project owner's note, UNVERIFIED.
Public Module Ts500
    Public Function Fcd(fck As Double) As Double
        Return fck / 1.5
    End Function
    Public Function Fyd(fyk As Double) As Double
        Return fyk / 1.15
    End Function
    Public Function K1(fck As Double) As Double
        Return Math.Min(0.85, Math.Max(0.7, 0.85 - 0.006 * (fck - 25.0)))
    End Function
End Module

'Column check from the combination envelope: biaxial P-M demand/capacity (RcCore.AciColumn.BiaxialDcr, linear sum), TBDY A1-A3 limits.
'Local axes of a vertical member: local 2 = global X (section depth H), local 3 = global Y (width B). M3 bends in the X-Z plane (resisted by depth H), M2 in the Y-Z plane (resisted by depth B).
'Compression positive: I end Pu = f(0), J end Pu = -f(6) (forces of the joints on the member ends).
Public Class ColumnGroupResult
    Public Group As String
    Public B, H, Fc, Rho As Double
    Public NBars As Integer, BarDia As Double
    Public AsTotal As Double
    Public DcrMax As Double                  'biaxial D/C (linear sum), > 1 = insufficient
    Public NdMax As Double                   'largest compression, N
    Public NdMin As Double                   'smallest axial compression of the seismic combinations (negative = tension), N
    Public G_Axial As Double                 'A2: Ndm/(0.40 fck Ac) - 1
    Public G_MinDim As Double                'A1: 300/min(b,h) - 1
    Public G_RhoLow, G_RhoHigh As Double     'A3
    Public Governing As String
End Class

Public Module ColumnDesign
    Public Const Fy As Double = 420.0
    Public Const Cover As Double = 60.0          'mm to bar centroid: ACI 318M-14 Table 20.6.1.3.1 cover 40 + tie 10 + half of 20 mm bar (PDF 337)
    Public Const BarSpacing As Double = 150.0    'mm, intermediate rows for the side bars [ASSUMPTION; TBDY 7.3.4 sets tie hook spacing]

    'Bars along the two side dimensions of one physical column: N bars (even >= 4) evenly spaced on the perimeter, 2 (nShort + nLong) - 4 = N, the shorter side gets the smaller count.
    'Both bending directions of one column use this same bar pattern (V2e review A2: before, the two directions used independent layer layouts for the same N).
    Public Sub PerimeterCounts(sideA As Double, sideB As Double, N As Integer, ByRef nA As Integer, ByRef nB As Integer)
        Dim s As Integer = N \ 2 + 2
        Dim sh As Double = Math.Min(sideA, sideB), lg As Double = Math.Max(sideA, sideB)
        Dim nShort As Integer = Math.Min(s - 2, Math.Max(2, CInt(Math.Round(s * sh / (sh + lg), MidpointRounding.AwayFromZero))))
        Dim nLong As Integer = s - nShort
        If sideA <= sideB Then nA = nShort : nB = nLong Else nA = nLong : nB = nShort
    End Sub

    'Section for bending over the depth Hb (width Bb): rows along Hb. The pattern is the perimeter pattern of the physical column: nW bars on the top and bottom rows, 2 on each intermediate row.
    'transposed = False: the physical column is Bb (counted along the width side) x Hb (rows along the depth side) = bending about axis 3 (M3);
    'transposed = True: the section is the same physical column seen for bending about axis 2 (M2), physical B = Hb, physical H = Bb, so the side counts are exchanged (needed for square columns with an odd count on one side pair).
    Public Function MakeSection(Bb As Double, Hb As Double, fc As Double, N As Integer, dia As Double, Optional designStrengths As Boolean = False, Optional transposed As Boolean = False) As ColumnSection
        Dim nB As Integer, nH As Integer            'bars along the physical B side and along the physical H side
        If transposed Then PerimeterCounts(Hb, Bb, N, nB, nH) Else PerimeterCounts(Bb, Hb, N, nB, nH)
        Dim nW As Integer = If(transposed, nH, nB)  'bars on the top/bottom rows of this section (along its width Bb)
        Dim nr As Integer = If(transposed, nB, nH)  'rows along this section's depth Hb
        Dim ab As Double = Math.PI * dia * dia / 4.0
        Dim ys(nr - 1) As Double, ar(nr - 1) As Double
        For k As Integer = 0 To nr - 1
            ys(k) = Cover + (Hb - 2.0 * Cover) * k / (nr - 1)
            ar(k) = If(k = 0 OrElse k = nr - 1, nW, 2) * ab
        Next
        If designStrengths Then Return New ColumnSection With {.B = Bb, .H = Hb, .Fc = Ts500.Fcd(fc), .Fy = Ts500.Fyd(Fy), .LayerY = ys, .LayerAs = ar, .Beta1Override = Ts500.K1(fc)}
        Return New ColumnSection With {.B = Bb, .H = Hb, .Fc = fc, .Fy = Fy, .LayerY = ys, .LayerAs = ar}
    End Function

    'number of bars for a target reinforcement ratio (even, >= 4)
    Public Function BarCount(b As Double, h As Double, rho As Double, dia As Double) As Integer
        Dim n As Integer = CInt(Math.Ceiling(rho * b * h / (Math.PI * dia * dia / 4.0)))
        If n < 4 Then n = 4
        If n Mod 2 = 1 Then n += 1
        Return n
    End Function

    'PCR-1 / PCR-2 (approved 2026-10-30): ACI 318M-14 minimum eccentricity (6.6.4.5.4: e_min = 15 mm + 0.03 h, one direction at a time) and non-sway moment magnification (6.6.4.5.2/3:
    'EI = 0.4 Ec Ig / (1 + beta_dns), Pc = pi^2 EI / lu^2 (k = 1), delta_ns = Cm / (1 - Pu / (0.75 Pc)) >= 1, applied when lu/r > 22 (sway frame, r = 0.3 h); Cm = 0.6 + 0.4 M1/M2 >= 0.4).
    'ETABS (V2b/V2d) applies e_min to every combination and delta_ns to the gravity combination only (sway combinations with P-Delta analysis: delta_s = 1).
    Public Const EminBase As Double = 15.24           'mm (0.6 in)
    Public Const DcrCap As Double = 10.0              'D/C returned when Pu >= 0.75 Pc (buckling)

    'delta_ns of one bending direction. depth: section depth in the bending plane, width: the other dimension; lu mm; m1, m2: signed end moments (any sign convention with same-sign = single curvature)
    Public Function DeltaNs(fc As Double, depth As Double, width As Double, lu As Double, pu As Double, betaDns As Double, mA As Double, mB As Double) As Double
        If pu <= 0.0 Then Return 1.0
        Dim big As Double = If(Math.Abs(mA) >= Math.Abs(mB), mA, mB), small As Double = If(Math.Abs(mA) >= Math.Abs(mB), mB, mA)
        Dim ratio As Double = If(Math.Abs(big) < 1.0E-9, 1.0, small / big)
        Dim r As Double = 0.3 * depth
        If lu / r <= 22.0 Then Return 1.0                                     'sway frame limit (ACI 6.2.5(a)); ETABS framing type Sway Special applies it to the gravity combination as well
        Dim ig As Double = width * depth * depth * depth / 12.0
        Dim ei As Double = 0.4 * Aci.Ec(fc) * ig / (1.0 + Math.Min(Math.Max(betaDns, 0.0), 1.0))
        Dim pc As Double = Math.PI * Math.PI * ei / (lu * lu)
        Dim den As Double = 1.0 - pu / (0.75 * pc)
        If den <= 0.0 Then Return Double.PositiveInfinity
        Return Math.Max(1.0 / den, 1.0)                                       'Cm = 1 (ETABS value for sway frames; conservative against 0.6 + 0.4 M1/M2)
    End Function

    'Design interaction curve of one section (phi Pn, phi Mn) sampled once, evaluated by interpolation (radial D/C of ETABS, PCR-3 decision 2026-10-30: OL/OC along the ray from the origin).
    Public Class ColCurve
        Public S As ColumnSection
        Public P() As Double, M() As Double          'ascending phi Pn (N, compression +), phi Mn (N.mm); P(0) = -pure tension, last point = design Pn,max
        Public PCap As Double, PTen As Double        'Pn,max design cap (+), tension capacity (-)
        Public Sub New(sec As ColumnSection)
            S = sec
            PCap = AciColumn.DesignPnMax(sec)
            PTen = AciColumn.PureTension(sec).PhiPn
            Dim n As Integer = 160
            Dim lp As New List(Of Double), lm As New List(Of Double)
            lp.Add(PTen) : lm.Add(0.0)
            Dim c0 As Double = 0.5, c1 As Double = 20.0 * sec.H
            For i As Integer = 0 To n - 1
                Dim c As Double = c0 * Math.Pow(c1 / c0, i / CDbl(n - 1))
                Dim pt = AciColumn.AtC(sec, c)
                Dim pp As Double = Math.Min(pt.PhiPn, PCap)
                If pp <= lp(lp.Count - 1) + 1.0E-6 Then Continue For
                lp.Add(pp) : lm.Add(Math.Abs(pt.PhiMn))
                If pp >= PCap Then Exit For
            Next
            P = lp.ToArray() : M = lm.ToArray()
        End Sub
        'moment capacity at axial force pu (0 outside the design range)
        Public Function CapM(pu As Double) As Double
            If pu >= P(P.Length - 1) OrElse pu <= P(0) Then Return 0.0
            Dim lo As Integer = 0, hi As Integer = P.Length - 1
            While hi - lo > 1
                Dim mid As Integer = (lo + hi) \ 2
                If P(mid) <= pu Then lo = mid Else hi = mid
            End While
            Dim t As Double = (pu - P(lo)) / (P(hi) - P(lo))
            Return M(lo) + t * (M(hi) - M(lo))
        End Function
    End Class

    'Radial demand/capacity (OL/OC): scale the load point (Pu, Mx, My) by lambda until the biaxial linear sum Mx/phiMnx(lambda Pu) + My/phiMny(lambda Pu) reaches 1; D/C = 1/lambda.
    'No moment: axial ratio Pu / cap.
    Public Function RadialDcr(cx As ColCurve, cy As ColCurve, pu As Double, m3 As Double, m2 As Double) As Double
        Dim lamCap As Double
        If pu > 0.0 Then
            lamCap = cx.P(cx.P.Length - 1) / pu
        ElseIf pu < 0.0 Then
            lamCap = cx.P(0) / pu
        Else
            lamCap = 50.0
        End If
        If m3 <= 0.0 AndAlso m2 <= 0.0 Then Return If(pu = 0.0, 0.0, 1.0 / lamCap)
        Dim lo As Double = 0.0, hi As Double = Math.Min(lamCap, 50.0)
        Dim gHi As Double = Dg(cx, cy, hi, pu, m3, m2)
        If gHi < 1.0 Then Return 1.0 / hi
        For i As Integer = 0 To 44
            Dim mid As Double = 0.5 * (lo + hi)
            If Dg(cx, cy, mid, pu, m3, m2) < 1.0 Then lo = mid Else hi = mid
        Next
        Return 1.0 / (0.5 * (lo + hi))
    End Function

    Private Function Dg(cx As ColCurve, cy As ColCurve, lam As Double, pu As Double, m3 As Double, m2 As Double) As Double
        Dim p As Double = lam * pu
        Dim sm As Double = 0.0
        If m3 > 0.0 Then
            Dim c As Double = cx.CapM(p)
            If c <= 0.0 Then Return 1.0E9
            sm += lam * m3 / c
        End If
        If m2 > 0.0 Then
            Dim c As Double = cy.CapM(p)
            If c <= 0.0 Then Return 1.0E9
            sm += lam * m2 / c
        End If
        Return sm
    End Function

    'Radial D/C with e_min (each direction on its own) and the magnification factors d3 (for M3) and d2 (for M2); pu N (compression +), moments N.mm magnitudes
    Public Function DcrWithEffects(cx As ColCurve, cy As ColCurve, pu As Double, m3 As Double, m2 As Double, d3 As Double, d2 As Double) As Double
        If Double.IsInfinity(d3) OrElse Double.IsInfinity(d2) Then Return DcrCap
        Dim e3 As Double = If(pu > 0.0, pu * (EminBase + 0.03 * cx.S.H), 0.0)      'M3: bending over depth H of sx
        Dim e2 As Double = If(pu > 0.0, pu * (EminBase + 0.03 * cy.S.H), 0.0)      'M2: bending over depth of sy
        Dim a As Double = RadialDcr(cx, cy, pu, d3 * Math.Max(m3, e3), d2 * m2)
        Dim b As Double = RadialDcr(cx, cy, pu, d3 * m3, d2 * Math.Max(m2, e2))
        Return Math.Min(Math.Max(a, b), DcrCap)
    End Function

    Public Function CheckGroup(fa As FrameAnalysis, group As String, b As Double, h As Double, fc As Double, rho As Double, dia As Double) As ColumnGroupResult
        Dim r As New ColumnGroupResult With {.Group = group, .B = b, .H = h, .Fc = fc, .Rho = rho, .BarDia = dia}
        r.NBars = BarCount(b, h, rho, dia)
        Dim ab As Double = Math.PI * dia * dia / 4.0
        r.AsTotal = r.NBars * ab
        Dim sx = MakeSection(b, h, fc, r.NBars, dia)      'bending about axis 3 (depth H, width B): M3
        Dim sy = MakeSection(h, b, fc, r.NBars, dia, False, True)      'bending about axis 2 (depth B, width H): M2
        Dim cvx As New ColCurve(sx), cvy As New ColCurve(sy)
        Dim ndMinSeen As Double = Double.MaxValue
        Dim m = fa.Gross.Model
        For ei As Integer = 0 To m.Elements.Count - 1
            If m.Elements(ei).Group <> group Then Continue For
            For Each cs In fa.Combos
                If cs.Seismic AndAlso cs.Forces.Count = 2 Then
                    'response spectrum magnitudes carry no sign: G +- E with every sign pattern of (P, M2, M3) (ETABS V2b: the pair (+,+),(-,-) alone under-predicts biaxial columns by up to 28 %)
                    Dim fp = cs.Forces(0)(ei), fn = cs.Forces(1)(ei)
                    Dim elS = m.Elements(ei)
                    Dim luS As Double = (elS.Length - elS.OffsetI - elS.OffsetJ) * 1000.0
                    Dim pgS As Double = (1.0 + fa.Ez) * fa.Rg.FaceForce(m.PatternIndex("G"))(ei)(0) * 1000.0      'sustained axial force of the seismic combinations (1 + ez) G, end I (magnitude used)
                    For endI As Integer = 0 To 1
                        Dim ip As Integer = If(endI = 0, 0, 6), i2 As Integer = If(endI = 0, 4, 10), i3 As Integer = If(endI = 0, 5, 11)
                        Dim sg As Double = If(endI = 0, 1.0, -1.0)
                        Dim gP As Double = sg * 0.5 * (fp(ip) + fn(ip)), eP As Double = sg * 0.5 * (fp(ip) - fn(ip))
                        Dim g2 As Double = 0.5 * (fp(i2) + fn(i2)), e2 As Double = 0.5 * (fp(i2) - fn(i2))
                        Dim g3 As Double = 0.5 * (fp(i3) + fn(i3)), e3 As Double = 0.5 * (fp(i3) - fn(i3))
                        For Each s1 In {-1.0, 1.0}
                            For Each s2 In {-1.0, 1.0}
                                For Each s3 In {-1.0, 1.0}
                                    Dim pu As Double = (gP + s1 * eP) * 1000.0
                                    Dim m2 As Double = Math.Abs(g2 + s2 * e2) * 1.0E6
                                    Dim m3 As Double = Math.Abs(g3 + s3 * e3) * 1.0E6
                                    r.NdMax = Math.Max(r.NdMax, pu)
                                    ndMinSeen = Math.Min(ndMinSeen, pu)
                                    'ETABS (V2e, NOMAG test) magnifies the moments of the seismic combinations as well with the Pu of the combination: delta_ns, sustained part (1 + ez) G
                                    Dim bS As Double = If(pu > 0.0, Math.Abs(pgS) / pu, 0.0)
                                    Dim dS3 As Double = DeltaNs(fc, h, b, luS, pu, bS, 1.0, 1.0), dS2 As Double = DeltaNs(fc, b, h, luS, pu, bS, 1.0, 1.0)
                                    r.DcrMax = Math.Max(r.DcrMax, DcrWithEffects(cvx, cvy, pu, m3, m2, dS3, dS2))
                                Next
                            Next
                        Next
                    Next
                    Continue For
                End If
                Dim el = m.Elements(ei)
                Dim lu As Double = (el.Length - el.OffsetI - el.OffsetJ) * 1000.0     'ACI 6.2.5.1: clear height between the joints (rigid end zones removed)
                Dim pgI As Double = 1.4 * fa.Rg.FaceForce(m.PatternIndex("G"))(ei)(0) * 1000.0         'sustained part of U1 (1.4 G), end I; end J sign flips
                For k As Integer = 0 To cs.Forces.Count - 1
                    Dim f = cs.Forces(k)(ei)
                    For endI As Integer = 0 To 1
                        Dim pu As Double = If(endI = 0, f(0), -f(6)) * 1000.0           'N
                        Dim m2 As Double = Math.Abs(If(endI = 0, f(4), f(10))) * 1.0E6
                        Dim m3 As Double = Math.Abs(If(endI = 0, f(5), f(11))) * 1.0E6
                        r.NdMax = Math.Max(r.NdMax, pu)
                        'non-sway magnification (gravity combination): signed end moments, diagram convention (-f(5) at I, +f(11) at J): same sign = single curvature
                        Dim beta As Double = If(pu > 0.0, Math.Abs(pgI) / pu, 0.0)
                        Dim d3 As Double = DeltaNs(fc, h, b, lu, pu, beta, -f(5) * 1.0E6, f(11) * 1.0E6)
                        Dim d2 As Double = DeltaNs(fc, b, h, lu, pu, beta, -f(4) * 1.0E6, f(10) * 1.0E6)
                        Dim d As Double = DcrWithEffects(cvx, cvy, pu, m3, m2, d3, d2)
                        r.DcrMax = Math.Max(r.DcrMax, d)
                    Next
                Next
            Next
        Next
        r.NdMin = If(ndMinSeen = Double.MaxValue, 0.0, ndMinSeen)
        r.G_Axial = r.NdMax / (0.4 * fc * b * h) - 1.0
        r.G_MinDim = 300.0 / Math.Min(b, h) - 1.0
        Dim actual As Double = r.AsTotal / (b * h)
        r.G_RhoLow = 0.01 / actual - 1.0
        r.G_RhoHigh = actual / 0.04 - 1.0
        r.Governing = If(r.DcrMax > 1.0, "D/C > 1", If(r.G_Axial > 0.0, "Ac < Nd/(0.4 fck)", If(r.G_RhoHigh > 0.0, "rho > 4 %", "ok")))
        Return r
    End Function
End Module

'Column shear and confinement, ACI 318M-14 Chapter 18 (special moment frame; decision IA 2026-10-30, ACI preferred over TBDY for the capacity design): design shear Ve = 2 Mpr / ln
'(18.7.6.1.1, Mpr with 1.25 fy, phi = 1, over the range of factored axial forces; each column end moment limited by the Mpr of the beams framing into its joint, shared between the columns above and below, and Ve not less than the analysis shear), phi_v = 0.75, Vc = 0 over lo when Pu < Ag f'c / 20 (18.7.6.2.1); transverse reinforcement Table 18.7.5.4 (a)-(c),
'spacing 18.7.5.3, lo 18.7.5.1, outside lo 18.7.5.5; Vs <= 0.66 sqrt(f'c) b d (22.5.1.2). Units N, mm, MPa.
Public Class ColumnShearResult
    Public Group As String
    Public Ve(1) As Double                    'N per direction (0: shear along local 2 / X, 1: along local 3 / Y)
    Public Legs(1) As Integer                 'tie legs parallel to the shear direction
    Public SConf, SMid, Lk As Double          'mm
    Public Tied As Double                     'confinement: Ash/s governing (mm2/mm)
    Public G_Crush(1) As Double               '(Ve/phi - Vc) / (0.66 sqrt(f'c) b d) - 1
    Public TieLenPerSet As Double             'mm of tie steel in one set (perimeter + cross ties + hooks)
    Public Governing As String
End Class

Public Module ColumnShear
    Public Const TieDia As Double = 10.0
    Public Const Phi As Double = 0.75
    Public Const TieCover As Double = 45.0      'mm to tie centreline: cover 40 + half of the 10 mm tie (ACI Table 20.6.1.3.1)
    Public Const ProbableFactor As Double = 1.25   'ACI 18.6.5 / 18.7.6.1.1: stress 1.25 fy in the longitudinal bars for Mpr

    'Mr (N.mm) of a TS 500-strength section (MakeSection(..., ts500:=True)) at axial compression Pu: nominal Mn at Pn(c) = Pu, no phi
    Public Function MrAtP(s As ColumnSection, Pu As Double) As Double
        Dim lo As Double = 0.5, hi As Double = 20.0 * s.H
        If AciColumn.AtC(s, hi).Pn < Pu Then Return 0.0
        For i As Integer = 0 To 59
            Dim mid As Double = 0.5 * (lo + hi)
            If AciColumn.AtC(s, mid).Pn < Pu Then lo = mid Else hi = mid
        Next
        Return Math.Abs(AciColumn.AtC(s, 0.5 * (lo + hi)).Mn)
    End Function

    'uniaxial design moment capacity (N.mm) of the section at axial compression Pu (N)
    Public Function PhiMnAtP(s As ColumnSection, Pu As Double) As Double
        If Pu >= AciColumn.DesignPnMax(s) Then Return Math.Abs(AciColumn.AtC(s, 0.5 * s.H).PhiMn) * 0.0
        Dim lo As Double = 0.5, hi As Double = 20.0 * s.H
        For i As Integer = 0 To 59
            Dim mid As Double = 0.5 * (lo + hi)
            Dim pPhi As Double = Math.Min(AciColumn.AtC(s, mid).PhiPn, AciColumn.DesignPnMax(s))
            If pPhi < Pu Then lo = mid Else hi = mid
        Next
        Return Math.Abs(AciColumn.AtC(s, 0.5 * (lo + hi)).PhiMn)
    End Function

    'Mpr (N.mm, phi = 1, 1.25 fy section) at the worst axial compression of the range [pLo, pHi] (nine points)
    Public Function MprMax(s As ColumnSection, pLo As Double, pHi As Double) As Double
        Dim best As Double = 0.0
        Dim lo As Double = Math.Max(pLo, 0.0), hi As Double = Math.Max(pHi, lo)
        For i As Integer = 0 To 8
            Dim pu As Double = lo + (hi - lo) * i / 8.0
            best = Math.Max(best, MrAtP(s, pu))
        Next
        Return best
    End Function

    'sum of the probable moments of the beams framing into a joint, in one frame direction (0: beams along X, 1: along Y): the larger of (top of one + bottom of the other) for two beams, the larger of top and bottom for one
    Public Function JointMprSum(fa As FrameAnalysis, node As Integer, dir As Integer, beams As Dictionary(Of String, BeamGroupResult), fibs As Dictionary(Of String, BeamFibre)) As Double
        If beams Is Nothing Then Return 1.0E30            'no beam data: no limit by the beams
        Dim mm = fa.Gross.Model
        Dim mT As New List(Of Double), mB As New List(Of Double)
        For ei As Integer = 0 To mm.Elements.Count - 1
            Dim e = mm.Elements(ei)
            If Math.Abs(e.Axes(0, 2)) > 0.9 Then Continue For
            If e.NI <> node AndAlso e.NJ <> node Then Continue For
            Dim alongX As Boolean = Math.Abs(mm.Nodes(e.NJ).X - mm.Nodes(e.NI).X) > 0.001
            If alongX <> (dir = 0) Then Continue For
            Dim br = beams(e.Group)
            Dim fb As BeamFibre = If(fibs IsNot Nothing AndAlso fibs.ContainsKey(e.Group), fibs(e.Group), Nothing)
            mT.Add(BeamShear.MrOf(br, br.AsTop, fb)) : mB.Add(BeamShear.MrOf(br, br.AsBot, fb))
        Next
        If mT.Count = 2 Then Return Math.Max(mT(0) + mB(1), mT(1) + mB(0))
        If mT.Count = 1 Then Return Math.Max(mT(0), mB(0))
        Return 1.0E30                                       'no beam framing in this direction: no limit
    End Function

    Public Function Design(fa As FrameAnalysis, cg As ColumnGroupResult, beams As Dictionary(Of String, BeamGroupResult), fibs As Dictionary(Of String, BeamFibre)) As ColumnShearResult
        Dim o As New ColumnShearResult With {.Group = cg.Group}
        Dim sx = ColumnDesign.MakeSection(cg.B, cg.H, cg.Fc, cg.NBars, cg.BarDia, False)               'M3, shear along local 2
        Dim sy = ColumnDesign.MakeSection(cg.H, cg.B, cg.Fc, cg.NBars, cg.BarDia, False, True)        'M2, shear along local 3
        sx.Fy = ProbableFactor * ColumnDesign.Fy : sy.Fy = ProbableFactor * ColumnDesign.Fy
        Dim mprC(1) As Double
        mprC(0) = MprMax(sx, cg.NdMin, cg.NdMax) : mprC(1) = MprMax(sy, cg.NdMin, cg.NdMax)
        Dim m = fa.Gross.Model
        Dim lnMin As Double = Double.MaxValue
        For ei As Integer = 0 To m.Elements.Count - 1
            Dim e = m.Elements(ei)
            If e.Group <> cg.Group Then Continue For
            Dim ln As Double = (e.Length - e.OffsetI - e.OffsetJ) * 1000.0
            lnMin = Math.Min(lnMin, ln)
            'analysis shear of the seismic combinations (Ve is never less than that, 18.7.6.1.1)
            Dim vAna(1) As Double
            For Each cs In fa.Combos
                If Not cs.Seismic Then Continue For
                For q As Integer = 0 To cs.Forces.Count - 1
                    Dim f = cs.Forces(q)(ei)
                    vAna(0) = Math.Max(vAna(0), Math.Max(Math.Abs(f(1)), Math.Abs(f(7))) * 1000.0)
                    vAna(1) = Math.Max(vAna(1), Math.Max(Math.Abs(f(2)), Math.Abs(f(8))) * 1000.0)
                Next
            Next
            For d As Integer = 0 To 1
                'end moments: column Mpr, limited by the beams of the joint (half to the column above and half to the one below; all to the column below at the roof; none at the base)
                Dim hasAboveTop As Boolean = False
                For ej As Integer = 0 To m.Elements.Count - 1
                    If Math.Abs(m.Elements(ej).Axes(0, 2)) > 0.9 AndAlso m.Elements(ej).NI = e.NJ Then hasAboveTop = True
                Next
                Dim mTop As Double = Math.Min(mprC(d), If(hasAboveTop, 0.5, 1.0) * JointMprSum(fa, e.NJ, d, beams, fibs))
                Dim atBase As Boolean = m.Nodes(e.NI).Z < 0.01
                Dim hasBelowBot As Boolean = Not atBase
                Dim mBot As Double = If(atBase, mprC(d), Math.Min(mprC(d), 0.5 * JointMprSum(fa, e.NI, d, beams, fibs)))
                Dim ve As Double = Math.Max((mTop + mBot) / ln, vAna(d))
                o.Ve(d) = Math.Max(o.Ve(d), ve)
            Next
        Next
        Dim fyt As Double = 420.0, aw As Double = cg.B * cg.H
        Dim ach As Double = (cg.B - 2.0 * (TieCover - TieDia / 2.0)) * (cg.H - 2.0 * (TieCover - TieDia / 2.0))   'out-to-out of the hoops
        Dim nB As Integer, nH As Integer
        ColumnDesign.PerimeterCounts(cg.B, cg.H, cg.NBars, nB, nH)
        o.Legs(0) = Math.Max(2, nB) : o.Legs(1) = Math.Max(2, nH)        'every perimeter bar is supported by a hoop corner or a crosstie (assumption); legs along X cross the B side
        Dim avTie As Double = Math.PI * TieDia * TieDia / 4.0
        'spacing limits 18.7.5.3: smallest dimension / 4, six longitudinal diameters, so
        Dim hx As Double = Math.Max((cg.B - 2.0 * ColumnDesign.Cover) / Math.Max(nB - 1, 1), (cg.H - 2.0 * ColumnDesign.Cover) / Math.Max(nH - 1, 1))
        Dim so As Double = Math.Min(150.0, Math.Max(100.0, 100.0 + (350.0 - hx) / 3.0))
        Dim sLim As Double = Math.Min(Math.Min(Math.Min(cg.B, cg.H) / 4.0, 6.0 * cg.BarDia), so)
        Dim kf As Double = Math.Max(cg.Fc / 175.0 + 0.6, 1.0)
        Dim nl As Double = CDbl(Math.Max(cg.NBars, 4))
        Dim kn As Double = nl / (nl - 2.0)
        Dim pHiConf As Boolean = cg.NdMax > 0.3 * aw * cg.Fc OrElse cg.Fc > 70.0
        Dim vcZero As Boolean = cg.NdMin < aw * cg.Fc / 20.0          '18.7.6.2.1(b): Pu including earthquake effects below Ag f'c/20 (smallest Pu governs); (a) holds, Ve is entirely earthquake induced
        Dim sConf As Double = sLim, tieMax As Double = 0.0, sMidShear As Double = Double.MaxValue
        For d As Integer = 0 To 1
            Dim bc As Double = If(d = 0, cg.B, cg.H) - 2.0 * TieCover            'core dimension perpendicular to the legs (centre to centre)
            Dim ashA As Double = 0.3 * (aw / ach - 1.0) * cg.Fc / fyt
            Dim ashB As Double = 0.09 * cg.Fc / fyt
            Dim ashC As Double = If(pHiConf, 0.2 * kf * kn * Math.Max(cg.NdMax, 0.0) / (fyt * ach), 0.0)
            Dim ashPerS As Double = bc * Math.Max(Math.Max(ashA, ashB), ashC)      'Table 18.7.5.4 (a), (b), (c)
            Dim bw As Double = If(d = 0, cg.B, cg.H), dEff As Double = If(d = 0, cg.H, cg.B) - 50.0
            Dim vcSect As Double = 0.17 * (1.0 + Math.Max(cg.NdMin, 0.0) / (14.0 * aw)) * Math.Sqrt(cg.Fc) * bw * dEff
            Dim vc As Double = If(vcZero, 0.0, vcSect)
            Dim avShearPerS As Double = Math.Max(o.Ve(d) / Phi - vc, 0.0) / (fyt * dEff)
            Dim avMidPerS As Double = Math.Max(o.Ve(d) / Phi - vcSect, 0.0) / (fyt * dEff)
            tieMax = Math.Max(tieMax, Math.Max(ashPerS, avShearPerS))
            Dim legsA As Double = o.Legs(d) * avTie
            sConf = Math.Min(sConf, legsA / Math.Max(Math.Max(ashPerS, avShearPerS), 0.000001))
            sMidShear = Math.Min(sMidShear, legsA / Math.Max(avMidPerS, 0.000001))
            o.G_Crush(d) = (o.Ve(d) / Phi - vc) / (0.66 * Math.Sqrt(cg.Fc) * bw * dEff) - 1.0
        Next
        o.SConf = Math.Max(50.0, sConf)
        o.SMid = Math.Min(Math.Min(6.0 * cg.BarDia, 150.0), sMidShear)           '18.7.5.5 and shear outside lo
        o.Tied = tieMax
        o.Lk = Math.Max(Math.Max(lnMin / 6.0, Math.Max(cg.B, cg.H)), 450.0)       '18.7.5.1: depth, ln/6, 450 mm
        o.TieLenPerSet = o.Legs(0) * (cg.H - 2.0 * TieCover) + o.Legs(1) * (cg.B - 2.0 * TieCover) + 2.0 * 12.0 * TieDia
        o.Governing = If(o.G_Crush(0) > 0.0 OrElse o.G_Crush(1) > 0.0, "Vs > 0.66 sqrt(f'c) b d", If(sConf < 50.0, "tie spacing < 50 mm", "ok"))
        Return o
    End Function
End Module
