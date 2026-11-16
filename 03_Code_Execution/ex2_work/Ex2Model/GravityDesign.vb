Imports System
Imports System.Collections.Generic
Imports FemCore
Imports FemRcModel
Imports RcCore

'PCR-15 (EX2_SPEC v2 §5): gravity-regime member design, ACI 318M-14 without Chapter 18 (no capacity design, no confinement zones, no joint checks, no TBDY).
'New classes; the Ö1 classes (BeamDesign, BeamShear, ColumnDesign.CheckGroup, ColumnShear) are NOT called because they carry TBDY / Chapter 18 rules.
'Only side-effect-free helpers of the frozen DLLs are reused: RcCore.AciBeam, RcCore.AciColumn, RcCore.SfrcBeam, FemRcModel.ColumnDesign.{MakeSection, BarCount,
'PerimeterCounts, ColCurve, DeltaNs, DcrWithEffects}. Units N, mm, MPa inside; FEM forces kN, m.

Public Class BeamGravityResult
    Public Group As String
    Public B, H, D, Fc As Double
    Public MuTop, MuBot As Double              'N.mm envelope over the stations of the clear span (hogging / sagging), main combinations
    Public AsTop, AsBot, AsMin As Double       'mm2 provided = max(required, As,min) (ACI 9.6.1.2)
    Public EpsTOk As Boolean                   'ACI 9.3.3.1: eps_t >= 0.004 at the provided steel (both faces)
    Public SectionOk As Boolean                'required As finite (section can carry Mu)
    Public G_Depth As Double                   'ACI Table 9.3.1.1: max over the group of (l / divisor) / h - 1
    Public Governing As String
End Class

Public Module BeamDesignGravity
    Public Const Fy As Double = 420.0
    Public Const CoverToCentroid As Double = 60.0      'mm (Ö1 value, ACI 20.6.1.3.1: cover 40 + stirrup 10 + half of a 20 mm bar; MEMORY §3f)

    Private Function Required(Mu As Double, b As Double, h As Double, d As Double, fc As Double, fib As BeamFibre) As Double
        If Mu <= 0.0 Then Return 0.0
        If fib Is Nothing OrElse fib.Params Is Nothing Then Return AciBeam.RequiredAs(Mu, b, d, fc, Fy)
        'SFRC (fib MC2010 sigma-w, RcCore.SfrcBeam; same bisection as Ö1 BeamDesign.Required, re-written here: the Ö1 function is private)
        Dim lo As Double = 0.0, hi As Double = 0.05 * b * d
        If SfrcBeam.Flexure(b, h, d, hi, fc, Fy, fib.Params, h).PhiMn < Mu Then Return Double.PositiveInfinity
        Dim lcs As Double = h
        For i As Integer = 0 To 59
            Dim mid As Double = 0.5 * (lo + hi)
            Dim r = SfrcBeam.Flexure(b, h, d, Math.Max(mid, 1.0), fc, Fy, fib.Params, lcs)
            lcs = Math.Max(h - r.C, 0.2 * h)
            If r.PhiMn >= Mu Then hi = mid Else lo = mid
        Next
        Return hi
    End Function

    Private Function EpsT(b As Double, h As Double, d As Double, As_ As Double, fc As Double, fib As BeamFibre) As Double
        If fib Is Nothing OrElse fib.Params Is Nothing Then Return AciBeam.Flexure(b, d, As_, fc, Fy).EpsT
        Return SfrcBeam.Flexure(b, h, d, As_, fc, Fy, fib.Params, 0.7 * h).EpsT
    End Function

    'ACI Table 9.3.1.1 divisor of a beam: one end continuous (end bays) 18.5, both ends continuous (interior bay) 21; l = axis span (spec §5.1 g5, conservative)
    Public Function DepthDivisor(a As Analysis2D, ei As Integer) As Double
        Dim m = a.Frame.Model, e = m.Elements(ei)
        Dim xMax As Double = a.Frame.Spec.XOf(a.Frame.Spec.NCols - 1)
        Dim x1 As Double = Math.Min(m.Nodes(e.NI).X, m.Nodes(e.NJ).X), x2 As Double = Math.Max(m.Nodes(e.NI).X, m.Nodes(e.NJ).X)
        Return If(x1 < 0.001 OrElse x2 > xMax - 0.001, 18.5, 21.0)
    End Function

    Public Function DesignGroup(a As Analysis2D, group As String, b As Double, h As Double, fc As Double, fib As BeamFibre) As BeamGravityResult
        Dim r As New BeamGravityResult With {.Group = group, .B = b, .H = h, .D = h - CoverToCentroid, .Fc = fc, .G_Depth = Double.NegativeInfinity}
        Dim m = a.Frame.Model
        For ei As Integer = 0 To m.Elements.Count - 1
            Dim e = m.Elements(ei)
            If e.Group <> group Then Continue For
            For ci As Integer = 0 To a.Combos.Count - 1
                If Not a.Combos(ci).Main Then Continue For
                For Each s In a.Station(ci)(ei)
                    Dim m3 As Double = s(5) * 1.0E6          'sagging positive
                    r.MuTop = Math.Max(r.MuTop, -m3) : r.MuBot = Math.Max(r.MuBot, m3)
                Next
            Next
            r.G_Depth = Math.Max(r.G_Depth, (e.Length * 1000.0 / DepthDivisor(a, ei)) / h - 1.0)
        Next
        r.AsMin = AciBeam.MinAs(b, r.D, fc, Fy)                                  'ACI 9.6.1.2 (A09)
        Dim reqTop As Double = Required(r.MuTop, b, h, r.D, fc, fib)
        Dim reqBot As Double = Required(r.MuBot, b, h, r.D, fc, fib)
        r.AsTop = Math.Max(reqTop, r.AsMin)
        r.AsBot = Math.Max(reqBot, r.AsMin)                                      'no TBDY bottom >= 50 % top, no rho <= 2 % (spec §5.1 g3)
        r.SectionOk = Not (Double.IsInfinity(r.AsTop) OrElse Double.IsInfinity(r.AsBot))
        r.EpsTOk = False
        If r.SectionOk Then
            r.EpsTOk = EpsT(b, h, r.D, r.AsTop, fc, fib) >= 0.004 AndAlso EpsT(b, h, r.D, r.AsBot, fc, fib) >= 0.004
        End If
        r.Governing = If(Not r.SectionOk, "section too small", If(Not r.EpsTOk, "epsT < 0.004", "ok"))
        Return r
    End Function
