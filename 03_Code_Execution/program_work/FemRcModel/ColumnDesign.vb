Imports System
Imports System.Collections.Generic
Imports FemCore
Imports RcCore

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
    Public Const Cover As Double = 50.0          'mm to bar centroid (cover 30 + stirrup 10 + half of 20 mm bar) [ASSUMPTION]
    Public Const BarSpacing As Double = 150.0    'mm, intermediate rows for the side bars [ASSUMPTION; TBDY 7.3.4 sets tie hook spacing]

    'Perimeter layout: rows (layers) along the bending depth Hb; width Bb. N bars (even >= 4) of diameter dia. Returns ColumnSection.
    Public Function MakeSection(Bb As Double, Hb As Double, fc As Double, N As Integer, dia As Double) As ColumnSection
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
