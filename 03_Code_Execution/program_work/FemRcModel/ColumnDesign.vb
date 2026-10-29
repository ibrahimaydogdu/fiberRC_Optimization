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
    Public G_Axial As Double                 'A2: Ndm/(0.40 fck Ac) - 1
    Public G_MinDim As Double                'A1: 300/min(b,h) - 1
    Public G_RhoLow, G_RhoHigh As Double     'A3
    Public Governing As String
End Class

Public Module ColumnDesign
    Public Const Fy As Double = 420.0
    Public Const Cover As Double = 60.0          'mm to bar centroid: ACI 318M-14 Table 20.6.1.3.1 cover 40 + tie 10 + half of 20 mm bar (PDF 337)
    Public Const BarSpacing As Double = 150.0    'mm, intermediate rows for the side bars [ASSUMPTION; TBDY 7.3.4 sets tie hook spacing]

    'Perimeter layout: rows (layers) along the bending depth Hb; width Bb. N bars (even >= 4) of diameter dia. Returns ColumnSection.
    Public Function MakeSection(Bb As Double, Hb As Double, fc As Double, N As Integer, dia As Double, Optional designStrengths As Boolean = False) As ColumnSection
        Dim nr As Integer = Math.Max(2, CInt(Math.Floor((Hb - 2.0 * Cover) / BarSpacing)) + 1)
        nr = Math.Min(nr, N \ 2)
        If nr < 2 Then nr = 2
        Dim nTB As Integer = Math.Max(2, CInt(Math.Ceiling((N - 2.0 * (nr - 2)) / 2.0)))
        Dim ab As Double = Math.PI * dia * dia / 4.0
        Dim ys(nr - 1) As Double, ar(nr - 1) As Double
        For k As Integer = 0 To nr - 1
            ys(k) = Cover + (Hb - 2.0 * Cover) * k / (nr - 1)
            ar(k) = If(k = 0 OrElse k = nr - 1, nTB, 2) * ab
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

    Public Function CheckGroup(fa As FrameAnalysis, group As String, b As Double, h As Double, fc As Double, rho As Double, dia As Double) As ColumnGroupResult
        Dim r As New ColumnGroupResult With {.Group = group, .B = b, .H = h, .Fc = fc, .Rho = rho, .BarDia = dia}
        r.NBars = BarCount(b, h, rho, dia)
        Dim ab As Double = Math.PI * dia * dia / 4.0
        r.AsTotal = r.NBars * ab
        Dim sx = MakeSection(b, h, fc, r.NBars, dia)      'bending about axis 3 (depth H, width B): M3
        Dim sy = MakeSection(h, b, fc, r.NBars, dia)      'bending about axis 2 (depth B, width H): M2
        Dim m = fa.Gross.Model
        For ei As Integer = 0 To m.Elements.Count - 1
            If m.Elements(ei).Group <> group Then Continue For
            For Each cs In fa.Combos
                For k As Integer = 0 To cs.Forces.Count - 1
                    Dim f = cs.Forces(k)(ei)
                    For endI As Integer = 0 To 1
                        Dim pu As Double = If(endI = 0, f(0), -f(6)) * 1000.0           'N
                        Dim m2 As Double = Math.Abs(If(endI = 0, f(4), f(10))) * 1.0E6
                        Dim m3 As Double = Math.Abs(If(endI = 0, f(5), f(11))) * 1.0E6
                        r.NdMax = Math.Max(r.NdMax, pu)
                        Dim d As Double = AciColumn.BiaxialDcr(sx, sy, pu, m3, m2)
                        r.DcrMax = Math.Max(r.DcrMax, d)
                    Next
                Next
            Next
        Next
        r.G_Axial = r.NdMax / (0.4 * fc * b * h) - 1.0
        r.G_MinDim = 300.0 / Math.Min(b, h) - 1.0
        Dim actual As Double = r.AsTotal / (b * h)
        r.G_RhoLow = 0.01 / actual - 1.0
        r.G_RhoHigh = actual / 0.04 - 1.0
        r.Governing = If(r.DcrMax > 1.0, "D/C > 1", If(r.G_Axial > 0.0, "Ac < Nd/(0.4 fck)", If(r.G_RhoHigh > 0.0, "rho > 4 %", "ok")))
        Return r
    End Function