End Module

'---------------------------------------------------------------------------------------------------------------------------------------------
'Beam shear model interface (spec §5.2). IA decisions 2026-11-16: Q17 increment rule (main series), Ö1 format (gammaC = 1.5 directly instead of Vc) as a
'flagged sensitivity option; Q18 fR taken as characteristic (declared), fR x 0.8 sensitivity via the fR scale (FibreTable.MakeBeamFibre fRScale).
Public Interface IBeamShear
    ReadOnly Property Name As String
    'concrete (+ fibre) shear strength Vc,f in N (nominal; phi applied by the caller). asl: tension steel for rho_l (mm2)
    Function ConcreteShear(bw As Double, d As Double, fc As Double, asl As Double, fib As BeamFibre) As Double
End Interface

'Plain RC: ACI 318M-14 22.5.5.1 Vc = 0.17 lambda sqrt(f'c) bw d (RcCore.AciBeam.Vc)
Public Class AciPlainShear
    Implements IBeamShear
    Public ReadOnly Property Name As String Implements IBeamShear.Name
        Get
            Return "ACI 22.5.5.1 plain"
        End Get
    End Property
    Public Function ConcreteShear(bw As Double, d As Double, fc As Double, asl As Double, fib As BeamFibre) As Double Implements IBeamShear.ConcreteShear
        If fib IsNot Nothing AndAlso fib.Params IsNot Nothing Then Throw New InvalidOperationException("AciPlainShear called for an SFRC beam")
        Return AciBeam.Vc(bw, d, fc)
    End Function
End Class

