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

'Beam shear, ACI 318M-14 18.6.5 capacity design (decision IA 2026-10-30): Ve = (Mpr,top + Mpr,bot)/ln + factored gravity shear, Mpr with 1.25 fy and phi = 1 (fibre part: gamma_F = 1), phi_v = 0.75,
'Vc = 0 over 2h when the earthquake shear is at least half of Ve (18.6.5.2); hoops 18.6.4.4. Units N, mm, MPa.
Public Class BeamShearResult
    Public Group As String
    Public Ve, Vdy, Vseis As Double           'N: design shear, gravity (simple beam) part, capacity-design part (Mpr,top + Mpr,bot)/ln
    Public MrTop, MrBot, Ln As Double         'N.mm, N.mm, mm
    Public ZeroVc As Boolean                  'ACI 18.6.5.2: Vseis >= Ve/2 -> Vc = 0 in the confinement zones
    Public AvConfPerS, AvMidPerS As Double    'required Av/s, mm2/mm
    Public SConf, SMid As Double              'chosen spacings, mm
    Public G_Crush As Double                  '(Ve/phi - Vc,conf) / (0.66 sqrt(f'c) bw d) - 1  (ACI 22.5.1.2)
    Public Governing As String
End Class

Public Module BeamShear
    Public Const Phi As Double = 0.75           'ACI 21.2.1(b) [ASSUMPTION: TBDY/TS 500 use gamma factors instead]
    Public Const StirrupDia As Double = 10.0    'mm, two legs [ASSUMPTION]
    Public Const LongBarDia As Double = 20.0    'mm, for the 8 phi spacing limit [ASSUMPTION]

    'ACI Mpr: probable moment strength at the beam face, 1.25 fy, no phi (18.6.5.1; R18.6.5 strain hardening). Fibre part: gamma_F = 1 (probable strength; assumption).
    Public Function MrOf(r As BeamGroupResult, As_ As Double, fib As BeamFibre) As Double
        Dim fyp As Double = 1.25 * BeamDesign.Fy
        If fib Is Nothing OrElse fib.Params Is Nothing Then Return RcCore.AciBeam.Flexure(r.B, r.D, As_, r.Fc, fyp).Mn
        Dim q As New RcCore.SfrcParams With {.FR1 = fib.Params.FR1, .FR3 = fib.Params.FR3, .GammaF = 1.0, .Linear = fib.Params.Linear, .EpsFu = fib.Params.EpsFu, .WMaxLimit = fib.Params.WMaxLimit, .Cmod3 = fib.Params.Cmod3, .WuOverride = fib.Params.WuOverride}
        Return RcCore.SfrcBeam.Flexure(r.B, r.H, r.D, As_, r.Fc, fyp, q, 0.7 * r.H).Mn
    End Function

    'Vdy of one beam: simple-beam shear of the vertical loads of the seismic combination (G + Q + 0.2 S) = half of the vertical load on the clear span
    Public Function VdySimple(fa As FrameAnalysis, ei As Integer) As Double
        Dim m = fa.Gross.Model
        Dim pg = m.PatternIndex("G"), pq = m.PatternIndex("Q"), ps = m.PatternIndex("S")
        Dim w As Double = 0.0
        For Each t In {Tuple.Create(pg, 1.0), Tuple.Create(pq, 1.0), Tuple.Create(ps, 0.2)}
            Dim f = fa.Rg.FaceForce(t.Item1)(ei)
            w += t.Item2 * (f(1) + f(7))
        Next
        Return Math.Abs(w) / 2.0 * 1000.0
    End Function

    Public Function DesignGroup(fa As FrameAnalysis, r As BeamGroupResult, fib As BeamFibre, fctk As Double, fFtuk As Double) As BeamShearResult
        Dim o As New BeamShearResult With {.Group = r.Group}
        o.MrTop = MrOf(r, r.AsTop, fib) : o.MrBot = MrOf(r, r.AsBot, fib)
        Dim m = fa.Gross.Model
        Dim worst As Double = -1.0
        For ei As Integer = 0 To m.Elements.Count - 1
            Dim e = m.Elements(ei)
            If e.Group <> r.Group Then Continue For
            Dim ln As Double = (e.Length - e.OffsetI - e.OffsetJ) * 1000.0
            Dim vdy As Double = VdySimple(fa, ei)
            Dim vs As Double = (o.MrTop + o.MrBot) / ln
            If vdy + vs > worst Then worst = vdy + vs : o.Ve = vdy + vs : o.Vdy = vdy : o.Vseis = vs : o.Ln = ln
        Next
        o.ZeroVc = o.Vseis >= 0.5 * o.Ve
        Dim fyt As Double = 420.0
        Dim av As Double = 2.0 * Math.PI * StirrupDia * StirrupDia / 4.0
        'confinement zones: Vc = 0 (when Vseis > Ve/2); lifli kirişte lif katkısı da 0 alınır (Critic kararı bekliyor, A11 C1e)
        Dim vcConf As Double = If(o.ZeroVc, 0.0, VcOf(r, fib, fctk, fFtuk))
        o.G_Crush = (o.Ve / Phi - vcConf) / (0.66 * Math.Sqrt(r.Fc) * r.B * r.D) - 1.0
        o.AvConfPerS = Math.Max(o.Ve / Phi - vcConf, 0.0) / (fyt * r.D)
        o.AvConfPerS = Math.Max(o.AvConfPerS, RcCore.AciBeam.AvMinPerS(r.B, r.Fc, fyt))
        Dim sLimConf As Double = Math.Min(Math.Min(r.D / 4.0, 6.0 * LongBarDia), 150.0)       'ACI 18.6.4.4: d/4, 6 db, 150 mm
        o.SConf = Math.Max(50.0, Math.Min(av / o.AvConfPerS, sLimConf))
        'middle zone: Vc (plain) / VRd,F (SFRC) acts; spacing limit d/2, 300 mm [ASSUMPTION]
        Dim vcMid As Double = VcOf(r, fib, fctk, fFtuk)
        o.AvMidPerS = Math.Max(Math.Max(o.Ve / Phi - vcMid, 0.0) / (fyt * r.D), RcCore.AciBeam.AvMinPerS(r.B, r.Fc, fyt))
        o.SMid = Math.Max(50.0, Math.Min(av / o.AvMidPerS, Math.Min(r.D / 2.0, 300.0)))
        o.Governing = If(o.G_Crush > 0.0, "Vs > 0.66 sqrt(f'c) bw d", If(av / o.AvConfPerS < 50.0, "stirrup spacing < 50 mm", "ok"))
        Return o
    End Function

    'concrete (+ fibre) shear contribution, N
    Private Function VcOf(r As BeamGroupResult, fib As BeamFibre, fctk As Double, fFtuk As Double) As Double
        If fib Is Nothing OrElse fib.Params Is Nothing Then Return RcCore.AciBeam.Vc(r.B, r.D, r.Fc)
        Return RcCore.SfrcBeam.VRdF(r.B, r.D, r.AsBot, r.Fc, fctk, fFtuk, 1.5)
    End Function
End Module
