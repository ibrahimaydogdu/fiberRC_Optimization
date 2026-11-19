Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports FemRcModel
Imports Ex2Model

'Deterministic tools of the Ex2 runner (second package, 2026-11-16). None of them optimises; results are information tables (spec §11.3) and
'are not reported before the protocol is approved. Output: CSV-like lines on stdout.
Module Tools
    ReadOnly ci As CultureInfo = CultureInfo.InvariantCulture
    ReadOnly DoseIdx As New Dictionary(Of Double, Integer) From {{0, 0}, {20, 1}, {25, 2}, {30, 3}, {35, 4}, {40, 5}}

    Function F2(v As Double) As String
        Return v.ToString("F2", ci)
    End Function
    Function F4(v As Double) As String
        Return v.ToString("F4", ci)
    End Function

    Function ReadX(path As String) As Integer()
        Dim line = File.ReadAllLines(path).First(Function(s) s.Trim() <> "" AndAlso Not s.TrimStart().StartsWith("#"))
        If line.StartsWith("x,") Then line = line.Substring(2)
        Return line.Split(","c).Select(Function(s) Integer.Parse(s.Trim())).ToArray()
    End Function

    Function NonFibre(r As Ex2Result) As Double
        Return r.Cost.Total - r.Cost.Fibre
    End Function

    '---------------------------------------------------------------------------------------------------------------------------------------
    Sub PrintResult(prob As Ex2Problem, x As Integer(), ev As Ex2Result)
        Console.WriteLine("x," & String.Join(",", x))
        Console.WriteLine("Fpen," & ev.Fpen.ToString("R", ci) & ",cost," & ev.Cost.Total.ToString("R", ci) & ",T," & ev.T.ToString("R", ci))
        Console.WriteLine("concrete," & F2(ev.Cost.Concrete) & ",rebar," & F2(ev.Cost.Rebar) & ",formwork," & F2(ev.Cost.Formwork) & ",fibreCost," & F2(ev.Cost.Fibre))
        Console.WriteLine("concreteM3," & F4(ev.ConcreteM3) & ",formworkM2," & ev.FormworkM2.ToString("F3", ci) & ",longKg," & F2(ev.LongKg) & ",tieKg," & F2(ev.TieKg))
        Console.WriteLine("beams_TL,total," & F2(ev.CostBeams) & ",concrete," & F2(ev.BeamConcreteTL) & ",long," & F2(ev.BeamLongTL) & ",stirrup," & F2(ev.BeamStirTL) & ",formwork," & F2(ev.BeamFormTL) & ",fibre," & F2(ev.BeamFibreTL))
        If ev.StoreyQ IsNot Nothing Then
            Console.WriteLine(StabilityLine(ev))
        Else
            Console.WriteLine("storeyQ,not computed (--qcheck 0: all storeys non-sway)")
        End If
        For Each v In ev.Violations : Console.WriteLine("g," & v.Item1.Replace(",", ";") & "," & v.Item2.ToString("R", ci)) : Next
        For Each kv In ev.ColRes
            Dim c = kv.Value
            Console.WriteLine("col," & kv.Key & ",b," & c.B & ",h," & c.H & ",rho," & F4(c.Rho) & ",nBars," & c.NBars & ",DC," & F4(c.DcrMax) & ",Nd_max_kN," & F2(c.NdMax / 1000.0) & ",Vu_kN," & F2(c.Vu / 1000.0) & ",Vc_kN," & F2(c.Vc / 1000.0) & ",s," & c.S.ToString("F1", ci) &
                              ",lu_mm," & c.LuMin.ToString("F0", ci) & If(c.LuMax > c.LuMin + 0.5, "-" & c.LuMax.ToString("F0", ci), "") & ",sway," & c.AnySway & ",deltaS," & F4(c.DeltaSMax) & ",delta_ns_in," & F4(c.Delta3Max) & ",delta_ns_out," & F4(c.Delta2Max) &
                              ",ACI6.2.6_ratio_in," & F4(c.Ratio626In) & ",ACI6.2.6_ratio_out," & F4(c.Ratio626Out) & ",ACI6.2.6_ratio/1.4," & F4(c.Ratio626 / ColumnDesignGravity.Ratio626Limit) & ",gov," & c.Governing)
        Next
        Dim dsg = prob.Decode(x)
        For Each kv In ev.BeamRes
            Dim b = kv.Value, s = ev.BeamShr(kv.Key)
            Dim bf = dsg.Beams(kv.Key).Fibre
            Dim fibTxt As String = If(bf IsNot Nothing AndAlso bf.Params IsNot Nothing, "dose," & bf.DoseKgM3.ToString(ci) & ",fR1," & bf.Params.FR1.ToString("F3", ci) & ",fR3," & bf.Params.FR3.ToString("F3", ci) & ",fR3/fR1>0.5," & s.FibreValid, "dose,0,fR1,0,fR3,0,fR3/fR1>0.5,na")
            Console.WriteLine("beam," & kv.Key & ",b," & b.B & ",h," & b.H & ",MuTop," & F2(b.MuTop / 1.0E6) & ",MuBot," & F2(b.MuBot / 1.0E6) & ",Nt_kN," & F2(b.NtMax / 1000.0) & ",VcAxialFactor," & F4(s.AxialFactor) & ",AsTop," & b.AsTop.ToString("F1", ci) & ",AsBot," & b.AsBot.ToString("F1", ci) &
                              ",DCflex," & F4(b.DcFlex) & ",Vu_d," & F2(s.Vu / 1000.0) & ",VcACI," & F2(s.VcPlain / 1000.0) & ",Vcf," & F2(s.Vcf / 1000.0) & ",dVF," & F2(s.DeltaVF / 1000.0) &
                              ",Vcf/VcACI," & F4(s.RatioVcf) & ",VRdF0(gc1)/VcACI," & F4(s.RatioMc0) & ",vminBindsDose0," & s.VminBinds & ",AvMin," & s.AvMinApplied & ",AvS_strength," & s.AvSStrength.ToString("F5", ci) & ",AvS_min," & s.AvSMin.ToString("F5", ci) & ",AvS_required," & s.AvPerS.ToString("F5", ci) & ",AvS_provided," & s.AvSProvided.ToString("F5", ci) & ",smax," & s.SMax.ToString("F1", ci) & ",s," & s.S.ToString("F1", ci) &
                              ",stirrupGov," & s.StirrupGov & ",DCshear," & F4(s.DcShear) & "," & fibTxt & ",g5_info(l/18.5|l/21;not a constraint)," & F4(b.G_Depth) & ",gov," & b.Governing & "/" & s.Governing)
        Next
    End Sub

    'EX2 V2-a pilot acceptance (reviews Q20): T = 0 and at least one beam flexure D/C, one beam shear D/C and one column P-M D/C >= 0.8
    Sub PilotCheck(ev As Ex2Result)
        Dim bf As Double = ev.BeamRes.Values.Max(Function(b) b.DcFlex)
        Dim bs As Double = ev.BeamShr.Values.Max(Function(s) s.DcShear)
        Dim cc As Double = ev.ColRes.Values.Max(Function(c) c.DcrMax)
        Dim spacingByStrength As Boolean = ev.BeamShr.Values.Any(Function(s) s.StirrupGov = "strength")
        Dim avsByStrength As Boolean = ev.BeamShr.Values.Any(Function(s) s.AvSStrength > 0.0 AndAlso s.AvSStrength >= s.AvSMin)
        Dim r626 As Double = ev.ColRes.Values.Max(Function(c) c.Ratio626)
        Dim ok As Boolean = ev.T = 0.0 AndAlso bf >= 0.8 AndAlso bs >= 0.8 AndAlso cc >= 0.8
        Console.WriteLine("pilot_check,T," & ev.T.ToString("R", ci) & ",maxDCbeamFlex," & F4(bf) & ",maxDCbeamShear," & F4(bs) & ",maxDCcolPM," & F4(cc) & ",maxACI6.2.6ratio," & F4(r626) & "(limit 1.4)" &
                          ",requiredAvS_byStrength(AvS_strength>=AvS_min)," & avsByStrength & ",spacingGovernedByStrength(s<smax)," & spacingByStrength & ",verdict," & If(ok, "OK (T=0, critical D/C >= 0.8)", "UYARI: T>0 or a critical D/C < 0.8"))
    End Sub

    'PCR-22 report: per storey Q, class, delta_s, gravity drift (mm) and the largest sway moment M_s (kN m)
    Function StabilityLine(ev As Ex2Result) As String
        Dim s = ev.Stability
        If s Is Nothing Then Return "stability,not computed (--qcheck 0: all storeys non-sway)"
        Return "stability (Q ACI 6.6.4.4.1 limit 0.05; delta_s = 1/(1-Q) ACI 6.6.4.6.2(a), Q <= 1/3; drift = gravity storey drift (floor average) mm; Ms_max = largest sway-part column end moment from the net storey sway forces kNm)," &
               String.Join(",", Enumerable.Range(1, s.Q.Length - 1).Select(Function(k) "S" & k & ":Q=" & F4(s.Q(k)) & If(s.NonSway(k), "(nonsway)", "(SWAY)") & ";deltaS=" & F4(s.DeltaS(k)) & ";drift_mm=" & s.DriftMm(k).ToString("F4", ci) & ";Ms_max_kNm=" & If(s.Braced Is Nothing, "na(no sway storey)", s.MsMaxKNm(k).ToString("F4", ci))))
    End Function

    Sub EvalOnly(prob As Ex2Problem, x As Integer())
        Dim ev = prob.Evaluate(x)
        PrintResult(prob, x, ev)
        PilotCheck(ev)
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------------------
    'Q21 marginal-cost table ("section fixed"): plain best vector -> each beam group alone and all together get (type, dose) in
    '{4D: 20, 30, 40; 5D: 25, 35}; sections fixed, longitudinal steel and stirrups re-derived. Beam cost and total cost separately.
    Function ToFibreVector(prob As Ex2Problem, xPlain As Integer(), typ As Integer, dose As Double, groups As IEnumerable(Of Integer)) As Integer()
        Dim nc As Integer = prob.ColGroups.Length * 2
        Dim r As New List(Of Integer)(xPlain.Take(nc))
        For g As Integer = 0 To prob.BeamGroups.Length - 1
            r.Add(xPlain(nc + 2 * g)) : r.Add(xPlain(nc + 2 * g + 1))
            If groups.Contains(g) Then
                r.Add(typ) : r.Add(DoseIdx(dose))
            Else
                r.Add(0) : r.Add(0)
            End If
        Next
        Return r.ToArray()
    End Function

    Sub Marginal(spec As FrameSpec2D, pr As RcCore.PriceSet, fr As Double, fmt As String, o As Ex2Options, xPlain As Integer())
        Dim pf As New Ex2Problem(spec, pr, True, ShearModels.ByName(fmt, True), fr)
        pf.Opts.QCheck = o.QCheck : pf.Opts.Patterned = o.Patterned : pf.Opts.BeamAxial = o.BeamAxial : pf.Opts.FlexCheck = o.FlexCheck : pf.Opts.DeflCheck = o.DeflCheck : pf.Opts.DeflPatterned = o.DeflPatterned
        Dim pp As New Ex2Problem(spec, pr, False, New AciPlainShear())
        pp.Opts.QCheck = o.QCheck : pp.Opts.Patterned = o.Patterned : pp.Opts.BeamAxial = o.BeamAxial : pp.Opts.FlexCheck = o.FlexCheck : pp.Opts.DeflCheck = o.DeflCheck : pp.Opts.DeflPatterned = o.DeflPatterned
        If xPlain.Length <> pp.NVars Then Throw New ArgumentException("--marginal expects a plain design vector (" & pp.NVars & " values)")
        Dim basePlain = pp.Evaluate(xPlain)
        Dim base = pf.Evaluate(ToFibreVector(pf, xPlain, 0, 0, {}))
        Console.WriteLine("# Q21 marginal cost, SECTIONS FIXED (fibre effect at fixed section; the section-reduction channel is not visible here)")
        Console.WriteLine("base_check,plainCost," & F2(basePlain.Cost.Total) & ",fibreModeDose0Cost," & F2(base.Cost.Total) & ",equal," & (Math.Abs(basePlain.Cost.Total - base.Cost.Total) < 1.0E-6) & ",T," & base.T.ToString("R", ci))
        Console.WriteLine("case,groups,type,dose,T,total,dTotal,dTotal%,beams,dBeams,dBeams%,concrete,dConcrete,long,dLong,stirrup,dStirrup,formwork,dFormwork,fibre,dFibre,perGroup(stirrupGov|flexGov|AsTop|AsBot|s)")
        Dim cases As New List(Of Tuple(Of String, Integer, Double))
        For Each t In {Tuple.Create("4D", 0, 20.0), Tuple.Create("4D", 0, 30.0), Tuple.Create("4D", 0, 40.0), Tuple.Create("5D", 1, 25.0), Tuple.Create("5D", 1, 35.0)}
            cases.Add(Tuple.Create(t.Item1, t.Item2, t.Item3))
        Next
        Dim groupSets As New List(Of Tuple(Of String, Integer()))
        For g As Integer = 0 To pf.BeamGroups.Length - 1 : groupSets.Add(Tuple.Create(pf.BeamGroups(g), {g})) : Next
        groupSets.Add(Tuple.Create("ALL", Enumerable.Range(0, pf.BeamGroups.Length).ToArray()))
        Console.WriteLine(Row("base", "-", "-", 0, base, base, pf))
        For Each gs In groupSets
            For Each c In cases
                Dim r = pf.Evaluate(ToFibreVector(pf, xPlain, c.Item2, c.Item3, gs.Item2))
                Console.WriteLine(Row("forced", gs.Item1, c.Item1, c.Item3, r, base, pf))
            Next
        Next
    End Sub

    Function Row(kind As String, grp As String, typ As String, dose As Double, r As Ex2Result, b As Ex2Result, p As Ex2Problem) As String
        Dim pct = Function(v As Double, v0 As Double) If(v0 = 0.0, "na", (100.0 * (v - v0) / v0).ToString("F2", ci))
        Dim pg = String.Join(";", p.BeamGroups.Select(Function(g) g & ":" & r.BeamShr(g).StirrupGov & "|" & r.BeamRes(g).Governing & "|" & r.BeamRes(g).AsTop.ToString("F0", ci) & "|" & r.BeamRes(g).AsBot.ToString("F0", ci) & "|" & r.BeamShr(g).S.ToString("F0", ci)))
        Return String.Join(",", {kind, grp, typ, dose.ToString(ci), r.T.ToString("R", ci), F2(r.Cost.Total), F2(r.Cost.Total - b.Cost.Total), pct(r.Cost.Total, b.Cost.Total),
                                 F2(r.CostBeams), F2(r.CostBeams - b.CostBeams), pct(r.CostBeams, b.CostBeams),
                                 F2(r.BeamConcreteTL), F2(r.BeamConcreteTL - b.BeamConcreteTL), F2(r.BeamLongTL), F2(r.BeamLongTL - b.BeamLongTL),
                                 F2(r.BeamStirTL), F2(r.BeamStirTL - b.BeamStirTL), F2(r.BeamFormTL), F2(r.BeamFormTL - b.BeamFormTL), F2(r.BeamFibreTL), F2(r.BeamFibreTL - b.BeamFibreTL), pg})
    End Function

    '---------------------------------------------------------------------------------------------------------------------------------------
    'Q22 single-pass coordinate scan around a design: every variable alone over its whole range (the +-1 subset flagged); in fibre mode also every
    '(type, dose) pair of each beam group jointly. Reports feasible cheaper neighbours; "local best" statements refer to this neighbourhood only.
    Sub Scan(prob As Ex2Problem, x As Integer())
        Dim base = prob.Evaluate(x)
        Console.WriteLine("base,cost," & F2(base.Cost.Total) & ",T," & base.T.ToString("R", ci))
        Dim nEval As Integer = 0, better As New List(Of Tuple(Of String, Double, Integer()))
        Dim betterPm1 As Integer = 0
        Dim test = Sub(lbl As String, xx As Integer(), pm1 As Boolean)
                       Dim r = prob.Evaluate(xx)
                       nEval += 1
                       If r.T = 0.0 AndAlso r.Cost.Total < base.Cost.Total - 1.0E-6 Then
                           better.Add(Tuple.Create(lbl, r.Cost.Total, xx))
                           If pm1 Then betterPm1 += 1
                       End If
                   End Sub
        For i As Integer = 0 To prob.NVars - 1
            For v As Integer = prob.Lb(i) To prob.Ub(i)
                If v = x(i) Then Continue For
                Dim xx = CType(x.Clone(), Integer()) : xx(i) = v
                test("x" & i & "=" & v, xx, Math.Abs(v - x(i)) = 1)
            Next
        Next
        If prob.UseFibre Then
            Dim nc As Integer = prob.ColGroups.Length * 2
            For g As Integer = 0 To prob.BeamGroups.Length - 1
                Dim it As Integer = nc + 4 * g + 2
                For t As Integer = prob.Lb(it) To prob.Ub(it)
                    For dz As Integer = prob.Lb(it + 1) To prob.Ub(it + 1)
                        If t = x(it) OrElse dz = x(it + 1) Then Continue For          'single-coordinate moves already covered
                        Dim xx = CType(x.Clone(), Integer()) : xx(it) = t : xx(it + 1) = dz
                        test(prob.BeamGroups(g) & " type=" & t & " dose=" & dz, xx, False)
                    Next
                Next
            Next
        End If
        Console.WriteLine("evaluations," & nEval & ",feasibleCheaper," & better.Count & ",feasibleCheaper_pm1," & betterPm1)
        Console.WriteLine("one_step_pm1_local_best," & If(betterPm1 = 0, "YES", "NO") & ",coordinate_scan_local_best," & If(better.Count = 0, "YES", "NO"))
        For Each b In better.OrderBy(Function(t) t.Item2).Take(20)
            Console.WriteLine("cheaper," & b.Item1 & "," & F2(b.Item2) & "," & F2(b.Item2 - base.Cost.Total) & ",x," & String.Join(";", b.Item3))
        Next
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------------------
    'ACI 6.4.2 live-load patterns (NOT an optimisation constraint, decision [K b]): (1) D/C of the design as optimised (steel from the single-pattern
    'envelope) under the patterned envelope; (2) re-derived design under the patterned envelope (cost to restore feasibility).
    Sub Patterns(prob As Ex2Problem, x As Integer())
        Dim keep As Boolean = prob.Opts.Patterned
        prob.Opts.Patterned = False
        Dim d = prob.Decode(x)
        Dim base = Ex2Evaluator.Evaluate(prob.Spec, d, prob.Prices, prob.Shear, prob.Opts)
        Dim defs As New Dictionary(Of String, RcSectionDef)
        For Each kv In d.Columns : defs(kv.Key) = New RcSectionDef With {.B = kv.Value.B, .H = kv.Value.H, .Fc = kv.Value.Fc} : Next
        For Each kv In d.Beams : defs(kv.Key) = New RcSectionDef With {.B = kv.Value.B, .H = kv.Value.H, .Fc = kv.Value.Fc} : Next
        Dim a2 = RcCombos2D.Analyze(prob.Spec, defs, False, True)
        'PCR-22: storey classification and the braced split for the patterned envelope (asymmetric patterns give M2s <> 0 -> delta_s applied)
        Dim st2 As StoreyStability = If(prob.Opts.QCheck, Stability2D.Assess(prob.Spec, defs, a2), Nothing)
        If st2 IsNot Nothing Then Console.WriteLine("patterned_" & StabilityLine(New Ex2Result With {.Stability = st2}))
        Console.WriteLine("patterned_combos," & a2.Combos.Count & " (1.4D, 1.2D+1.6L all spans, " & (a2.Combos.Count - 2) & " ACI 6.4.1/6.4.2 patterns)")
        Console.WriteLine("base,cost," & F2(base.Cost.Total) & ",T," & base.T.ToString("R", ci))
        Dim nOver As Integer = 0
        For Each g In a2.Frame.BeamGroups
            Dim b0 = base.BeamRes(g), s0 = base.BeamShr(g), bd = d.Beams(g)
            Dim r2 = BeamDesignGravity.DesignGroup(a2, g, bd.B, bd.H, bd.Fc, bd.Fibre, prob.Opts.BeamAxial, prob.Opts.FlexCheck)
            'PCR-23/24: the same face D/C as the evaluator (consistent SFRC capacity, axial tension of the patterned envelope)
            Dim dcf As Double = Math.Max(BeamDesignGravity.FaceDc(r2.MuTop, r2.NtMax, bd.B, bd.H, b0.D, b0.AsTop, bd.Fc, bd.Fibre), BeamDesignGravity.FaceDc(r2.MuBot, r2.NtMax, bd.B, bd.H, b0.D, b0.AsBot, bd.Fc, bd.Fibre))
            Dim vu2 As Double = BeamShearGravity.DemandAtD(a2, g, b0.D)
            Dim av As Double = 2.0 * Math.PI * BeamShearGravity.StirrupDia ^ 2 / 4.0
            Dim vs As Double = Math.Min(av * BeamShearGravity.Fyt * b0.D / s0.S, RcCore.AciBeam.VsMax(bd.B, b0.D, bd.Fc))
            Dim dcs As Double = vu2 / (BeamShearGravity.Phi * (s0.Vcf + vs))
            If dcf > 1.0 + 1.0E-9 Then nOver += 1
            If dcs > 1.0 + 1.0E-9 Then nOver += 1
            Console.WriteLine("beam," & g & ",MuTop_single," & F2(b0.MuTop / 1.0E6) & ",MuTop_pattern," & F2(r2.MuTop / 1.0E6) & ",MuBot_single," & F2(b0.MuBot / 1.0E6) & ",MuBot_pattern," & F2(r2.MuBot / 1.0E6) &
                              ",DCflex_asDesigned," & F4(dcf) & ",Vu_single," & F2(s0.Vu / 1000.0) & ",Vu_pattern," & F2(vu2 / 1000.0) & ",DCshear_asDesigned," & F4(dcs))
        Next
        For Each g In a2.Frame.ColumnGroups
            Dim c0 = base.ColRes(g), cd = d.Columns(g)
            Dim c2 = ColumnDesignGravity.CheckGroup(a2, g, cd.B, cd.H, cd.Fc, c0.Rho, cd.BarDia, False, st2)
            If c2.DcrMax > 1.0 + 1.0E-9 Then nOver += 1
            If c2.Ratio626 > ColumnDesignGravity.Ratio626Limit + 1.0E-9 Then nOver += 1
            Console.WriteLine("col," & g & ",DC_single," & F4(c0.DcrMax) & ",DC_pattern_asDesigned," & F4(c2.DcrMax) & ",ACI6.2.6_ratio_single," & F4(c0.Ratio626) & ",ACI6.2.6_ratio_pattern," & F4(c2.Ratio626) & ",deltaS_pattern," & F4(c2.DeltaSMax))
        Next
        Console.WriteLine("checks_over_1_asDesigned," & nOver)
        prob.Opts.Patterned = True
        Dim rp = prob.Evaluate(x)
        prob.Opts.Patterned = keep
        Console.WriteLine("redesign_with_patterns,cost," & F2(rp.Cost.Total) & ",dCost," & F2(rp.Cost.Total - base.Cost.Total) & ",dCost%," & (100.0 * (rp.Cost.Total - base.Cost.Total) / base.Cost.Total).ToString("F2", ci) & ",T," & rp.T.ToString("R", ci))
        For Each v In rp.Violations.Where(Function(t) t.Item2 > 0.0) : Console.WriteLine("redesign_violation," & v.Item1.Replace(",", ";") & "," & F4(v.Item2)) : Next
    End Sub

    '---------------------------------------------------------------------------------------------------------------------------------------
    'Fibre group-by-group zeroing (reviews Q19): dose -> 0 in one beam group at a time (and all); feasible + equal non-fibre cost = "neutral".
    Sub ZeroFibre(prob As Ex2Problem, x As Integer())
        If Not prob.UseFibre Then Throw New ArgumentException("--zerofibre needs --fibre 1")
        Dim base = prob.Evaluate(x)
        Dim nc As Integer = prob.ColGroups.Length * 2
        Console.WriteLine("base,cost," & F2(base.Cost.Total) & ",nonFibre," & F2(NonFibre(base)) & ",fibre," & F2(base.Cost.Fibre) & ",T," & base.T.ToString("R", ci))
        Console.WriteLine("group,dose,T_zeroed,cost_zeroed,dCost,dNonFibre,label")
        Dim all = CType(x.Clone(), Integer())
        Dim any As Boolean = False
        For g As Integer = 0 To prob.BeamGroups.Length - 1
            Dim id As Integer = nc + 4 * g + 3
            If x(id) = 0 Then
                Console.WriteLine(prob.BeamGroups(g) & ",0,-,-,-,-,no fibre")
                Continue For
            End If
            any = True : all(id) = 0
            Dim xx = CType(x.Clone(), Integer()) : xx(id) = 0
            Dim r = prob.Evaluate(xx)
            Console.WriteLine(prob.BeamGroups(g) & "," & Ex2Problem.Doses(x(id)).ToString(ci) & "," & r.T.ToString("R", ci) & "," & F2(r.Cost.Total) & "," & F2(r.Cost.Total - base.Cost.Total) & "," & F2(NonFibre(r) - NonFibre(base)) & "," & Label(r, base))
        Next
        If any Then
            Dim r = prob.Evaluate(all)
            Console.WriteLine("ALL,-," & r.T.ToString("R", ci) & "," & F2(r.Cost.Total) & "," & F2(r.Cost.Total - base.Cost.Total) & "," & F2(NonFibre(r) - NonFibre(base)) & "," & Label(r, base))
        End If
    End Sub

    Function Label(r As Ex2Result, b As Ex2Result) As String
        If r.T > 0.0 Then Return "required (infeasible without fibre)"
        Dim dn As Double = NonFibre(r) - NonFibre(b)
        If Math.Abs(dn) < 0.01 Then Return "neutral (feasible; non-fibre cost unchanged)"
        If dn > 0.0 Then Return "useful (fibre saves " & F2(dn) & " TL of non-fibre cost)"
        Return "harmful (non-fibre cost lower without fibre by " & F2(-dn) & " TL)"
    End Function

    '---------------------------------------------------------------------------------------------------------------------------------------
    Sub Deflection(prob As Ex2Problem, x As Integer())
        Dim d = prob.Decode(x)
        Dim ev = Ex2Evaluator.Evaluate(prob.Spec, d, prob.Prices, prob.Shear, prob.Opts)
        Console.WriteLine("# ACI 24.2 report (not a constraint): Ie 24.2.3.5/24.2.3.6 per group, lambda = 2.0 (rho' = 0), sustained = D, live transient; mm")
        Console.WriteLine("element,group,span_m,IeIg_D,IeIg_DL,dD,dDL,dL,dAfter=2dD+dL,lim_l480,ratio480,lim_l240(same quantity as l480; not likely damaged),ratio240,lim_l360(L),ratioL360")
        Dim worst480 As Double = 0.0, worst240 As Double = 0.0, worst360 As Double = 0.0
        For Each rw In Deflection2D.Report(prob.Spec, d, ev)
            Dim r480 As Double = rw.DeltaAfter / rw.LimAfter480, r240 As Double = rw.DeltaAfter / rw.LimTotal240, r360 As Double = rw.DeltaL / rw.LimL360
            worst480 = Math.Max(worst480, r480) : worst240 = Math.Max(worst240, r240) : worst360 = Math.Max(worst360, r360)
            Console.WriteLine(String.Join(",", {rw.Element, rw.Group, rw.SpanM.ToString(ci), F4(rw.IeOverIgD), F4(rw.IeOverIgDL), F2(rw.DeltaD), F2(rw.DeltaDL), F2(rw.DeltaL), F2(rw.DeltaAfter), F2(rw.LimAfter480), F4(r480), F2(rw.LimTotal240), F4(r240), F2(rw.LimL360), F4(r360)}))
        Next
        Console.WriteLine("worst_ratio,l480," & F4(worst480) & ",l240," & F4(worst240) & ",l360," & F4(worst360) & ",verdict_l480," & If(worst480 <= 1.0, "OK", "EXCEEDED"))
    End Sub
End Module