'Q17 main series (IA 2026-11-16): Vc,f = Vc,ACI + max(0, VRd,F(fFtuk; gammaC = 1.0) - VRd,F(fFtuk = 0; gammaC = 1.0))
'VRd,F: fib MC2010 Eq. 7.7-4 with the Eq. 7.7-5 lower bound in BOTH terms (RcCore.SfrcBeam.VRdF); fctk = fctk,min (Ö1), fFtuk at wu = 1.5 mm (Ö1 FFtukShear);
'fR as tabulated x fRScale (Q18: characteristic, declared). Plain beam (dose 0): Vc,ACI exactly (continuity). sigma_cp = 0 (beam). phi = 0.75 by the caller.
'rho_l from the bottom (sagging) steel AsBot, as Ö1 BeamDesign.VcOf [conservative at the hogging supports; EX2_CODE_LOG].
Public Class FibreShearIncrementQ17
    Implements IBeamShear
    Public Const GammaC As Double = 1.0
    Public ReadOnly Property Name As String Implements IBeamShear.Name
        Get
            Return "Q17 increment: Vc,ACI + max(0, VRdF(gc=1) - VRdF(0, gc=1))"
        End Get
    End Property
    Public Shared Function Increment(bw As Double, d As Double, fc As Double, asl As Double, fib As BeamFibre) As Double
        If fib Is Nothing OrElse fib.Params Is Nothing Then Return 0.0
        Dim fctk As Double = SfrcBeam.FctkMin(fc), fFtuk As Double = SfrcBeam.FFtukShear(fib.Params)
        Return Math.Max(0.0, SfrcBeam.VRdF(bw, d, asl, fc, fctk, fFtuk, GammaC) - SfrcBeam.VRdF(bw, d, asl, fc, fctk, 0.0, GammaC))
    End Function
    Public Function ConcreteShear(bw As Double, d As Double, fc As Double, asl As Double, fib As BeamFibre) As Double Implements IBeamShear.ConcreteShear
        Return AciBeam.Vc(bw, d, fc) + Increment(bw, d, fc, asl, fib)
    End Function
End Class

'Sensitivity only (flag --shearformat o1): Ö1 BeamDesign.VcOf format, Vc,f = VRd,F(fFtuk; gammaC = 1.5) replacing Vc for SFRC beams, then phi = 0.75
'(double safety). Plain beams: Vc,ACI. Ö1 itself is frozen; this is a re-statement of its formula for the deterministic re-evaluation (spec §5.2, §11.3).
Public Class FibreShearO1Format
    Implements IBeamShear
    Public Const GammaC As Double = 1.5
    Public ReadOnly Property Name As String Implements IBeamShear.Name
        Get
            Return "O1 format: VRdF(gc=1.5) replaces Vc (sensitivity)"
        End Get
    End Property
    Public Function ConcreteShear(bw As Double, d As Double, fc As Double, asl As Double, fib As BeamFibre) As Double Implements IBeamShear.ConcreteShear
        If fib Is Nothing OrElse fib.Params Is Nothing Then Return AciBeam.Vc(bw, d, fc)
        Return SfrcBeam.VRdF(bw, d, asl, fc, SfrcBeam.FctkMin(fc), SfrcBeam.FFtukShear(fib.Params), GammaC)
    End Function
End Class

Public Class BeamShearGravityResult
    Public Group As String
    Public Vu As Double                        'N, largest |V2| at the critical sections d from the element ends (ACI 9.4.3.2; main combinations)
    Public VcPlain, Vcf As Double              'N: plain ACI Vc (Av,min threshold, spec §5.2) and Vc,f of the shear model
    Public DeltaVF As Double                   'N: Vcf - VcPlain (fibre contribution credited)
    Public AvPerS As Double                    'required Av/s, mm2/mm
    Public S As Double                         'chosen spacing, mm (one spacing over the whole beam)
    Public AvMinApplied As Boolean             'ACI 9.6.3.1: Vu > 0.5 phi Vc
    Public G_Crush As Double                   'Vs,req / (0.66 sqrt(f'c) bw d) - 1 (ACI 22.5.1.2)
    Public Governing As String
End Class

