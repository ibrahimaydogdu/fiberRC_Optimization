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
    Public G_Depth As Double                   'ACI Table 9.3.1.1: max over the group of (l / divisor) / h - 1 (REPORT ONLY since IA 2026-11-16 servis (a); not a constraint)
    Public DcFlex As Double                    'max over faces of flexural D/C with the provided steel (PCR-23: same fixed-point capacity as Required; PCR-24: axial tension); constraint g8
    Public NtMax As Double                     'N, largest axial TENSION of the group's elements over the design combinations (PCR-24; 0 when Ex2Options.BeamAxial = False or all compression)
    Public Governing As String
End Class

Public Module BeamDesignGravity
    Public Const Fy As Double = 420.0
    Public Const CoverToCentroid As Double = 60.0      'mm (Ö1 value, ACI 20.6.1.3.1: cover 40 + stirrup 10 + half of a 20 mm bar; MEMORY §3f)

    'PCR-23 (2026-11-17, post-hoc bug fix; Review_Round_Ex2_sonuc_ic A1 / _dis §3): the SFRC capacity is ALWAYS the fixed point lcs = max(h - c(lcs), 0.2 h)
    'of the steel area being judged (FlexFixed). Before PCR-23 the bisection in Required judged each trial with the lcs of the PREVIOUS trial (stale lcs;
    'path dependent, unsafe in ~2 % of cases, D/C up to 1.027) and EpsT used a third assumption lcs = 0.7 h. Plain RC: unchanged closed forms (AciBeam).
    Public Const LcsTol As Double = 0.000001             'mm, fixed-point convergence of lcs
    Public Const LcsMaxIter As Integer = 60

    'flexural result of a singly reinforced section at its own consistent lcs. lcsStart: warm start (0 = cold start at lcs = h). If the iteration does not
    'converge (oscillation), the result with the smaller phiMn of the last two iterates is returned (conservative). ByRef lcsOut: the lcs used for the result.
    Public Function FlexFixed(b As Double, h As Double, d As Double, As_ As Double, fc As Double, fib As BeamFibre, Optional lcsStart As Double = 0.0) As FlexureResult
        Dim dummy As Double
        Return FlexFixed(b, h, d, As_, fc, fib, lcsStart, dummy)
    End Function

    Public Function FlexFixed(b As Double, h As Double, d As Double, As_ As Double, fc As Double, fib As BeamFibre, lcsStart As Double, ByRef lcsOut As Double) As FlexureResult
        If fib Is Nothing OrElse fib.Params Is Nothing Then
            lcsOut = 0.0
            Return AciBeam.Flexure(b, d, As_, fc, Fy)
        End If
        Dim a As Double = Math.Max(As_, 1.0)
        Dim lcs As Double = If(lcsStart > 0.0, lcsStart, h)
        Dim r As FlexureResult = SfrcBeam.Flexure(b, h, d, a, fc, Fy, fib.Params, lcs)
        For i As Integer = 1 To LcsMaxIter
            Dim nl As Double = Math.Max(h - r.C, 0.2 * h)
            If Math.Abs(nl - lcs) <= LcsTol Then lcsOut = lcs : Return r
            Dim rPrev As FlexureResult = r, lPrev As Double = lcs
            lcs = nl
            r = SfrcBeam.Flexure(b, h, d, a, fc, Fy, fib.Params, lcs)
            If i = LcsMaxIter Then                                          'not converged: conservative choice of the last two iterates
                If rPrev.PhiMn < r.PhiMn Then lcsOut = lPrev : Return rPrev
            End If
        Next
        lcsOut = lcs
        Return r
    End Function

    'PCR-23: bending steel for a pure moment Ms (N.mm). SFRC: bisection whose acceptance test is the consistent capacity (warm-started fixed point),
    'then a cold-start verification (the same FlexFixed call as PhiMn / DcFlex) raises the result until phiMn(As) >= Ms holds exactly.
    'consistentPlain (Ex2Options.FlexCheck, campaign 2): the frozen AciBeam.RequiredAs stops its phi iteration after 50 steps; in the transition zone
    '(0.004 <= eps_t < 0.005) it can return As with phiMn(As) up to ~0.04 % below Ms (found by g8 on 2026-11-17). The result is raised by RaiseUntil
    '(geometric relative steps 1e-12, 2e-12, 4e-12, ...: overshoot <= 2 x the shortfall) until AciBeam.Flexure(As).PhiMn >= Ms. False = campaign-1 behaviour.
    Private Function RequiredBending(Ms As Double, b As Double, h As Double, d As Double, fc As Double, fib As BeamFibre, Optional consistentPlain As Boolean = False) As Double
        If Ms <= 0.0 Then Return 0.0
        If fib Is Nothing OrElse fib.Params Is Nothing Then
            Dim asP As Double = AciBeam.RequiredAs(Ms, b, d, fc, Fy)
            If consistentPlain AndAlso Not Double.IsInfinity(asP) Then asP = RaiseUntil(asP, Function(a) AciBeam.Flexure(b, d, a, fc, Fy).PhiMn >= Ms)
            Return asP
        End If
        Dim lo As Double = 0.0, hi As Double = 0.05 * b * d
        Dim lcsW As Double = 0.0
        If FlexFixed(b, h, d, hi, fc, fib, 0.0, lcsW).PhiMn < Ms Then Return Double.PositiveInfinity
        For i As Integer = 0 To 47
            Dim mid As Double = 0.5 * (lo + hi)
            Dim lcsMid As Double
            Dim r = FlexFixed(b, h, d, mid, fc, fib, lcsW, lcsMid)
            If r.PhiMn >= Ms Then hi = mid : lcsW = lcsMid Else lo = mid
        Next
        Return RaiseUntil(hi, Function(a) FlexFixed(b, h, d, a, fc, fib).PhiMn >= Ms)
    End Function

    'smallest raise of As0 (relative steps 1e-12 * 2^k from As0, k = 0..40, i.e. at most x 2.1) such that ok(As) holds; As0 itself if it already holds.
    'If ok never holds (phiMn decreasing in As in the transition zone), the last value is returned and g3 / g8 decide feasibility.
    Public Function RaiseUntil(As0 As Double, ok As Func(Of Double, Boolean)) As Double
        If ok(As0) Then Return As0
        Dim a As Double = As0, stp As Double = 0.000000000001
        For k As Integer = 0 To 40
            a = As0 * (1.0 + stp)
            If ok(a) Then Return a
            stp *= 2.0
        Next
        Return a
    End Function

    'PCR-24 (2026-11-17; ETABS V2-a BULGU-2): beam axial TENSION Nt (N, >= 0) with flexure, ACI 22.2 equilibrium with yielding tension steel:
    'Pn = Nt / phi, Mn = Mu / phi about the centroid -> As = As_bend(Ms) + Nt / (phi fy), Ms = Mu - Nt (d - h/2) (moment about the tension steel).
    'Compression is ignored (conservative). Nt = 0 -> exactly the PCR-23 bending result. The fibre stress block is the one of the bending part (same strain profile).
    Public Function AxialEcc(h As Double, d As Double) As Double
        Return d - h / 2.0
    End Function

    Private Function Required(Mu As Double, Nt As Double, b As Double, h As Double, d As Double, fc As Double, fib As BeamFibre, Optional consistentPlain As Boolean = False) As Double
        If Nt <= 0.0 Then Return RequiredBending(Mu, b, h, d, fc, fib, consistentPlain)
        Dim ms As Double = Math.Max(Mu - Nt * AxialEcc(h, d), 0.0)
        Dim asb As Double = RequiredBending(ms, b, h, d, fc, fib, consistentPlain)
        If Double.IsInfinity(asb) Then Return asb
        Dim phi As Double = If(asb > 0.0, FlexFixed(b, h, d, asb, fc, fib).Phi, 0.9)
        Return asb + Nt / (phi * Fy)
    End Function

    'bending part of a provided steel area under axial tension: asb = As - Nt / (phi(asb) fy) (fixed point; Nt = 0 -> As). Pn = Nt / phi (nominal equilibrium).
    Public Function BendingSteel(b As Double, h As Double, d As Double, As_ As Double, Nt As Double, fc As Double, fib As BeamFibre) As Double
        If Nt <= 0.0 Then Return As_
        Dim asb As Double = As_ - Nt / (0.9 * Fy)
        For i As Integer = 0 To 49
            If asb <= 0.0 Then Return asb
            Dim nb As Double = As_ - Nt / (FlexFixed(b, h, d, asb, fc, fib).Phi * Fy)
            If Math.Abs(nb - asb) <= 0.000000001 Then Return nb
            asb = nb
        Next
        Return asb
    End Function

    'flexural D/C of one face (PCR-23/24): Ms / phiMn(bending part); infinite if the steel cannot even carry the tension
    Public Function FaceDc(Mu As Double, Nt As Double, b As Double, h As Double, d As Double, As_ As Double, fc As Double, fib As BeamFibre) As Double
        If Mu <= 0.0 AndAlso Nt <= 0.0 Then Return 0.0
        Dim asb As Double = BendingSteel(b, h, d, As_, Nt, fc, fib)
        If asb <= 0.0 Then Return Double.PositiveInfinity
        Dim ms As Double = If(Nt > 0.0, Math.Max(Mu - Nt * AxialEcc(h, d), 0.0), Mu)
        If ms <= 0.0 Then Return 0.0
        Return ms / PhiMn(b, h, d, asb, fc, fib)
    End Function

    'net tensile strain at nominal strength (ACI 9.3.3.1) of one face: strain of the bending part at its consistent lcs (PCR-23) under Pn = Nt / phi (PCR-24)
    Private Function EpsT(b As Double, h As Double, d As Double, As_ As Double, Nt As Double, fc As Double, fib As BeamFibre) As Double
        Dim asb As Double = BendingSteel(b, h, d, As_, Nt, fc, fib)
        If asb <= 0.0 Then Return Double.PositiveInfinity
        Return FlexFixed(b, h, d, asb, fc, fib).EpsT
    End Function

    'ACI Table 9.3.1.1 divisor of a beam: one end continuous (end bays) 18.5, both ends continuous (interior bay) 21; l = axis span (spec §5.1 g5, conservative)
    Public Function DepthDivisor(a As Analysis2D, ei As Integer) As Double
        Dim m = a.Frame.Model, e = m.Elements(ei)
        Dim xMax As Double = a.Frame.Spec.XOf(a.Frame.Spec.NCols - 1)
        Dim x1 As Double = Math.Min(m.Nodes(e.NI).X, m.Nodes(e.NJ).X), x2 As Double = Math.Max(m.Nodes(e.NI).X, m.Nodes(e.NJ).X)
        Return If(x1 < 0.001 OrElse x2 > xMax - 0.001, 18.5, 21.0)
    End Function

    'largest axial tension (N, >= 0) of a beam group over the design (Main) combinations; station N of FemStatic is tension-positive (checked in T23)
    Public Function GroupTension(a As Analysis2D, group As String) As Double
        Dim m = a.Frame.Model, nt As Double = 0.0
        For ei As Integer = 0 To m.Elements.Count - 1
            If m.Elements(ei).Group <> group Then Continue For
            For ci As Integer = 0 To a.Combos.Count - 1
                If Not a.Combos(ci).Main Then Continue For
                For Each s In a.Station(ci)(ei) : nt = Math.Max(nt, s(0) * 1000.0) : Next
            Next
        Next
        Return nt
    End Function

    'withAxial (PCR-24, Ex2Options.BeamAxial): the group's largest axial tension is paired with the moment envelope of each face (conservative: As grows with Nt)
    Public Function DesignGroup(a As Analysis2D, group As String, b As Double, h As Double, fc As Double, fib As BeamFibre, Optional withAxial As Boolean = False, Optional consistentPlain As Boolean = False) As BeamGravityResult
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
        r.NtMax = If(withAxial, GroupTension(a, group), 0.0)
        r.AsMin = AciBeam.MinAs(b, r.D, fc, Fy)                                  'ACI 9.6.1.2 (A09)
        Dim reqTop As Double = Required(r.MuTop, r.NtMax, b, h, r.D, fc, fib, consistentPlain)
        Dim reqBot As Double = Required(r.MuBot, r.NtMax, b, h, r.D, fc, fib, consistentPlain)
        r.AsTop = Math.Max(reqTop, r.AsMin)
        r.AsBot = Math.Max(reqBot, r.AsMin)                                      'no TBDY bottom >= 50 % top, no rho <= 2 % (spec §5.1 g3)
        r.SectionOk = Not (Double.IsInfinity(r.AsTop) OrElse Double.IsInfinity(r.AsBot))
        r.EpsTOk = False
        If r.SectionOk Then
            r.EpsTOk = EpsT(b, h, r.D, r.AsTop, r.NtMax, fc, fib) >= 0.004 AndAlso EpsT(b, h, r.D, r.AsBot, r.NtMax, fc, fib) >= 0.004
        End If
        If r.SectionOk Then
            r.DcFlex = Math.Max(FaceDc(r.MuTop, r.NtMax, b, h, r.D, r.AsTop, fc, fib), FaceDc(r.MuBot, r.NtMax, b, h, r.D, r.AsBot, fc, fib))
        Else
            r.DcFlex = Double.PositiveInfinity
        End If
        r.Governing = If(Not r.SectionOk, "section too small", If(Not r.EpsTOk, "epsT < 0.004", "ok"))
        Return r
    End Function

    'design flexural strength of a singly reinforced section (N.mm); SFRC: consistent fixed-point lcs (PCR-23, cold start; the same function judges Required)
    Public Function PhiMn(b As Double, h As Double, d As Double, As_ As Double, fc As Double, fib As BeamFibre) As Double
        Return FlexFixed(b, h, d, As_, fc, fib).PhiMn
    End Function

    'PCR-23 unit-test hook: required steel of one face (Mu, Nt in N.mm / N)
    Public Function RequiredFace(Mu As Double, Nt As Double, b As Double, h As Double, d As Double, fc As Double, fib As BeamFibre, Optional consistentPlain As Boolean = False) As Double
        Return Required(Mu, Nt, b, h, d, fc, fib, consistentPlain)
    End Function