End Module

'Column shear and confinement (TBDY-2018 7.3.4, 7.3.7; Vc, Vr by ACI 318M-14 instead of TS 500). Units N, mm, MPa.
Public Class ColumnShearResult
    Public Group As String
    Public Ve(1) As Double                    'N per direction (0: shear along local 2 / X, 1: along local 3 / Y)
    Public Legs(1) As Integer                 'tie legs parallel to the shear direction
    Public SConf, SMid, Lk As Double          'mm
    Public Tied As Double                     'confinement: Ash/s governing (mm2/mm)
    Public G_Crush(1) As Double               'Eq. 7.7: Ve/(0.85 Aw sqrt(fck)) - 1
    Public TieLenPerSet As Double             'mm of tie steel in one set (perimeter + cross ties + hooks)
    Public Governing As String
End Class

Public Module ColumnShear
    Public Const TieDia As Double = 10.0
    Public Const Phi As Double = 0.75
    Public Const TieCover As Double = 45.0      'mm to tie centreline: cover 40 + half of the 10 mm tie (ACI Table 20.6.1.3.1)

    'Mr (N.mm) of a TS 500-strength section (MakeSection(..., ts500:=True)) at axial compression Pu: nominal Mn at Pn(c) = Pu, no phi
    Public Function MrAtP(s As ColumnSection, Pu As Double) As Double
        Dim lo As Double = 0.5, hi As Double = 20.0 * s.H
        If AciColumn.AtC(s, hi).Pn < Pu Then Return 0.0
        For i As Integer = 0 To 199
            Dim mid As Double = 0.5 * (lo + hi)
            If AciColumn.AtC(s, mid).Pn < Pu Then lo = mid Else hi = mid
        Next
        Return Math.Abs(AciColumn.AtC(s, 0.5 * (lo + hi)).Mn)
    End Function

    'uniaxial design moment capacity (N.mm) of the section at axial compression Pu (N)
    Public Function PhiMnAtP(s As ColumnSection, Pu As Double) As Double
        If Pu >= AciColumn.DesignPnMax(s) Then Return Math.Abs(AciColumn.AtC(s, 0.5 * s.H).PhiMn) * 0.0
        Dim lo As Double = 0.5, hi As Double = 20.0 * s.H
        For i As Integer = 0 To 199
            Dim mid As Double = 0.5 * (lo + hi)
            Dim pPhi As Double = Math.Min(AciColumn.AtC(s, mid).PhiPn, AciColumn.DesignPnMax(s))
            If pPhi < Pu Then lo = mid Else hi = mid
        Next
        Return Math.Abs(AciColumn.AtC(s, 0.5 * (lo + hi)).PhiMn)
    End Function

    Public Function Design(fa As FrameAnalysis, cg As ColumnGroupResult) As ColumnShearResult
        Dim o As New ColumnShearResult With {.Group = cg.Group}
        Dim sx = ColumnDesign.MakeSection(cg.B, cg.H, cg.Fc, cg.NBars, cg.BarDia, True)      'M3, shear along local 2 (TS 500 strengths for Mr)
        Dim sy = ColumnDesign.MakeSection(cg.H, cg.B, cg.Fc, cg.NBars, cg.BarDia, True)      'M2, shear along local 3
        Dim m = fa.Gross.Model
        Dim ndSeen As New List(Of Double)
        Dim lnMin As Double = Double.MaxValue
        For ei As Integer = 0 To m.Elements.Count - 1
            Dim e = m.Elements(ei)
            If e.Group <> cg.Group Then Continue For
            Dim ln As Double = (e.Length - e.OffsetI - e.OffsetJ) * 1000.0
            lnMin = Math.Min(lnMin, ln)
            Dim vseisMax(1) As Double
            For Each cs In fa.Combos
                If Not cs.Seismic Then Continue For
                For k As Integer = 0 To cs.Forces.Count - 1
                    Dim f = cs.Forces(k)(ei)
                    Dim pu As Double = Math.Max(0.0, Math.Min(f(0), -f(6))) * 1000.0     'smaller of the end compressions (conservative for Mr only above balanced; scan below)
                    Dim pu2 As Double = Math.Max(f(0), -f(6)) * 1000.0
                    For Each pn In {pu, pu2, 0.5 * (pu + pu2)}
                        Dim vx As Double = 2.0 * 1.4 * MrAtP(sx, pn) / ln
                        Dim vy As Double = 2.0 * 1.4 * MrAtP(sy, pn) / ln
                        o.Ve(0) = Math.Max(o.Ve(0), vx) : o.Ve(1) = Math.Max(o.Ve(1), vy)
                    Next
                Next
            Next
        Next
        Dim fyt As Double = 420.0, aw As Double = cg.B * cg.H
        Dim bk(1) As Double
        bk(0) = cg.H - 2.0 * TieCover : bk(1) = cg.B - 2.0 * TieCover      'core dimension along the shear direction (0: X = depth H)
        Dim ac As Double = aw, ack As Double = (cg.B - 2.0 * TieCover) * (cg.H - 2.0 * TieCover)
        Dim ndFrac As Double = cg.NdMax / (aw * cg.Fc)
        Dim avTie As Double = Math.PI * TieDia * TieDia / 4.0
        Dim sLim As Double = Math.Min(Math.Min(Math.Min(cg.B, cg.H) / 3.0, 150.0), 6.0 * cg.BarDia)
        Dim sConf As Double = sLim, tieMax As Double = 0.0
        For d As Integer = 0 To 1
            'legs parallel to shear direction d: spacing a <= 25 phi_e = 250 mm over the dimension perpendicular to the legs
            Dim perp As Double = If(d = 0, cg.B, cg.H) - 2.0 * TieCover
            o.Legs(d) = Math.Max(2, CInt(Math.Ceiling(perp / 250.0)) + 1)
            Dim ashPerS As Double
            Dim fckOverFy As Double = cg.Fc / fyt
            Dim bkd As Double = If(d = 0, cg.B, cg.H) - 2.0 * TieCover       'bk of the tie leg direction (leg length across)
            If ndFrac > 0.2 Then
                ashPerS = Math.Max(0.3 * bkd * (ac / ack - 1.0) * fckOverFy, 0.075 * bkd * fckOverFy)
            Else
                ashPerS = 2.0 / 3.0 * Math.Max(0.3 * bkd * (ac / ack - 1.0) * fckOverFy, 0.075 * bkd * fckOverFy)
            End If
            Dim vseis As Double = o.Ve(d)
            Dim ndN As Double = cg.NdMax
            Dim vc As Double = If(ndFrac <= 0.05, 0.0, 0.17 * (1.0 + ndN / (14.0 * aw)) * Math.Sqrt(cg.Fc) * If(d = 0, cg.B, cg.H) * ((If(d = 0, cg.H, cg.B)) - 50.0))
            Dim dEff As Double = If(d = 0, cg.H, cg.B) - 50.0
            Dim avShearPerS As Double = Math.Max(o.Ve(d) / Phi - vc, 0.0) / (fyt * dEff)
            tieMax = Math.Max(tieMax, Math.Max(ashPerS, avShearPerS))
            Dim legsA As Double = o.Legs(d) * avTie
            sConf = Math.Min(sConf, legsA / Math.Max(Math.Max(ashPerS, avShearPerS), 0.000001))
            o.G_Crush(d) = o.Ve(d) / (0.85 * aw * Math.Sqrt(cg.Fc)) - 1.0
        Next
        o.SConf = Math.Max(50.0, sConf)
        o.SMid = Math.Min(Math.Min(Math.Min(cg.B, cg.H) / 2.0, 200.0), 2.0 * o.SConf)      'mid-zone shear governed by the same demand: not more than 2 x s_conf [ASSUMPTION]
        o.Tied = tieMax
        o.Lk = Math.Max(Math.Max(lnMin / 6.0, 1.5 * Math.Max(cg.B, cg.H)), 500.0)
        Dim nxl As Integer = o.Legs(0), nyl As Integer = o.Legs(1)
        o.TieLenPerSet = nxl * (cg.H - 2.0 * TieCover) + nyl * (cg.B - 2.0 * TieCover) + 2.0 * 12.0 * TieDia
        o.Governing = If(o.G_Crush(0) > 0.0 OrElse o.G_Crush(1) > 0.0, "Ve > 0.85 Aw sqrt(fck)", If(sConf < 50.0, "tie spacing < 50 mm", "ok"))
        Return o
    End Function
End Module