Public Module BeamShearGravity
    Public Const Phi As Double = 0.75           'ACI 21.2.1(b)
    Public Const Fyt As Double = 420.0
    Public Const StirrupDia As Double = 10.0    'mm, two legs (Ö1 assumption)

    'V2 (kN) at distance x (m) from the start of the clear span, linear interpolation between stations (exact for the uniform beam loads of Ex2)
    Public Function ShearAt(a As Analysis2D, ci As Integer, ei As Integer, x As Double) As Double
        Dim xs = a.StationX(ei), st = a.Station(ci)(ei)
        If x <= xs(0) Then Return st(0)(1)
        For k As Integer = 0 To xs.Length - 2
            If x <= xs(k + 1) Then
                Dim t As Double = (x - xs(k)) / (xs(k + 1) - xs(k))
                Return st(k)(1) + t * (st(k + 1)(1) - st(k)(1))
            End If
        Next
        Return st(xs.Length - 1)(1)
    End Function

    'Critical section (IA 2026-11-16, = V3 rule): ACI 9.4.3.2, Vu at d from the support face; conditions (a)-(c) hold (beam framing monolithically into a column,
    'loads on the top surface, no concentrated load). Analysis model without rigid end zones (ETABS model the same): the support face is taken at the node,
    'i.e. the critical section is at x = d and x = L - d of the element [EX2_CODE_LOG AÇIK: column half-depth not added].
    Public Function DesignGroup(a As Analysis2D, r As BeamGravityResult, fib As BeamFibre, model As IBeamShear) As BeamShearGravityResult
        Dim o As New BeamShearGravityResult With {.Group = r.Group}
        Dim m = a.Frame.Model
        Dim dm As Double = r.D / 1000.0
        For ei As Integer = 0 To m.Elements.Count - 1
            If m.Elements(ei).Group <> r.Group Then Continue For
            Dim xs = a.StationX(ei), lc As Double = xs(xs.Length - 1)
            For ci As Integer = 0 To a.Combos.Count - 1
                If Not a.Combos(ci).Main Then Continue For
                Dim v1 As Double = Math.Abs(ShearAt(a, ci, ei, Math.Min(dm, 0.5 * lc)))
                Dim v2 As Double = Math.Abs(ShearAt(a, ci, ei, Math.Max(lc - dm, 0.5 * lc)))
                o.Vu = Math.Max(o.Vu, Math.Max(v1, v2) * 1000.0)
            Next
        Next
        o.VcPlain = AciBeam.Vc(r.B, r.D, r.Fc)
        o.Vcf = model.ConcreteShear(r.B, r.D, r.Fc, r.AsBot, fib)
        o.DeltaVF = o.Vcf - o.VcPlain
        Dim vsReq As Double = Math.Max(o.Vu / Phi - o.Vcf, 0.0)
        o.AvPerS = vsReq / (Fyt * r.D)
        o.AvMinApplied = o.Vu > 0.5 * Phi * o.VcPlain           'spec §5.2: threshold with the plain ACI Vc for plain and SFRC beams (ACI 26.4.2.2(d) dose not met)
        If o.AvMinApplied Then o.AvPerS = Math.Max(o.AvPerS, AciBeam.AvMinPerS(r.B, r.Fc, Fyt))
        Dim av As Double = 2.0 * Math.PI * StirrupDia * StirrupDia / 4.0
        Dim sMax As Double = AciBeam.MaxStirrupSpacing(vsReq, r.B, r.D, r.Fc)       'ACI 9.7.6.2.2 (A19): d/2 <= 600, or d/4 <= 300 when Vs > 0.33 sqrt(f'c) bw d
        o.S = If(o.AvPerS > 0.0, Math.Min(av / o.AvPerS, sMax), sMax)               'Vu <= 0.5 phi Vc: nominal stirrups at sMax [ASSUMPTION, EX2_CODE_LOG]
        o.G_Crush = vsReq / AciBeam.VsMax(r.B, r.D, r.Fc) - 1.0
        o.Governing = If(o.G_Crush > 0.0, "Vs > 0.66 sqrt(f'c) bw d", If(o.S < 50.0, "stirrup spacing < 50 mm", "ok"))
        Return o
    End Function
End Module

'---------------------------------------------------------------------------------------------------------------------------------------------
'Column check: uniaxial in-plane P-M (M3, depth H) with ACI 6.6.4.5 delta_ns (non-sway, k = 1, lu = clear height, Cm = 1: Ö1 PCR-2 function) and
'e_min = 15.24 + 0.03 h in each direction separately (PCR-1; out-of-plane: e_min only, braced, k = 1), radial D/C (PCR-3). Ties ACI 25.7.2.1 + gravity shear.
Public Class ColumnGravityResult
    Public Group As String
    Public B, H, Fc, Rho, BarDia As Double
    Public NBars As Integer
    Public AsTotal As Double
    Public DcrMax As Double
    Public NdMax, NdMin As Double              'N, compression +
    Public G_RhoLow, G_RhoHigh As Double       'ACI 10.6.1.1 (A22): 1 % .. grid 4 % (bar rounding can exceed the grid value)
    'ties
    Public Vu As Double                        'N, in-plane shear (local 2)
    Public Vc As Double                        'N, ACI 22.5.6.1 with the smallest compression
    Public S As Double                         'tie spacing, mm
    Public Legs(1) As Integer
    Public TieLenPerSet As Double              'mm
    Public G_Crush As Double
    Public Governing As String
