Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports FemRcModel
Imports RcCore

'PCR-17 (EX2_SPEC v2 §5, §5.4): design -> cost (TL, ÇŞİDB 2026, RcCore.PriceSet = same source as Ö1) and normalised constraints (g <= 0 feasible),
'Ö1 multiplicative penalty F (1 + T)^2 (RcCore.Penalty). No pre-design class (Q9: arm A only).
Public Class Ex2Design
    Public Columns As New Dictionary(Of String, ColumnDef)     'FemRcModel.ColumnDef: B, H (mm; H in-plane), Rho, Fc, BarDia, RhoDerived
    Public Beams As New Dictionary(Of String, BeamDef)         'FemRcModel.BeamDef: B, H, Fc, Fibre (Nothing = plain)
End Class

Public Class Ex2Result
    Public Cost As CostBreakdown
    Public CostBeams, CostColumns As Double
    Public T As Double
    Public Fpen As Double
    Public Violations As New List(Of Tuple(Of String, Double))
    Public ColRes As New Dictionary(Of String, ColumnGravityResult)
    Public BeamRes As New Dictionary(Of String, BeamGravityResult)
    Public BeamShr As New Dictionary(Of String, BeamShearGravityResult)
    Public Analysis As Analysis2D
    Public ConcreteM3, FormworkM2, LongKg, TieKg, FibreKg As Double
    'beam items (Q21 marginal-cost table): TL and kg of the beams only
    Public BeamConcreteTL, BeamLongTL, BeamStirTL, BeamFormTL, BeamFibreTL As Double
    Public BeamLongKg, BeamStirKg As Double
    Public StoreyQ As Double()                 'ACI 6.6.4.4.1 Q per storey (index 1..N); Nothing when QCheck = False
    Public NonSway As Boolean()                'storey classified non-sway (Q <= 0.05) -> ACI 6.2.5(b)(c) limit for in-plane column slenderness
    Public Stability As StoreyStability        'PCR-22: Q, delta_s, gravity drift, M_s; Nothing when QCheck = False
    Public ReadOnly Property SteelKg As Double
        Get
            Return LongKg + TieKg
        End Get
    End Property
End Class

Public Class Ex2Options
    'PCR-25 (2026-11-17, post-hoc; ACI 318M-14 6.4.2 / R6.4.2 for beams, 6.6.2.2 for columns; the L <= 0.75D exemption 6.4.3.2 is for two-way slab systems only,
    'CODE_PROVISIONS_EVIDENCE A59): live-load patterns (24 combinations of RcCombos2D.PatternCombos) are part of the design envelope of beams AND columns.
    'Default True since PCR-25 (campaign2). False = the pre-registered campaign (2026-11-16) single-pattern envelope (decision [K b], superseded).
    Public Patterned As Boolean = True
    'PCR-24 (2026-11-17, post-hoc; ETABS V2-a BULGU-2): beam axial tension in flexure (ACI 22.2 equilibrium) and in beam shear (ACI 22.5.7.1). Default True since
    'PCR-24; False = the pre-registered campaign (axial force of beams neglected).
    Public BeamAxial As Boolean = True
    'PCR-23 item 3 (2026-11-17): constraint g8 (beam flexural D/C <= 1 of the provided steel) and the consistency raise of the plain AciBeam.RequiredAs
    '(transition zone). Default True since campaign 2; False = campaign 1 (so that the PCR-23 SFRC fix alone can be regressed bit-for-bit on the plain path).
    Public FlexCheck As Boolean = True
    'PCR-26 (2026-11-18, IA decision "A" on the deflection disagreement of the referees): ACI 318M-14 Table 24.2.2 l/480 (part of the total deflection after
    'attachment of non-structural elements likely to be damaged) as constraint g5, computed by Deflection2D.Report (unchanged since the report-only version).
    'Default False so that campaign-1/2 rules and all earlier tests stay bit-identical; campaign 3 runs with --deflcheck 1.
    Public DeflCheck As Boolean = False
    'PCR-26b (IA 2026-11-19, "Ie env"): with DeflCheck, g5 is evaluated under the ACI 6.4.2 live-load patterns with the envelope Ie
    '(Deflection2D.PatternedEnvelope; conservative, >= the all-loaded Report); needs Patterned = True. False = PCR-26 all-loaded g5 (campaign 3).
    Public DeflPatterned As Boolean = False
    Public QCheck As Boolean = True            'ACI 6.6.4.3(b): storey Q from a unit lateral load; Q > 0.05 -> sway slenderness limit 22 for that storey

    'options of the pre-registered campaign (ex2_campaign_bin, 2026-11-16): no patterns, no beam axial force (PCR-23 alone keeps the plain path bit-identical)
    Public Shared Function Campaign1() As Ex2Options
        Return New Ex2Options With {.Patterned = False, .BeamAxial = False, .FlexCheck = False}
    End Function