End Module

'---------------------------------------------------------------------------------------------------------------------------------------------
'Beam shear models (spec §5.2). IA decisions 2026-11-16 and the second-package review (Review_Round_Ex2_oneriler_ic/_dis):
' * ACI Vc: the SIMPLE equation 22.5.5.1, Vc = 0.17 lambda sqrt(f'c) bw d (lambda = 1, sqrt(f'c) <= 8.3 MPa; RcCore.AciBeam.Vc; A12, A15). The detailed
'   Table 22.5.5.1 form is not used (spec g4 cites 22.5; ETABS default to be confirmed in V2-a B2).
' * fib MC2010 VRd,F: Eq. (7.7-4), lower bound Eq. (7.7-5) (fib Bulletin 56 Vol. 2 draft, PDF 179-180 / printed 160-161; CODE_PROVISIONS_EVIDENCE B16, B17;
'   re-read 2026-11-16). No rho_l <= 0.02 cap in that clause (not found); Asl "extends >= lbd + d beyond the section" (group-continuous bars, spec §5.4).
' * rho_l = AsTop / (bw d): tension steel at the critical (hogging) support section (main series and MC2010-local variant). The Ö1 variant keeps AsBot (Ö1 code).
' * sigma_cp = 0 in every VRd,F term (beam axial force neglected; explicit argument 0.0).
' * Validity: fibre credit only if fR3/fR1 > 0.5 (MC2010 Vol. 1 Eq. 5.6-3, B03b; SfrcParams.CanSubstituteRebar). fR1k/fLk > 0.4 (5.6-2) NOT checked: fLk not available.
'   Not satisfied -> dVF = 0 (main and MC2010-local). All C30 grid points satisfy it (fR3 > fR1).
Public Interface IBeamShear
    ReadOnly Property Name As String
    'concrete (+ fibre) shear strength Vc,f in N such that the check is Vu <= phi (Vc,f + Vs); asTop / asBot: provided steel (mm2)
    Function ConcreteShear(bw As Double, d As Double, fc As Double, asTop As Double, asBot As Double, fib As BeamFibre) As Double
End Interface

Public Module FibreShearCommon
    Public Const SigmaCp As Double = 0.0
    Public Function FibreValid(fib As BeamFibre) As Boolean
        Return fib IsNot Nothing AndAlso fib.Params IsNot Nothing AndAlso fib.Params.CanSubstituteRebar()
    End Function
    'dVF = max(0, VRd,F(fFtuk) - VRd,F(0)), both with the same gammaC, Eq. 7.7-5 lower bound in both terms
    Public Function Increment(bw As Double, d As Double, fc As Double, asl As Double, fib As BeamFibre, gammaC As Double) As Double
        If Not FibreValid(fib) Then Return 0.0
        Dim fctk As Double = SfrcBeam.FctkMin(fc), fFtuk As Double = SfrcBeam.FFtukShear(fib.Params)
        Return Math.Max(0.0, SfrcBeam.VRdF(bw, d, asl, fc, fctk, fFtuk, gammaC, SigmaCp) - SfrcBeam.VRdF(bw, d, asl, fc, fctk, 0.0, gammaC, SigmaCp))
    End Function
    'Eq. 7.7-5 lower bound active in the dose-0 term (report: the fibre must first lift the base above vmin)
    Public Function VminBindsAtDose0(bw As Double, d As Double, fc As Double, asl As Double, gammaC As Double) As Boolean
        Dim k As Double = Math.Min(2.0, 1.0 + Math.Sqrt(200.0 / d))
        Dim vmin As Double = 0.035 * Math.Pow(k, 1.5) * Math.Sqrt(fc)
        Return SfrcBeam.VRdF(bw, d, asl, fc, SfrcBeam.FctkMin(fc), 0.0, gammaC, SigmaCp) <= vmin * bw * d * (1.0 + 1.0E-12)
    End Function
End Module

'Plain RC: ACI 318M-14 22.5.5.1
Public Class AciPlainShear
    Implements IBeamShear
    Public ReadOnly Property Name As String Implements IBeamShear.Name
        Get
            Return "ACI 22.5.5.1 plain"
        End Get
    End Property
    Public Function ConcreteShear(bw As Double, d As Double, fc As Double, asTop As Double, asBot As Double, fib As BeamFibre) As Double Implements IBeamShear.ConcreteShear
        If fib IsNot Nothing AndAlso fib.Params IsNot Nothing Then Throw New InvalidOperationException("AciPlainShear called for an SFRC beam")
        Return AciBeam.Vc(bw, d, fc)
    End Function
End Class

'Q17 main series (IA 2026-11-16): Vc,f = Vc,ACI(22.5.5.1) + max(0, VRd,F(fFtuk; gc = 1.0) - VRd,F(0; gc = 1.0)), rho_l = AsTop, sigma_cp = 0;
'Vu <= phi (Vc,f + Vs), phi = 0.75. Dose 0 or invalid fibre: Vc,ACI exactly.
Public Class FibreShearIncrementQ17
    Implements IBeamShear
    Public Const GammaC As Double = 1.0
    Public ReadOnly Property Name As String Implements IBeamShear.Name
        Get
            Return "Q17 increment: Vc,ACI + max(0; VRdF(gc=1) - VRdF(0; gc=1)); rho_l=AsTop"
        End Get
    End Property
    Public Shared Function Increment(bw As Double, d As Double, fc As Double, asl As Double, fib As BeamFibre) As Double
        Return FibreShearCommon.Increment(bw, d, fc, asl, fib, GammaC)
    End Function
    Public Function ConcreteShear(bw As Double, d As Double, fc As Double, asTop As Double, asBot As Double, fib As BeamFibre) As Double Implements IBeamShear.ConcreteShear
        Return AciBeam.Vc(bw, d, fc) + Increment(bw, d, fc, asTop, fib)
    End Function
End Class

'Deterministic variant "MC2010-local" (--shearformat mc2010): Vu <= phi (Vc,ACI + Vs) + dVF(gc = 1.5): the fibre term carries only its own code factor.
'Written as Vc,f = Vc,ACI + dVF(gc = 1.5) / phi so that the common check Vu <= phi (Vc,f + Vs) is identical.
Public Class FibreShearMc2010Local
    Implements IBeamShear
    Public Const GammaC As Double = 1.5
    Public ReadOnly Property Name As String Implements IBeamShear.Name
        Get
            Return "MC2010-local: phi(Vc,ACI + Vs) + dVF(gc=1.5); rho_l=AsTop"
        End Get
    End Property
    Public Function ConcreteShear(bw As Double, d As Double, fc As Double, asTop As Double, asBot As Double, fib As BeamFibre) As Double Implements IBeamShear.ConcreteShear
        Return AciBeam.Vc(bw, d, fc) + FibreShearCommon.Increment(bw, d, fc, asTop, fib, GammaC) / BeamShearGravity.Phi
    End Function
End Class

'Deterministic variant "Ö1" (--shearformat o1): the Ö1 formula one-to-one (FemRcModel BeamDesign.VcOf + Ex1Evaluator): SFRC beam Vc,f =
'SfrcBeam.VRdF(B, D, AsBot, fc, FctkMin(fc), FFtukShear(params), 1.5) [sigma_cp default 0], i.e. VRd,F REPLACES Vc (format AND summation rule differ
'from the main series), then phi = 0.75; plain beam: AciBeam.Vc. No fR3/fR1 validity check (Ö1 has none). Other Ö1 shear rules (Mpr capacity shear,
'Vc = 0 zones, 2h confinement, middle-zone d/2 <= 300) are regime rules and are NOT carried over.
Public Class FibreShearO1Format
    Implements IBeamShear
    Public Const GammaC As Double = 1.5
    Public ReadOnly Property Name As String Implements IBeamShear.Name
        Get
            Return "O1 formula: VRdF(gc=1.5; rho_l=AsBot) replaces Vc"
        End Get
    End Property
    Public Function ConcreteShear(bw As Double, d As Double, fc As Double, asTop As Double, asBot As Double, fib As BeamFibre) As Double Implements IBeamShear.ConcreteShear
        If fib Is Nothing OrElse fib.Params Is Nothing Then Return AciBeam.Vc(bw, d, fc)
        Return SfrcBeam.VRdF(bw, d, asBot, fc, SfrcBeam.FctkMin(fc), SfrcBeam.FFtukShear(fib.Params), GammaC)
    End Function
End Class

Public Module ShearModels
    Public Function ByName(format As String, useFibre As Boolean) As IBeamShear
        If Not useFibre Then Return New AciPlainShear()
        Select Case format.ToLowerInvariant()
            Case "delta" : Return New FibreShearIncrementQ17()
            Case "mc2010" : Return New FibreShearMc2010Local()
            Case "o1" : Return New FibreShearO1Format()
        End Select
        Throw New ArgumentException("--shearformat must be delta, mc2010 or o1")
    End Function
End Module

Public Class BeamShearGravityResult
    Public Group As String
    Public Vu As Double                        'N, largest |V2| at the critical sections d from the element ends (ACI 9.4.3.2; main combinations)
    Public VcPlain, Vcf As Double              'N: plain ACI Vc (Av,min threshold, spec §5.2) and Vc,f of the shear model
    Public DeltaVF As Double                   'N: Vcf - VcPlain (fibre contribution credited)
    Public AxialFactor As Double = 1.0         'PCR-24: ACI 22.5.7.1 factor (1 + Nu / (3.5 Ag)) >= 0 with the group's largest tension (1 = no tension)
    Public RatioVcf As Double                  'Vc,f / Vc,ACI
    Public RatioMc0 As Double                  'VRd,F(0; gc = 1; rho_l = AsTop) / Vc,ACI: direction of the MC2010-vs-ACI base difference (Critic Q17-2)
    Public VminBinds As Boolean                'Eq. 7.7-5 active in the dose-0 term (gc = 1)
    Public FibreValid As Boolean               'fR3/fR1 > 0.5 (only meaningful for SFRC beams)
    Public AvPerS As Double                    'required Av/s, mm2/mm = max(AvSStrength, AvSMin)
    Public AvSStrength As Double               'Av/s required by strength, (Vu/phi - Vc,f) / (fyt d), mm2/mm (V2-a B2 comparison quantity)
    Public AvSMin As Double                    'Av,min/s (ACI 9.6.3.3) when applied, else 0, mm2/mm
    Public AvSProvided As Double               'Av / s of the chosen spacing, mm2/mm
    Public SMax As Double                      'ACI 9.7.6.2.2 spacing limit, mm
    Public S As Double                         'chosen spacing, mm (one spacing over the whole beam)
    Public AvMinApplied As Boolean             'ACI 9.6.3.1: Vu > 0.5 phi Vc
    'what sets the SPACING (2026-11-16 relabel): "strength" / "Avmin" (spacing from the required Av/s), "smax(Av/s by strength)" or "smax(Av/s by Avmin)"
    '(spacing capped by smax although the required Av/s comes from strength or Av,min), "smax-nominal" (no shear reinforcement required)
    Public StirrupGov As String
    Public DcShear As Double                   'Vu / (phi (Vc,f + Vs,provided)), Vs,provided = min(Av fyt d / s, 0.66 sqrt(f'c) bw d)
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

    'largest |V| (N) at the critical sections x = d and x = L - d of the group's elements over the design combinations (ACI 9.4.3.2; face = node, no rigid zone)
    Public Function DemandAtD(a As Analysis2D, group As String, d As Double) As Double
        Dim m = a.Frame.Model, vu As Double = 0.0
        Dim dm As Double = d / 1000.0
        For ei As Integer = 0 To m.Elements.Count - 1
            If m.Elements(ei).Group <> group Then Continue For
            Dim xs = a.StationX(ei), lc As Double = xs(xs.Length - 1)
            For ci As Integer = 0 To a.Combos.Count - 1
                If Not a.Combos(ci).Main Then Continue For
                Dim v1 As Double = Math.Abs(ShearAt(a, ci, ei, Math.Min(dm, 0.5 * lc)))
                Dim v2 As Double = Math.Abs(ShearAt(a, ci, ei, Math.Max(lc - dm, 0.5 * lc)))
                vu = Math.Max(vu, Math.Max(v1, v2) * 1000.0)
            Next
        Next
        Return vu
    End Function

    'Critical section (IA 2026-11-16, = V3 rule): ACI 9.4.3.2, Vu at d from the support face; conditions (a)-(c) hold (beam framing monolithically into a column,
    'loads on the top surface, no concentrated load). Analysis model without rigid end zones (ETABS model the same): the support face is taken at the node,
    'i.e. the critical section is at x = d and x = L - d of the element [EX2_CODE_LOG AÇIK-14: column half-depth not added].
    Public Function DesignGroup(a As Analysis2D, r As BeamGravityResult, fib As BeamFibre, model As IBeamShear) As BeamShearGravityResult
        Dim o As New BeamShearGravityResult With {.Group = r.Group}
        o.Vu = DemandAtD(a, r.Group, r.D)
        'PCR-24: ACI 318M-14 22.5.7.1 (axial tension; Nu negative for tension): Vc = 0.17 (1 + Nu / (3.5 Ag)) lambda sqrt(f'c) bw d >= 0, with the group's
        'largest tension r.NtMax (0 when BeamAxial is off -> factor exactly 1). The reduction dVc = Vc,22.5.5.1 (1 - factor) is subtracted from Vc,f of every
        'shear format (the MC2010 fibre increment dVF keeps sigma_cp = 0: the 0.15 sigma_cp term cancels in VRdF(fF) - VRdF(0) unless Eq. 7.7-5 binds).
        o.AxialFactor = Math.Max(0.0, 1.0 - r.NtMax / (3.5 * r.B * r.H))
        Dim vc0 As Double = AciBeam.Vc(r.B, r.D, r.Fc)
        Dim dVcAxial As Double = vc0 - vc0 * o.AxialFactor
        o.VcPlain = vc0 * o.AxialFactor
        o.Vcf = model.ConcreteShear(r.B, r.D, r.Fc, r.AsTop, r.AsBot, fib) - dVcAxial
        o.DeltaVF = o.Vcf - o.VcPlain
        o.RatioVcf = o.Vcf / o.VcPlain
        o.RatioMc0 = SfrcBeam.VRdF(r.B, r.D, r.AsTop, r.Fc, SfrcBeam.FctkMin(r.Fc), 0.0, 1.0, FibreShearCommon.SigmaCp) / o.VcPlain
        o.VminBinds = FibreShearCommon.VminBindsAtDose0(r.B, r.D, r.Fc, r.AsTop, 1.0)
        o.FibreValid = FibreShearCommon.FibreValid(fib)
        Dim vsReq As Double = Math.Max(o.Vu / Phi - o.Vcf, 0.0)
        Dim avsStrength As Double = vsReq / (Fyt * r.D)
        o.AvMinApplied = o.Vu > 0.5 * Phi * o.VcPlain           'spec §5.2: threshold with the plain ACI Vc for plain and SFRC beams (ACI 26.4.2.2(d) dose not met)
        Dim avsMin As Double = If(o.AvMinApplied, AciBeam.AvMinPerS(r.B, r.Fc, Fyt), 0.0)
        o.AvPerS = Math.Max(avsStrength, avsMin)
        o.AvSStrength = avsStrength : o.AvSMin = avsMin
        Dim av As Double = 2.0 * Math.PI * StirrupDia * StirrupDia / 4.0
        Dim sMax As Double = AciBeam.MaxStirrupSpacing(vsReq, r.B, r.D, r.Fc)       'ACI 9.7.6.2.2 (A19): d/2 <= 600, or d/4 <= 300 when Vs > 0.33 sqrt(f'c) bw d
        o.SMax = sMax
        o.S = If(o.AvPerS > 0.0, Math.Min(av / o.AvPerS, sMax), sMax)               'Vu <= 0.5 phi Vc: nominal stirrups at sMax [ASSUMPTION, EX2_CODE_LOG]
        o.AvSProvided = av / o.S
        If o.AvPerS <= 0.0 Then
            o.StirrupGov = "smax-nominal"
        ElseIf av / o.AvPerS >= sMax Then
            o.StirrupGov = If(avsStrength >= avsMin, "smax(Av/s by strength)", "smax(Av/s by Avmin)")
        ElseIf avsStrength >= avsMin Then
            o.StirrupGov = "strength"
        Else
            o.StirrupGov = "Avmin"
        End If
        Dim vsProv As Double = Math.Min(av * Fyt * r.D / o.S, AciBeam.VsMax(r.B, r.D, r.Fc))
        o.DcShear = o.Vu / (Phi * (o.Vcf + vsProv))
        o.G_Crush = vsReq / AciBeam.VsMax(r.B, r.D, r.Fc) - 1.0
        o.Governing = If(o.G_Crush > 0.0, "Vs > 0.66 sqrt(f'c) bw d", If(o.S < 50.0, "stirrup spacing < 50 mm", "ok"))
        Return o
    End Function