End Class

Public Module ColumnDesignGravity
    Public Const Fy As Double = 420.0
    Public Const Fyt As Double = 420.0
    Public Const Phi As Double = 0.75
    Public Const TieDia As Double = 10.0
    Public Const TieCover As Double = 45.0      'mm to tie centreline (Ö1 ColumnShear.TieCover)
    Public Const Cover As Double = 60.0         'mm to bar centroid (Ö1 ColumnDesign.Cover)

    Public Function CheckGroup(a As Analysis2D, group As String, b As Double, h As Double, fc As Double, rho As Double, dia As Double, Optional withTies As Boolean = True) As ColumnGravityResult
        Dim r As New ColumnGravityResult With {.Group = group, .B = b, .H = h, .Fc = fc, .Rho = rho, .BarDia = dia}
        r.NBars = ColumnDesign.BarCount(b, h, rho, dia)
        r.AsTotal = r.NBars * Math.PI * dia * dia / 4.0
        Dim sx = ColumnDesign.MakeSection(b, h, fc, r.NBars, dia)                 'M3: in-plane, depth H (spec §1.1)
        Dim sy = ColumnDesign.MakeSection(h, b, fc, r.NBars, dia, False, True)    'M2: out-of-plane, depth B
        Dim cvx As New ColumnDesign.ColCurve(sx), cvy As New ColumnDesign.ColCurve(sy)
        Dim m = a.Frame.Model
        Dim ndMin As Double = Double.MaxValue
        For ei As Integer = 0 To m.Elements.Count - 1
            Dim el = m.Elements(ei)
            If el.Group <> group Then Continue For
            Dim lu As Double = (el.Length - el.OffsetI - el.OffsetJ) * 1000.0
            For ci As Integer = 0 To a.Combos.Count - 1
                If Not a.Combos(ci).Main Then Continue For
                Dim f = a.Face(ci)(ei)
                Dim sus As Double = a.SustainedAxial(ci)(ei) * 1000.0
                r.Vu = Math.Max(r.Vu, Math.Max(Math.Abs(f(1)), Math.Abs(f(7))) * 1000.0)
                For endI As Integer = 0 To 1
                    Dim pu As Double = If(endI = 0, f(0), -f(6)) * 1000.0
                    Dim m2 As Double = Math.Abs(If(endI = 0, f(4), f(10))) * 1.0E6
                    Dim m3 As Double = Math.Abs(If(endI = 0, f(5), f(11))) * 1.0E6
                    r.NdMax = Math.Max(r.NdMax, pu) : ndMin = Math.Min(ndMin, pu)
                    Dim beta As Double = If(pu > 0.0, Math.Abs(sus) / pu, 0.0)
                    Dim d3 As Double = DeltaNsNonSway(fc, h, b, lu, pu, beta, -f(5) * 1.0E6, f(11) * 1.0E6)
                    Dim d2 As Double = DeltaNsNonSway(fc, b, h, lu, pu, beta, -f(4) * 1.0E6, f(10) * 1.0E6)
                    r.DcrMax = Math.Max(r.DcrMax, ColumnDesign.DcrWithEffects(cvx, cvy, pu, m3, m2, d3, d2))
                Next
            Next
        Next
        r.NdMin = If(ndMin = Double.MaxValue, 0.0, ndMin)
        Dim actual As Double = r.AsTotal / (b * h)
        r.G_RhoLow = 0.01 / actual - 1.0
        r.G_RhoHigh = actual / 0.04 - 1.0
        If withTies Then Ties(r)
        r.Governing = If(r.DcrMax > 1.0, "D/C > 1", If(r.G_RhoHigh > 0.0, "rho > 4 %", If(r.G_Crush > 0.0, "Vs > 0.66 sqrt(f'c) b d", "ok")))
        Return r
    End Function

    'ACI 6.2.5 for a column braced against sidesway (spec §5.1: non-sway frame assumed, k = 1, r = 0.3 h): slenderness neglected if lu/r <= 34 + 12 (M1/M2) (6.2.5b)
    'and <= 40 (6.2.5c), M1/M2 negative for single curvature (ACI 318M-14 sign; PDF 66 / 64). The frozen ColumnDesign.DeltaNs uses the sway limit 22 (6.2.5a) and
    'same-sign end moments = single curvature, so ACI M1/M2 = -ratio. Above the non-sway limit (always >= 22) the frozen magnifier is used unchanged (Cm = 1, PCR-2).
    Public Function NonSwayLimit(mA As Double, mB As Double) As Double
        Dim big As Double = If(Math.Abs(mA) >= Math.Abs(mB), mA, mB), small As Double = If(Math.Abs(mA) >= Math.Abs(mB), mB, mA)
        Dim ratio As Double = If(Math.Abs(big) < 1.0E-9, 1.0, small / big)          'frozen DeltaNs convention (+1 = single curvature)
        Return Math.Min(34.0 - 12.0 * ratio, 40.0)
    End Function

    Public Function DeltaNsNonSway(fc As Double, depth As Double, width As Double, lu As Double, pu As Double, betaDns As Double, mA As Double, mB As Double) As Double
        If pu <= 0.0 Then Return 1.0
        If lu / (0.3 * depth) <= NonSwayLimit(mA, mB) Then Return 1.0
        Return ColumnDesign.DeltaNs(fc, depth, width, lu, pu, betaDns, mA, mB)
    End Function

    'Ties (ACI 318M-14, read 2026-11-16, CODE_PROVISIONS_EVIDENCE A43-A46): 25.7.2.1(b) s <= min(16 db, 48 dtie, least dimension) (PDF 440 / 438);
    '10.6.2.1 Av,min where Vu > 0.5 phi Vc (PDF 158 / 156; A25); 10.7.6.5.2 when shear reinforcement is required: s <= d/2 <= 600, or d/4 <= 300 if
    'Vs > 0.33 sqrt(f'c) bw d (Table 10.7.6.5.2, PDF 164 / 162); Vc ACI 22.5.6.1 (compression; A23) or 22.5.7.1 (tension, >= 0; A24), Nu = smallest
    'compression of the main combinations. Legs: every perimeter bar supported (Ö1 rule, conservative against ACI 25.7.2.3) [ASSUMPTION].
    Private Sub Ties(r As ColumnGravityResult)
        Dim nB As Integer, nH As Integer
        ColumnDesign.PerimeterCounts(r.B, r.H, r.NBars, nB, nH)
        r.Legs(0) = Math.Max(2, nB) : r.Legs(1) = Math.Max(2, nH)        'legs(0): parallel to the in-plane shear (X), they cross the B side
        Dim dEff As Double = r.H - Cover, bw As Double = r.B, ag As Double = r.B * r.H
        If r.NdMin >= 0.0 Then
            r.Vc = 0.17 * (1.0 + r.NdMin / (14.0 * ag)) * Math.Sqrt(r.Fc) * bw * dEff
        Else
            r.Vc = Math.Max(0.0, 0.17 * (1.0 + r.NdMin / (3.5 * ag)) * Math.Sqrt(r.Fc) * bw * dEff)
        End If
        Dim vsReq As Double = Math.Max(r.Vu / Phi - r.Vc, 0.0)
        Dim avPerS As Double = vsReq / (Fyt * dEff)
        Dim shearReinf As Boolean = r.Vu > 0.5 * Phi * r.Vc
        If shearReinf Then avPerS = Math.Max(avPerS, AciBeam.AvMinPerS(bw, r.Fc, Fyt))
        Dim sLim As Double = Math.Min(Math.Min(16.0 * r.BarDia, 48.0 * TieDia), Math.Min(r.B, r.H))
        If shearReinf Then sLim = Math.Min(sLim, AciBeam.MaxStirrupSpacing(vsReq, bw, dEff, r.Fc))     'Table 10.7.6.5.2 = Table 9.7.6.2.2 values (nonprestressed)
        Dim av As Double = r.Legs(0) * Math.PI * TieDia * TieDia / 4.0
        r.S = If(avPerS > 0.0, Math.Min(sLim, av / avPerS), sLim)
        r.G_Crush = vsReq / AciBeam.VsMax(bw, dEff, r.Fc) - 1.0
        r.TieLenPerSet = r.Legs(0) * (r.H - 2.0 * TieCover) + r.Legs(1) * (r.B - 2.0 * TieCover) + 2.0 * 12.0 * TieDia
    End Sub
End Module