End Class

Public Module Ex2Evaluator
    Public Const SteelDensity As Double = 7850.0       'kg/m3
    Public Const StirrupCover As Double = 45.0         'mm to stirrup centreline (Ö1)
    Public Const HookLen As Double = 12.0              'x tie diameter, per hook (Ö1 assumption)
    Public Const BarDia As Double = 20.0               'column bar diameter (Ö1)
    Public Const RhoSteps As Integer = 12              'column rho grid 1 .. 4 % in 0.25 % steps (PCR-8, Q7)
    Public Const G5PatternName As String = "g5 beam deflection after attachment <= l/480 (ACI 24.2, Table 24.2.2; live-load patterns, Ie envelope)"
    Public Const DcTol As Double = 0.000001           'g8 tolerance on the beam flexural D/C (fixed-point iterations)

    Public Function RhoOfIdx(i As Integer) As Double
        Return 0.01 + 0.0025 * i
    End Function

    Private Function KgPerM(areaMm2 As Double) As Double
        Return areaMm2 * 1.0E-6 * SteelDensity
    End Function

    'PCR-8 (gravity form): smallest grid rho with P-M D/C <= 1 (bisection; D/C non-increasing in rho at fixed demands; no strong-column pass: Chapter 18 not applied)
    Private Function DeriveRho(a As Analysis2D, g As String, c As ColumnDef, ns As StoreyStability) As Integer
        Dim ok = Function(i As Integer) ColumnDesignGravity.CheckGroup(a, g, c.B, c.H, c.Fc, RhoOfIdx(i), c.BarDia, False, ns).DcrMax <= 1.0
        If ok(0) Then Return 0
        If Not ok(RhoSteps) Then Return RhoSteps
        Dim lo As Integer = 0, hi As Integer = RhoSteps
        While hi - lo > 1
            Dim mid As Integer = (lo + hi) \ 2
            If ok(mid) Then hi = mid Else lo = mid
        End While
        Return hi
    End Function

    Public Function Evaluate(spec As FrameSpec2D, d As Ex2Design, prices As PriceSet, shear As IBeamShear, Optional opts As Ex2Options = Nothing) As Ex2Result
        If shear Is Nothing Then Throw New ArgumentNullException(NameOf(shear))
        If opts Is Nothing Then opts = New Ex2Options()
        Dim r As New Ex2Result
        Dim defs As New Dictionary(Of String, RcSectionDef)
        For Each kv In d.Columns : defs(kv.Key) = New RcSectionDef With {.B = kv.Value.B, .H = kv.Value.H, .Fc = kv.Value.Fc} : Next
        For Each kv In d.Beams : defs(kv.Key) = New RcSectionDef With {.B = kv.Value.B, .H = kv.Value.H, .Fc = kv.Value.Fc} : Next
        Dim a = RcCombos2D.Analyze(spec, defs, False, opts.Patterned)
        r.Analysis = a
        Dim m = a.Frame.Model
        If opts.QCheck Then
            r.Stability = Stability2D.Assess(spec, defs, a)
            r.StoreyQ = r.Stability.Q
            r.NonSway = r.Stability.NonSway
        End If

        For Each g In a.Frame.BeamGroups
            Dim b = d.Beams(g)
            r.BeamRes(g) = BeamDesignGravity.DesignGroup(a, g, b.B, b.H, b.Fc, b.Fibre, opts.BeamAxial, opts.FlexCheck)
            r.BeamShr(g) = BeamShearGravity.DesignGroup(a, r.BeamRes(g), b.Fibre, shear)
        Next
        For Each g In a.Frame.ColumnGroups
            Dim c = d.Columns(g)
            If c.RhoDerived Then c.Rho = RhoOfIdx(DeriveRho(a, g, c, r.Stability))
            r.ColRes(g) = ColumnDesignGravity.CheckGroup(a, g, c.B, c.H, c.Fc, c.Rho, c.BarDia, True, r.Stability)
        Next

        'cost (spec §5.4): concrete b h L and longitudinal bars over the axis length (group-continuous bars, no laps); formwork beam (b + 2h) L, column 2 (b + h) L;
        'stirrups / ties at one spacing over the member (no confinement zones, no joint ties); fibre = dose x beam concrete volume x price
        Dim tieKgPerM As Double = KgPerM(Math.PI * ColumnDesignGravity.TieDia ^ 2 / 4.0)
        Dim stirKgPerM As Double = KgPerM(Math.PI * BeamShearGravity.StirrupDia ^ 2 / 4.0)
        For ei As Integer = 0 To m.Elements.Count - 1
            Dim e = m.Elements(ei)
            Dim L As Double = e.Length
            If a.Frame.IsColumn(ei) Then
                Dim c = d.Columns(e.Group), cr = r.ColRes(e.Group)
                Dim vol As Double = c.B * c.H * 1.0E-6 * L
                Dim form As Double = 2.0 * (c.B + c.H) * 0.001 * L
                Dim longKg As Double = KgPerM(cr.AsTotal) * L
                Dim tieKg As Double = (L * 1000.0 / cr.S + 1.0) * cr.TieLenPerSet * 0.001 * tieKgPerM
                Dim cb = Cost.Member(prices, c.Fc, vol, form, tieKg, longKg, 0.0, 0.0)
                r.Cost.Concrete += cb.Concrete : r.Cost.Rebar += cb.Rebar : r.Cost.Formwork += cb.Formwork
                r.CostColumns += cb.Total
                r.ConcreteM3 += vol : r.FormworkM2 += form : r.LongKg += longKg : r.TieKg += tieKg
            Else
                Dim b = d.Beams(e.Group), br = r.BeamRes(e.Group), bs = r.BeamShr(e.Group)
                Dim vol As Double = b.B * b.H * 1.0E-6 * L
                Dim form As Double = (b.B + 2.0 * b.H) * 0.001 * L
                Dim longKg As Double = KgPerM(br.AsTop + br.AsBot) * L
                Dim setLen As Double = (2.0 * (b.B - 2.0 * StirrupCover) + 2.0 * (b.H - 2.0 * StirrupCover) + 2.0 * HookLen * BeamShearGravity.StirrupDia) * 0.001
                Dim stirKg As Double = (L * 1000.0 / bs.S + 1.0) * setLen * stirKgPerM
                Dim dose As Double = If(b.Fibre IsNot Nothing AndAlso b.Fibre.Params IsNot Nothing, b.Fibre.DoseKgM3, 0.0)
                If Double.IsInfinity(longKg) Then longKg = 0.0                       'section too small: steel not priced, the design is infeasible (g3)
                Dim cb = Cost.Member(prices, b.Fc, vol, form, stirKg, longKg, dose, vol)
                r.Cost.Concrete += cb.Concrete : r.Cost.Rebar += cb.Rebar : r.Cost.Formwork += cb.Formwork : r.Cost.Fibre += cb.Fibre
                r.CostBeams += cb.Total
                r.BeamConcreteTL += cb.Concrete : r.BeamFormTL += cb.Formwork : r.BeamFibreTL += cb.Fibre
                r.BeamLongTL += longKg * prices.RebarPerKg(16.0) : r.BeamStirTL += stirKg * prices.RebarPerKg(12.0)
                r.BeamLongKg += longKg : r.BeamStirKg += stirKg
                r.ConcreteM3 += vol : r.FormworkM2 += form : r.LongKg += longKg : r.TieKg += stirKg : r.FibreKg += dose * vol
            End If
        Next

        'constraints: violation of a type = max over groups of max(0, g); T = sum over the types (Ö1 Penalty)
        Dim viol As New List(Of IEnumerable(Of Double)), names As New List(Of String)
        Dim add = Sub(nm As String, vals As IEnumerable(Of Double))
                      names.Add(nm) : viol.Add(vals.ToList())
                  End Sub
        Dim cl = r.ColRes.Values
        add("g1 col P-M D/C (ACI 22.4, 6.6.4.5)", cl.Select(Function(x) x.DcrMax - 1.0))
        add("g6 col 2nd-order M <= 1.4 x 1st-order M (ACI 6.2.6)", cl.Select(Function(x) x.Ratio626 / ColumnDesignGravity.Ratio626Limit - 1.0))     'PCR-20
        add("g7 storey Q <= 1/3 (delta_s <= 1.5; ACI 6.6.4.6.2(a))", If(r.Stability Is Nothing, New Double() {-1.0}, r.Stability.Q.Skip(1).Select(Function(q) q / Stability2D.QMaxMethodA - 1.0)))     'PCR-22
        add("g2 col rho >= 1 % (ACI 10.6.1.1)", cl.Select(Function(x) x.G_RhoLow))
        add("g2 col rho <= 4 % (grid bound)", cl.Select(Function(x) x.G_RhoHigh))
        add("col shear Vs <= 0.66 sqrt(fc) b d (ACI 22.5.1.2)", cl.Select(Function(x) x.G_Crush))
        add("col tie spacing >= 50", cl.Select(Function(x) 50.0 / Math.Max(x.S, 1.0) - 1.0))
        Dim bl = r.BeamRes.Values
        add("g3 beam section capacity (As finite)", bl.Select(Function(x) If(x.SectionOk, -1.0, 1.0)))
        add("g3 beam eps_t >= 0.004 (ACI 9.3.3.1)", bl.Select(Function(x) If(x.EpsTOk, -1.0, 1.0)))
        'PCR-23 item 3 / PCR-25: flexural D/C of the provided steel under the full design envelope (patterns, axial tension) <= 1; by construction <= 1 for
        'finite As with eps_t >= 0.004 (safety check against a silent design error, tolerance DcTol). Not evaluated where g3 is already violated (section too
        'small, or eps_t < 0.004 where AciBeam.RequiredAs assumes yielding steel and the D/C can exceed 1): no double penalty, and the plain path of the
        'campaign-1 rules stays bit-identical (PCR-23 regression; g8 itself is part of the campaign-2 rules, Ex2Options.FlexCheck)
        If opts.FlexCheck Then add("g8 beam flexure D/C <= 1 (ACI 9.5.1; envelope incl. ACI 6.4.2 patterns)", bl.Select(Function(x) If(x.SectionOk AndAlso x.EpsTOk, Math.Min(x.DcFlex - (1.0 + DcTol), 10.0), -1.0)))
        add("g4 beam shear Vs <= 0.66 sqrt(fc) bw d (ACI 22.5.1.2)", r.BeamShr.Values.Select(Function(x) x.G_Crush))
        add("g4 beam stirrup spacing >= 50", r.BeamShr.Values.Select(Function(x) 50.0 / Math.Max(x.S, 1.0) - 1.0))
        'ACI Table 9.3.1.1 (G_Depth) is NOT a constraint (IA 2026-11-16 servis decision (a)); PCR-26 (IA 2026-11-18, option A): g5 = ACI 24.2 l/480 deflection after
        'attachment (Deflection2D.Report) when opts.DeflCheck. Not evaluated where g3 is already violated (As infinite / eps_t < 0.004: no double penalty, no NaN).
        If opts.DeflCheck Then
            Dim sectionsOk As Boolean = bl.All(Function(x) x.SectionOk AndAlso x.EpsTOk)
            Dim vals As IEnumerable(Of Double)
            If sectionsOk Then
                If opts.DeflPatterned AndAlso opts.Patterned Then
                    vals = Deflection2D.PatternedEnvelope(spec, d, r).Select(Function(q) If(Double.IsNaN(q) OrElse Double.IsInfinity(q), 10.0, Math.Min(q - 1.0, 10.0))).ToList()
                Else
                    vals = Deflection2D.Report(spec, d, r).Select(Function(rw) If(Double.IsNaN(rw.DeltaAfter) OrElse Double.IsInfinity(rw.DeltaAfter), 10.0, Math.Min(rw.DeltaAfter / rw.LimAfter480 - 1.0, 10.0))).ToList()
                End If
            Else
                vals = New Double() {-1.0}
            End If
            add(If(opts.DeflPatterned AndAlso opts.Patterned, G5PatternName, "g5 beam deflection after attachment <= l/480 (ACI 24.2, Table 24.2.2)"), vals)
        End If
        For i As Integer = 0 To viol.Count - 1
            r.Violations.Add(Tuple.Create(names(i), Penalty.GroupViolation(viol(i))))
        Next
        r.T = Penalty.Total(viol)
        r.Fpen = Penalty.Apply(r.Cost.Total, r.T)
        Return r
    End Function
End Module
