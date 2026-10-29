Imports System
Imports System.Collections.Generic
Imports FemCore

'Beam flexural design from the combination envelope (ACI 318M-14 flexure; fib MC2010 fibre contribution for SFRC beams). Units N, mm, MPa inside; forces from the FEM in kN, m.
'Sign convention (sagging positive): M(x = 0) = -f(5), M(x = Lc) = f(11), mid = Mid(5)  (face forces f, FemStatic station convention).
Public Class BeamFibre
    Public Params As RcCore.SfrcParams   'Nothing = plain RC
    Public DoseKgM3 As Double
End Class

Public Class BeamGroupResult
    Public Group As String
    Public B, H, D, Fc As Double
    Public MuTop, MuBot As Double             'envelope demands, N.mm (hogging at the faces / sagging at faces and midspan)
    Public AsTop, AsBot As Double             'provided (required, minimum and ratio rules applied), mm2
    Public AsMin, AsMaxRho As Double
    Public RhoTop, RhoBot As Double
    Public EpsTOk As Boolean
    Public Governing As String
End Class

Public Module BeamDesign
    Public Const Fy As Double = 420.0
    Public Const CoverToCentroid As Double = 60.0      'mm: cover 30 + stirrup 10 + half of a 20 mm bar [ASSUMPTION; to be replaced by the detailing rule]

    'envelope demands of one beam (N.mm)
    Public Sub Demands(fa As FrameAnalysis, ei As Integer, ByRef top As Double, ByRef bot As Double)
        top = 0.0 : bot = 0.0
        For Each cs In fa.Combos
            Dim src = If(cs.Seismic, fa.Cracked, fa.Gross)
            For k As Integer = 0 To cs.Forces.Count - 1
                Dim f = cs.Forces(k)(ei)
                Dim mI As Double = -f(5) * 1.0E6, mJ As Double = f(11) * 1.0E6, mm As Double = cs.Mid(k)(ei)(5) * 1.0E6
                top = Math.Max(top, Math.Max(-mI, -mJ))
                bot = Math.Max(bot, Math.Max(Math.Max(mI, mJ), mm))
            Next
        Next
    End Sub

    Private Function Required(Mu As Double, b As Double, h As Double, d As Double, fc As Double, fib As BeamFibre) As Double
        If Mu <= 0.0 Then Return 0.0
        If fib Is Nothing OrElse fib.Params Is Nothing Then Return RcCore.AciBeam.RequiredAs(Mu, b, d, fc, Fy)
        'SFRC: bisection on As (monotonic), lcs = h - c of the previous trial [ASSUMPTION: lcs = distance neutral axis - tension fibre; srm pending]
        Dim lo As Double = 0.0, hi As Double = 0.05 * b * d
        If RcCore.SfrcBeam.Flexure(b, h, d, hi, fc, Fy, fib.Params, h).PhiMn < Mu Then Return Double.PositiveInfinity
        Dim lcs As Double = h
        For i As Integer = 0 To 59
            Dim mid As Double = 0.5 * (lo + hi)
            Dim r = RcCore.SfrcBeam.Flexure(b, h, d, Math.Max(mid, 1.0), fc, Fy, fib.Params, lcs)
            lcs = Math.Max(h - r.C, 0.2 * h)
            If r.PhiMn >= Mu Then hi = mid Else lo = mid
        Next
        Return hi
    End Function

    Private Function StrainOk(b As Double, h As Double, d As Double, As_ As Double, fc As Double, fib As BeamFibre) As Double
        If fib Is Nothing OrElse fib.Params Is Nothing Then Return RcCore.AciBeam.Flexure(b, d, As_, fc, Fy).EpsT
        Return RcCore.SfrcBeam.Flexure(b, h, d, As_, fc, Fy, fib.Params, 0.7 * h).EpsT
    End Function

    'Group design: reinforcement = envelope of all members of the group
    Public Function DesignGroup(fa As FrameAnalysis, group As String, b As Double, h As Double, fc As Double, fib As BeamFibre) As BeamGroupResult
        Dim r As New BeamGroupResult With {.Group = group, .B = b, .H = h, .D = h - CoverToCentroid, .Fc = fc}
        Dim m = fa.Gross.Model
        For ei As Integer = 0 To m.Elements.Count - 1
            If m.Elements(ei).Group <> group Then Continue For
            Dim t As Double, bt As Double
            Demands(fa, ei, t, bt)
            r.MuTop = Math.Max(r.MuTop, t) : r.MuBot = Math.Max(r.MuBot, bt)
        Next
        Dim fctd As Double = 0.35 * Math.Sqrt(fc) / 1.5 * 1.0       'placeholder for fctd: 0.35 sqrt(fck)/gamma_c (TS 500 form) [ASSUMPTION; B4 TBDY Eq. 7.8]
        r.AsMin = Math.Max(RcCore.AciBeam.MinAs(b, r.D, fc, Fy), 0.8 * fctd / (Fy / 1.15) * b * r.D)
        r.AsMaxRho = 0.02 * b * r.D                                       'TBDY 7.4.2.4: rho <= 2 %
        Dim reqTop As Double = Required(r.MuTop, b, h, r.D, fc, fib)
        Dim reqBot As Double = Required(r.MuBot, b, h, r.D, fc, fib)
        r.AsTop = Math.Max(reqTop, r.AsMin)
        r.AsBot = Math.Max(Math.Max(reqBot, r.AsMin), 0.5 * r.AsTop)      'TBDY 7.4.2.3 (DTS 1,1a,2,2a): bottom >= 50 % of top
        r.RhoTop = r.AsTop / (b * r.D) : r.RhoBot = r.AsBot / (b * r.D)
        r.Governing = If(Double.IsInfinity(r.AsTop) OrElse Double.IsInfinity(r.AsBot), "section too small", If(r.AsTop > r.AsMaxRho OrElse r.AsBot > r.AsMaxRho, "rho > 2 %", "ok"))
        'ACI 9.3.3.1 / A10: net tensile strain >= 0.004 at the provided steel (B7 TBDY: TS 500 maximum not available -> ACI limit)
        r.EpsTOk = False
        If Not Double.IsInfinity(r.AsTop) AndAlso Not Double.IsInfinity(r.AsBot) Then
            Dim eT As Double = StrainOk(b, h, r.D, r.AsTop, fc, fib)
            Dim eB As Double = StrainOk(b, h, r.D, r.AsBot, fc, fib)
            r.EpsTOk = eT >= 0.004 AndAlso eB >= 0.004
            If Not r.EpsTOk AndAlso r.Governing = "ok" Then r.Governing = "epsT < 0.004"
        End If
        Return r
    End Function
End Module