End Module


'---------------------------------------------------------------------------------------------------------------------------------------------
'Column check: uniaxial in-plane P-M (M3, depth H) with ACI 6.6.4.5 delta_ns (k = 1, lu = clear height since PCR-21 (axis length before), Cm = 1: Ö1 PCR-2 function;
'sway storeys PCR-22: delta_s on M2s and no slenderness exemption), ACI 6.2.6 ratio (PCR-20) and
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
    'PCR-20/21/22 (2026-11-16)
    Public LuMin, LuMax As Double              'mm, clear height used for slenderness (PCR-21)
    Public Ratio626 As Double                  'max over ends / combinations / planes of (M incl. 2nd-order) / (1st-order M) (ACI 6.2.6, limit 1.4); e_min included in both
    Public Ratio626In, Ratio626Out As Double   'in-plane (M3) and out-of-plane (M2) parts
    Public Delta3Max, Delta2Max As Double      'largest delta_ns used in-plane / out-of-plane
    Public DeltaSMax As Double                 'largest delta_s applied to M2s (1 in non-sway storeys)
    Public AnySway As Boolean                  'group has an element in a sway storey (Q > 0.05)
End Class

Public Module ColumnDesignGravity
    Public Const Fy As Double = 420.0
    Public Const Fyt As Double = 420.0
    Public Const Phi As Double = 0.75
    Public Const TieDia As Double = 10.0
    Public Const TieCover As Double = 45.0      'mm to tie centreline (Ö1 ColumnShear.TieCover)
    Public Const Cover As Double = 60.0         'mm to bar centroid (Ö1 ColumnDesign.Cover)

    'PCR-21 (2026-11-16): clear height lu (m) of a column element by the Ö1 rule one-to-one (FemRcModel RcFrameBuilder column offsets = half of the deepest
    'beam framing into the end node at that level, 0 at the base): lu = L - h_b(below)/2 - h_b(above)/2. ACI 6.2.5.1 / 6.6.4.4 (lu clear length); the
    'storey Q (6.6.4.4.1) keeps lc = axis height 3.3 m. Interior storeys with equal beams: lu = 3.3 - h_b; storey 1: 3.3 - h_b/2.
    Public Function ClearHeight(fm As FrameModel2D, ei As Integer) As Double
        Dim m = fm.Model, e = m.Elements(ei)
        Return e.Length - HalfBeamDepth(fm, e.NI, e.Story - 1) - HalfBeamDepth(fm, e.NJ, e.Story)
    End Function

    Private Function HalfBeamDepth(fm As FrameModel2D, node As Integer, level As Integer) As Double
        If level < 1 Then Return 0.0
        Dim best As Double = 0.0
        For ej As Integer = 0 To fm.Model.Elements.Count - 1
            If fm.IsColumn(ej) Then Continue For
            Dim e = fm.Model.Elements(ej)
            If e.Story <> level Then Continue For
            If e.NI = node OrElse e.NJ = node Then best = Math.Max(best, e.Section.Depth)
        Next
        Return best / 2.0
    End Function

    'PCR-22: delta_ns WITHOUT the slenderness exemption (sway storeys: ACI 6.2.5(a) uses k lu / r <= 22 with the sway-frame k >= 1, which is not computed;
    'k = 1 would be unconservative, so the exemption is not used at all). Same formula as the frozen ColumnDesign.DeltaNs above its exemption
    '(EI = 0.4 Ec Ig / (1 + beta_dns), Pc = pi^2 EI / lu^2, Cm = 1, delta >= 1).
    Public Function DeltaNsNoExemption(fc As Double, depth As Double, width As Double, lu As Double, pu As Double, betaDns As Double) As Double
        If pu <= 0.0 Then Return 1.0
        Dim ig As Double = width * depth * depth * depth / 12.0
        Dim ei As Double = 0.4 * Aci.Ec(fc) * ig / (1.0 + Math.Min(Math.Max(betaDns, 0.0), 1.0))
        Dim pc As Double = Math.PI * Math.PI * ei / (lu * lu)
        Dim den As Double = 1.0 - pu / (0.75 * pc)
        If den <= 0.0 Then Return Double.PositiveInfinity
        Return Math.Max(1.0 / den, 1.0)
    End Function

    Public Const Ratio626Limit As Double = 1.4
    Public Const RatioCap As Double = 10.0     'finite value for the penalty when delta is infinite (buckling)

    'ACI 318M-14 6.2.6 ratio of one plane at one end: (delta_ns * max(|M_mag1|, Pu e_min)) / max(|M_1st|, Pu e_min), M_mag1 = M2ns + delta_s M2s
    Public Function Ratio626Of(delta As Double, mFirst As Double, mSwayMag As Double, puEmin As Double) As Double
        If Double.IsInfinity(delta) OrElse Double.IsNaN(delta) Then Return RatioCap
        Dim first As Double = Math.Max(mFirst, puEmin)
        If first <= 0.0 Then Return 1.0
        Return Math.Min(delta * Math.Max(mSwayMag, puEmin) / first, RatioCap)
    End Function

    Public Function CheckGroup(a As Analysis2D, group As String, b As Double, h As Double, fc As Double, rho As Double, dia As Double, Optional withTies As Boolean = True, Optional stab As StoreyStability = Nothing) As ColumnGravityResult
        'stab.NonSway(k): ACI 6.6.4.3(b) Q <= 0.05 for storey k (Stability2D) -> in-plane limit 6.2.5(b)(c). Sway storey (PCR-22): no exemption,
        'M = |M_ns + delta_s M_s| magnified by delta_ns (Cm = 1; conservative envelope of 6.6.4.6). stab Nothing (--qcheck 0) = all non-sway.
        'Out-of-plane: braced assumption (spec §5.1) -> always non-sway limit. lu = clear height (PCR-21). 6.2.6 ratio recorded (PCR-20; constraint g6).
        Dim r As New ColumnGravityResult With {.Group = group, .B = b, .H = h, .Fc = fc, .Rho = rho, .BarDia = dia, .LuMin = Double.MaxValue, .Ratio626 = 1.0, .Ratio626In = 1.0, .Ratio626Out = 1.0, .Delta3Max = 1.0, .Delta2Max = 1.0, .DeltaSMax = 1.0}
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
            Dim lu As Double = ClearHeight(a.Frame, ei) * 1000.0
            r.LuMin = Math.Min(r.LuMin, lu) : r.LuMax = Math.Max(r.LuMax, lu)
            Dim ns As Boolean = stab Is Nothing OrElse stab.NonSway(el.Story)
            Dim dS As Double = If(ns, 1.0, stab.DeltaS(el.Story))
            If Not ns Then r.AnySway = True : r.DeltaSMax = Math.Max(r.DeltaSMax, dS)
            For ci As Integer = 0 To a.Combos.Count - 1
                If Not a.Combos(ci).Main Then Continue For
                Dim f = a.Face(ci)(ei)
                Dim fs As Double() = If(Not ns AndAlso stab.SwayFace.Count > 0, stab.SwayFace(ci)(ei), Nothing)      'sway part M_s (Nothing -> M_s = 0)
                Dim sus As Double = a.SustainedAxial(ci)(ei) * 1000.0
                r.Vu = Math.Max(r.Vu, Math.Max(Math.Abs(f(1)), Math.Abs(f(7))) * 1000.0)
                For endI As Integer = 0 To 1
                    Dim i3 As Integer = If(endI = 0, 5, 11), i2 As Integer = If(endI = 0, 4, 10)
                    Dim pu As Double = If(endI = 0, f(0), -f(6)) * 1000.0
                    Dim m2 As Double = Math.Abs(f(i2)) * 1.0E6
                    Dim m3 As Double = Math.Abs(f(i3)) * 1.0E6
                    r.NdMax = Math.Max(r.NdMax, pu) : ndMin = Math.Min(ndMin, pu)
                    Dim beta As Double = If(pu > 0.0, Math.Abs(sus) / pu, 0.0)
                    Dim d3 As Double, m3use As Double
                    If ns Then
                        d3 = DeltaNsNonSway(fc, h, b, lu, pu, beta, -f(5) * 1.0E6, f(11) * 1.0E6)
                        m3use = m3
                    Else
                        Dim msw As Double = If(fs Is Nothing, 0.0, fs(i3)), mns As Double = f(i3) - msw
                        m3use = If(Double.IsInfinity(dS), Double.PositiveInfinity, Math.Abs(mns + dS * msw) * 1.0E6)     'ACI 6.6.4.6.1: M2 = M2ns + delta_s M2s
                        d3 = If(Double.IsInfinity(dS), Double.PositiveInfinity, DeltaNsNoExemption(fc, h, b, lu, pu, beta))
                    End If
                    Dim d2 As Double = DeltaNsNonSway(fc, b, h, lu, pu, beta, -f(4) * 1.0E6, f(10) * 1.0E6)
                    r.Delta3Max = Math.Max(r.Delta3Max, d3) : r.Delta2Max = Math.Max(r.Delta2Max, d2)
                    If Double.IsInfinity(m3use) Then
                        r.DcrMax = Math.Max(r.DcrMax, ColumnDesign.DcrCap)
                    Else
                        r.DcrMax = Math.Max(r.DcrMax, ColumnDesign.DcrWithEffects(cvx, cvy, pu, m3use, m2, d3, d2))
                    End If
                    'ACI 6.2.6 (PCR-20): e_min of DcrWithEffects (15.24 + 0.03 depth of the bending plane) in the first- and second-order moments
                    Dim e3 As Double = If(pu > 0.0, pu * (ColumnDesign.EminBase + 0.03 * h), 0.0), e2 As Double = If(pu > 0.0, pu * (ColumnDesign.EminBase + 0.03 * b), 0.0)
                    Dim rin As Double = If(Double.IsInfinity(m3use), RatioCap, Ratio626Of(d3, m3, m3use, e3))
                    Dim rout As Double = Ratio626Of(d2, m2, m2, e2)
                    r.Ratio626In = Math.Max(r.Ratio626In, rin) : r.Ratio626Out = Math.Max(r.Ratio626Out, rout)
                    r.Ratio626 = Math.Max(r.Ratio626, Math.Max(rin, rout))
                Next
            Next
        Next
        If r.LuMin = Double.MaxValue Then r.LuMin = 0.0
        r.NdMin = If(ndMin = Double.MaxValue, 0.0, ndMin)
        Dim actual As Double = r.AsTotal / (b * h)
        r.G_RhoLow = 0.01 / actual - 1.0
        r.G_RhoHigh = actual / 0.04 - 1.0
        If withTies Then Ties(r)
        r.Governing = If(r.DcrMax > 1.0, "D/C > 1", If(r.Ratio626 > Ratio626Limit, "2nd-order M > 1.4 x 1st-order (ACI 6.2.6)", If(r.G_RhoHigh > 0.0, "rho > 4 %", If(r.G_Crush > 0.0, "Vs > 0.66 sqrt(f'c) b d", "ok"))))
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
