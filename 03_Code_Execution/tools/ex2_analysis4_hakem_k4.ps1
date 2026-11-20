# Example 2 (Ö2) campaign 4: referee-round (Review_Round_Ex2_k4_ic / _dis, 2026-11-19) deterministic sensitivity tools.
# NO optimisation, NO change to the frozen binary: the C# below only CALLS the public API of the frozen campaign-4 DLLs
# (Ex2Model f7207d66..., FemCore, FemRcModel, RcCore) loaded from a SHA-checked COPY of ex2_campaign4_bin.
#  (1) defl_sens : ACI 24.2 l/480 ratio of P, F0, F0Z and F0_c2 (campaign-2 F0 vector) under assumption variants (net span, sustained share psi of L,
#                  immediate D counted, Bischoff Ie, per-case Ie, Ie = Ig bound, stress combinations). Engine = per-case solves (as ex2_analysis4_extra DeflPat),
#                  i.e. NOT the superposition of Deflection2D.PatternedEnvelope; baseline variant V0 is compared with the library value (tolerance 1e-9).
#  (2) match     : library Deflection2D.PatternedEnvelope max <-> tool V0 (code <-> tool match, K4 item 4, iç Critic B3)
#  (3) r8        : ACI 6.2.6 ratio / storey Q / delta_s of P and F0 for gravity load x 0.95 / 1.00 / 1.05 (sections fixed, reinforcement re-derived),
#                  critical load factors (ratio 1.4, Q = 0.05) by bisection
#  (4) lcs       : SFRC flexure lcs = max(h - c, 0.2 h) of the F0 beams: is the 0.2 h floor active? (K3 / A3)
#  (5) bebound   : smallest feasible fibre-beam section per group (others at P sections, fibre 4D/20 kg/m3) -> fibre mass floor m_min, rough break-even scale
# Usage (Windows PowerShell 5.1): powershell -File ex2_analysis4_hakem_k4.ps1 <copied_bin_dir> <analysis4_dir>
param([string]$Bin, [string]$An)
$ErrorActionPreference = "Stop"
$Out = Join-Path $An "sensitivity_defl"
New-Item -ItemType Directory -Force $Out | Out-Null
$dlls = "Ex2Model.dll", "FemCore.dll", "FemRcModel.dll", "RcCore.dll"
$refs = $dlls | ForEach-Object { Join-Path $Bin $_ }
foreach ($r in $refs) { [void][Reflection.Assembly]::LoadFrom($r) }
$ver = ($dlls | ForEach-Object { "$_ sha256=" + (Get-FileHash -Algorithm SHA256 (Join-Path $Bin $_)).Hash.ToLower() }) -join "; "

$src = @'
using System; using System.Collections.Generic; using System.Linq; using System.IO; using System.Globalization; using System.Text;
using Ex2Model; using FemCore; using FemRcModel; using RcCore;
public class DOpt { public string Name; public bool Net; public double Psi; public bool InstD; public string Ie = "branson"; public bool PerCase; }
public static class HK {
  static CultureInfo ci = CultureInfo.InvariantCulture;
  static string F(double v, string f) { return v.ToString(f, ci); }
  public static Ex2Problem Make(bool fibre, double fp, double load) {
    FrameSpec2D spec = new FrameSpec2D(); spec.Groups = 9;
    spec.DeadLine *= load; spec.LiveLine *= load; spec.ConcreteUnitWeight *= load;
    PriceSet pr = new PriceSet(); pr.Fibre = fp;
    Ex2Problem p = new Ex2Problem(spec, pr, fibre, ShearModels.ByName("delta", fibre), 1.0);
    p.Opts.QCheck = true; p.Opts.Patterned = true; p.Opts.BeamAxial = true; p.Opts.FlexCheck = true; p.Opts.DeflCheck = true; p.Opts.DeflPatterned = true;
    return p;
  }
  public static int[] ReadX(string path) {
    string line = File.ReadAllLines(path).First(s => s.StartsWith("x,"));
    return line.Substring(2).Split(',').Select(s => int.Parse(s.Trim())).ToArray();
  }
  static double Icr(double b, double d, double As_, double n) {
    double A = b / 2.0, B = n * As_, C = -n * As_ * d; double x = (-B + Math.Sqrt(B * B - 4.0 * A * C)) / (2.0 * A);
    return b * Math.Pow(x, 3) / 3.0 + n * As_ * Math.Pow(d - x, 2);
  }
  // Bischoff effective inertia (ACI 318-19 24.2.3.5 form, Bischoff 2005; formula from the literature, source PDF NOT in Evidence/: assumption, label in the report)
  static double IeOf(string mode, double b, double h, double d, double As_, double fc, double Ma) {
    double ig = b * Math.Pow(h, 3) / 12.0;
    if (mode == "ig") return ig;
    if (mode == "branson") return Deflection2D.Ie(b, h, d, As_, fc, Ma);
    double mcr = 0.62 * Math.Sqrt(fc) * ig / (h / 2.0);
    if (Ma <= 2.0 / 3.0 * mcr) return ig;
    double n = 200000.0 / (4700.0 * Math.Sqrt(fc)); double icr = Icr(b, d, As_, n);
    double r = Math.Pow(2.0 / 3.0 * mcr / Ma, 2);
    return Math.Min(ig, icr / (1.0 - r * (1.0 - icr / ig)));
  }
  static void Moments(Analysis2D a, string g, List<int> qon, out double neg, out double pos) {
    FemModel m = a.Frame.Model; int pSW = m.PatternIndex("SW"), pG = m.PatternIndex("G");
    neg = 0; pos = 0;
    for (int ei = 0; ei < m.Elements.Count; ei++) {
      if (m.Elements[ei].Group != g) continue;
      double[] xs = a.StationX[ei];
      double[][] s0 = a.Fs.StationForces(a.R, pSW, ei, xs), s1 = a.Fs.StationForces(a.R, pG, ei, xs);
      var sq = new List<double[][]>();
      foreach (int e2 in qon) sq.Add(a.Fs.StationForces(a.R, m.PatternIndex("Q_E" + e2), ei, xs));
      for (int j = 0; j < xs.Length; j++) {
        double m3 = s0[j][5] + s1[j][5]; foreach (var s in sq) m3 += s[j][5];
        neg = Math.Max(neg, -m3); pos = Math.Max(pos, m3);
      }
    }
  }
  static double[] Solve(FrameSpec2D spec, Ex2Design d, Dictionary<string, double> ratio, List<int> qon, out FrameModel2D fm) {
    var secs = new Dictionary<string, FemSection>();
    foreach (var kv in d.Columns) secs[kv.Key] = RcFrameBuilder.MakeSection(kv.Key, kv.Value.B, kv.Value.H, kv.Value.Fc, spec.ConcreteUnitWeight, 1.0);
    foreach (var kv in d.Beams) secs[kv.Key] = RcFrameBuilder.MakeSection(kv.Key, kv.Value.B, kv.Value.H, kv.Value.Fc, spec.ConcreteUnitWeight, ratio[kv.Key]);
    fm = RcFrameBuilder2D.Build(spec, secs);
    FemPattern pq = new FemPattern(); pq.Name = "QP"; pq.SelfWeightMultiplier = 0.0;
    if (qon != null) foreach (int ei in qon) { FemDistLoad dl = new FemDistLoad(); dl.Element = ei; dl.Dir = 10; dl.IsGlobal = true; dl.Relative = true; dl.D1 = 0; dl.D2 = 1; dl.V1 = spec.LiveLine; dl.V2 = spec.LiveLine; pq.DistLoads.Add(dl); }
    fm.Model.Patterns.Add(pq);
    StaticResults rr = new FemStatic(fm.Model).Solve();
    if (rr.Failed) throw new Exception("deflection model failed");
    FemModel m = fm.Model; int iSW = m.PatternIndex("SW"), iG = m.PatternIndex("G"), iQ = m.PatternIndex("QP");
    double[] u = new double[rr.U[iSW].Length];
    for (int k = 0; k < u.Length; k++) u[k] = rr.U[iSW][k] + rr.U[iG][k] + (qon != null ? rr.U[iQ][k] : 0.0);
    return u;
  }
  static double ColH(Ex2Design d, FrameSpec2D spec, FemModel m, int node, int story) {
    double x = m.Nodes[node].X; int line = -1;
    for (int i = 0; i < spec.NCols; i++) if (Math.Abs(spec.XOf(i) - x) < 1e-6) line = i;
    string g = (line == 0 || line == spec.NCols - 1) ? "C_Edge_S" + story : "C_Inter_S" + story;
    return d.Columns[g].H;
  }
  // worst l/480 ratio over the 25 load cases and all beam elements; Ie group value = min over cases (envelope) or per case (opt.PerCase)
  public static double Ratio(Ex2Problem prob, int[] x, DOpt o, out string at) {
    Ex2Design d = prob.Decode(x); Ex2Result ev = prob.Evaluate(x); Analysis2D a = ev.Analysis; FrameSpec2D spec = prob.Spec; FemModel m = a.Frame.Model;
    var cases = new List<List<int>>(); var names = new List<string>();
    var all = new List<int>(); for (int ei = 0; ei < m.Elements.Count; ei++) if (!a.Frame.IsColumn(ei)) all.Add(ei);
    cases.Add(all); names.Add("all spans");
    foreach (Combo2D c in RcCombos2D.PatternCombos(a.Frame)) { cases.Add(new List<int>(c.QOn)); names.Add(c.Name); }
    Func<string, List<int>, double> ratioOf = (g, q) => {
      double n, p; Moments(a, g, q, out n, out p); var bd = d.Beams[g]; var br = ev.BeamRes[g]; double ig = bd.B * Math.Pow(bd.H, 3) / 12.0;
      return 0.5 * (IeOf(o.Ie, bd.B, bd.H, br.D, br.AsTop, bd.Fc, n * 1e6) + IeOf(o.Ie, bd.B, bd.H, br.D, br.AsBot, bd.Fc, p * 1e6)) / ig; };
    var rD = new Dictionary<string, double>(); var rEnv = new Dictionary<string, double>(); var rCase = new List<Dictionary<string, double>>();
    foreach (string g in a.Frame.BeamGroups) rD[g] = ratioOf(g, new List<int>());
    foreach (var cs in cases) { var r = new Dictionary<string, double>();
      foreach (string g in a.Frame.BeamGroups) { r[g] = ratioOf(g, cs); rEnv[g] = rEnv.ContainsKey(g) ? Math.Min(rEnv[g], r[g]) : r[g]; }
      rCase.Add(r); }
    FrameModel2D fmD; double[] uD = Solve(spec, d, rD, null, out fmD);
    double worst = double.NegativeInfinity; at = "";
    for (int ci_ = 0; ci_ < cases.Count; ci_++) {
      var ratio = o.PerCase ? rCase[ci_] : rEnv;
      FrameModel2D fmL; double[] uL = Solve(spec, d, ratio, cases[ci_], out fmL);
      for (int ei = 0; ei < m.Elements.Count; ei++) {
        if (a.Frame.IsColumn(ei)) continue;
        var e = m.Elements[ei]; var bd = d.Beams[e.Group];
        double ecm = 4700.0 * Math.Sqrt(bd.Fc) * 1000.0, ig = bd.B * Math.Pow(bd.H, 3) / 12.0 * 1e-12, wSW = spec.ConcreteUnitWeight * bd.B * bd.H * 1e-6;
        bool loaded = cases[ci_].Contains(ei);
        double dD = Deflection2D.MidspanDeflection(new double[][] { uD }, fmD, ei, wSW + spec.DeadLine, ecm * ig * rD[e.Group]) * 1000.0;
        double dDL = Deflection2D.MidspanDeflection(new double[][] { uL }, fmL, ei, wSW + spec.DeadLine + (loaded ? spec.LiveLine : 0.0), ecm * ig * ratio[e.Group]) * 1000.0;
        double dL = dDL - dD;
        double dA = Deflection2D.Xi * (dD + o.Psi * dL) + (1.0 - o.Psi) * dL; if (o.InstD) dA += dD;
        double L = e.Length * 1000.0;
        if (o.Net) L -= 0.5 * (ColH(d, spec, m, e.NI, e.Story) + ColH(d, spec, m, e.NJ, e.Story));
        double r480 = dA / (L / 480.0);
        if (r480 > worst) { worst = r480; at = names[ci_] + " " + e.Name + " (" + e.Group + ", span " + F(e.Length, "R") + " m)"; }
      }
    }
    return worst;
  }
  public static double Library(Ex2Problem prob, int[] x) {
    Ex2Design d = prob.Decode(x); Ex2Result ev = prob.Evaluate(x);
    return Deflection2D.PatternedEnvelope(prob.Spec, d, ev).Max();
  }
  public static string Variants(Ex2Problem prob, int[] x, string label) {
    var vs = new List<DOpt> {
      new DOpt { Name = "V0 baseline (axis span, psi 0, Branson, envelope Ie)" },
      new DOpt { Name = "V1 net span l_n (l - mean column depth) for the limit", Net = true },
      new DOpt { Name = "V2 psi = 0.25 (share of L sustained)", Psi = 0.25 },
      new DOpt { Name = "V3 psi = 0.50", Psi = 0.50 },
      new DOpt { Name = "V4 immediate D deflection counted ((1+lambda) dD + dL; partitions attached before D)", InstD = true },
      new DOpt { Name = "V5 Bischoff Ie (envelope)", Ie = "bischoff" },
      new DOpt { Name = "V6 Branson Ie per load case (not envelope)", PerCase = true },
      new DOpt { Name = "V7 Ie = Ig (uncracked; math upper bound of stiffness, NOT a fibre estimate)", Ie = "ig" },
      new DOpt { Name = "V8 psi 0.25 + Bischoff", Psi = 0.25, Ie = "bischoff" },
      new DOpt { Name = "V9 psi 0.25 + Bischoff + net span (stress)", Psi = 0.25, Ie = "bischoff", Net = true }
    };
    var sb = new StringBuilder();
    foreach (var v in vs) { string at; double r = Ratio(prob, x, v, out at);
      sb.AppendLine(label + "," + v.Name.Replace(",", ";") + "," + F(r, "F4") + "," + (r > 1.0 ? "EXCEEDS 1" : "<= 1") + "," + at.Replace(",", ";")); }
    return sb.ToString();
  }
  public static string Match(Ex2Problem prob, int[] x, string label) {
    string at; double tool = Ratio(prob, x, new DOpt(), out at); double lib = Library(prob, x);
    return label + ",library_PatternedEnvelope_max," + F(lib, "R") + ",tool_V0_per_case_solves," + F(tool, "R") + ",abs_diff," + F(Math.Abs(lib - tool), "E3") + "," + (Math.Abs(lib - tool) <= 1e-9 ? "MATCH (<=1e-9)" : "MISMATCH");
  }
  // (3) R8: gravity load scale s on D, L and self weight; sections fixed, reinforcement re-derived
  static Ex2Result EvalS(bool fibre, double fp, int[] x, double s) { return Make(fibre, fp, s).Evaluate(x); }
  static double MaxR626(Ex2Result r) { return r.ColRes.Values.Max(c => c.Ratio626); }
  public static string R8(bool fibre, double fp, int[] x, string label) {
    var sb = new StringBuilder();
    foreach (double s in new[] { 0.95, 1.00, 1.05 }) {
      Ex2Result r = EvalS(fibre, fp, x, s);
      string q = string.Join(";", Enumerable.Range(1, r.Stability.Q.Length - 1).Select(i => F(r.Stability.Q[i], "F4") + (r.Stability.NonSway[i] ? "(non-sway)" : "(sway)")));
      string ds = string.Join(";", Enumerable.Range(1, r.Stability.DeltaS.Length - 1).Select(i => F(r.Stability.DeltaS[i], "F4")));
      string viol = string.Join(" | ", r.Violations.Where(v => v.Item2 > 0).Select(v => v.Item1 + "=" + F(v.Item2, "F4")));
      sb.AppendLine(label + ",load x" + F(s, "F2") + ",cost," + F(r.Cost.Total, "F2") + ",T," + F(r.T, "R") + ",Q(S1;S2;S3)," + q + ",delta_s," + ds + ",ratio626_max," + F(MaxR626(r), "F4") + ",ratio626_in," + F(r.ColRes.Values.Max(c => c.Ratio626In), "F4") + ",ratio626_out," + F(r.ColRes.Values.Max(c => c.Ratio626Out), "F4") + ",violations," + (viol == "" ? "none" : viol));
    }
    // critical factors
    double lo = 1.0, hi = 1.5; double cr;
    if (MaxR626(EvalS(fibre, fp, x, hi)) <= 1.4) cr = double.NaN; else { for (int i = 0; i < 30; i++) { double mid = 0.5 * (lo + hi); if (MaxR626(EvalS(fibre, fp, x, mid)) > 1.4) hi = mid; else lo = mid; } cr = hi; }
    sb.AppendLine(label + ",critical_load_factor_ratio626_exceeds_1.4," + (double.IsNaN(cr) ? ">1.50" : F(cr, "F4")));
    // Q of the top storey crossing 0.05 (non-sway <-> sway)
    int top = EvalS(fibre, fp, x, 1.0).Stability.Q.Length - 1;
    double q1 = EvalS(fibre, fp, x, 1.0).Stability.Q[top];
    double a0 = 0.5, a1 = 1.5; double qa = EvalS(fibre, fp, x, a0).Stability.Q[top], qb = EvalS(fibre, fp, x, a1).Stability.Q[top];
    sb.AppendLine(label + ",top_storey_Q_vs_load,Q(x0.5)," + F(qa, "F4") + ",Q(x1.0)," + F(q1, "F4") + ",Q(x1.5)," + F(qb, "F4") + ",note,Q = 0.05 crossing: " + ((qa - 0.05) * (q1 - 0.05) < 0 ? "between x0.5 and x1.0" : ((q1 - 0.05) * (qb - 0.05) < 0 ? "between x1.0 and x1.5" : "none in x0.5..x1.5")));
    return sb.ToString();
  }
  // (4) lcs floor
  public static string Lcs(Ex2Problem prob, int[] x, string label) {
    Ex2Design d = prob.Decode(x); Ex2Result ev = prob.Evaluate(x); var sb = new StringBuilder();
    foreach (string g in ev.BeamRes.Keys) {
      var bd = d.Beams[g]; var br = ev.BeamRes[g]; if (bd.Fibre == null) { sb.AppendLine(label + "," + g + ",plain (no SFRC flexure)"); continue; }
      double lt = 0, lb = 0;
      BeamDesignGravity.FlexFixed(bd.B, bd.H, br.D, br.AsTop, bd.Fc, bd.Fibre, 0.0, ref lt);
      BeamDesignGravity.FlexFixed(bd.B, bd.H, br.D, br.AsBot, bd.Fc, bd.Fibre, 0.0, ref lb);
      sb.AppendLine(label + "," + g + ",h," + F(bd.H, "F0") + ",0.2h," + F(0.2 * bd.H, "F1") + ",lcs_top_face," + F(lt, "F2") + ",lcs_bottom_face," + F(lb, "F2") + ",floor_active_top," + (Math.Abs(lt - 0.2 * bd.H) < 1e-3) + ",floor_active_bottom," + (Math.Abs(lb - 0.2 * bd.H) < 1e-3));
    }
    return sb.ToString();
  }
  // (5) smallest feasible fibre-beam section per group: others at the P sections, no fibre; group g: 4D / dose index 1 (20 kg/m3)
  public static string BEBound(int[] xPplain, double pCost, double cRef) {
    Ex2Problem p = Make(true, 0.0, 1.0); var sb = new StringBuilder();
    int[] bx = new int[p.NVars]; for (int i = 0; i < 12; i++) bx[i] = xPplain[i];
    for (int g = 0; g < 3; g++) { bx[12 + 4 * g] = xPplain[12 + 2 * g]; bx[12 + 4 * g + 1] = xPplain[12 + 2 * g + 1]; }
    Ex2Result r0 = p.Evaluate(bx);
    sb.AppendLine("base (P sections, all doses 0 in the fibre problem): cost," + F(r0.Cost.Total, "F2") + ",T," + F(r0.T, "R"));
    double mMin = double.PositiveInfinity; string where = "";
    for (int g = 0; g < 3; g++) {
      double best = double.PositiveInfinity; string bs = "";
      for (int bi = 0; bi < 5; bi++) for (int hi = 0; hi < 7; hi++) {
        int[] x = (int[])bx.Clone(); x[12 + 4 * g] = bi; x[12 + 4 * g + 1] = hi; x[12 + 4 * g + 2] = 0; x[12 + 4 * g + 3] = 1;
        Ex2Result r = p.Evaluate(x);
        if (r.T == 0.0 && r.FibreKg < best) { best = r.FibreKg; bs = Ex2Problem.BPool[bi] + "x" + Ex2Problem.HPool[hi]; }
      }
      sb.AppendLine("B_S" + (g + 1) + ",smallest_feasible_section_with_fibre_4D/20," + bs + ",fibre_kg," + F(best, "F2"));
      if (best < mMin) { mMin = best; where = "B_S" + (g + 1) + " " + bs; }
    }
    sb.AppendLine("m_min_fibre_kg," + F(mMin, "F2") + ",at," + where);
    sb.AppendLine("rough scale (P - C_ref) / m_min with C_ref = " + F(cRef, "F2") + " (best fp0 design found; NOT a proven minimum of the non-fibre cost): " + F((pCost - cRef) / mMin, "F2") + " TL/kg");
    return sb.ToString();
  }
}
'@
$fw = [Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()
Add-Type -TypeDefinition $src -ReferencedAssemblies ($refs + "System.Core.dll" + (Join-Path $fw "netstandard.dll") + (Join-Path $fw "System.Runtime.dll")) -Language CSharp

$X = Join-Path $An "x"
$xP = [HK]::ReadX((Join-Path $X "P.txt")); $xF0 = [HK]::ReadX((Join-Path $X "F0.txt")); $xF0Z = [HK]::ReadX((Join-Path $X "F0Z.txt"))
$xC2 = [HK]::ReadX((Join-Path $Out "F0c2_x.txt"))
$pP = [HK]::Make($false, 90.0, 1.0); $pF = [HK]::Make($true, 0.0, 1.0)
$sw = [Diagnostics.Stopwatch]::StartNew()

# (2) match
$m = @("# $ver", "# code <-> tool match (K4 item 4): Deflection2D.PatternedEnvelope (superposition engine, frozen DLL) vs tool V0 (per-case solves), tolerance 1e-9")
$m += [HK]::Match($pP, $xP, "P"); $m += [HK]::Match($pF, $xF0, "F0"); $m += [HK]::Match($pF, $xF0Z, "F0Z"); $m += [HK]::Match($pF, $xC2, "F0_c2 (campaign-2 vector)")
$m | Set-Content -Encoding utf8 (Join-Path $Out "code_tool_match.txt"); "match " + $sw.Elapsed.TotalSeconds

# (1) sensitivities
$s = @("# $ver", "# design,variant,worst l/480 ratio (ACI 24.2 Table 24.2.2 incl. ACI 6.4.2 patterns; Xi = 2.0),verdict,governing case/element",
  "# psi: sustained share of the case live load; dAfter = Xi (dD + psi dL) + (1 - psi) dL, dL = dDL - dD (as the library); V4 adds dD; V5/V8/V9 Bischoff formula from the literature (ACI 318-19 24.2.3.5), source PDF not in Evidence: assumption",
  "# V7 is a mathematical bound (Ie = Ig), not a fibre stiffness estimate: no source for the fibre stiffness contribution in the project -> fibre stiffness estimate NOT produced (UNVERIFIED: DOGRULANAMADI)")
$s += [HK]::Variants($pP, $xP, "P (= F90 = PY)"); $s += [HK]::Variants($pF, $xF0, "F0"); $s += [HK]::Variants($pF, $xF0Z, "F0Z"); $s += [HK]::Variants($pF, $xC2, "F0_c2 (campaign-2 vector; infeasible under campaign-4 g5)")
$s | Set-Content -Encoding utf8 (Join-Path $Out "defl_sensitivity.csv"); "sens " + $sw.Elapsed.TotalSeconds

# (3) R8
$r = @("# $ver", "# R8: gravity load (D, L, self weight) x0.95 / 1.00 / 1.05; sections FIXED, reinforcement re-derived; campaign-4 rules (patterns, axial, g5 pattern/envelope)")
$r += [HK]::R8($false, 90.0, $xP, "P"); $r += [HK]::R8($true, 0.0, $xF0, "F0")
$r | Set-Content -Encoding utf8 (Join-Path $Out "r8_load_sensitivity.txt"); "r8 " + $sw.Elapsed.TotalSeconds

# (4) lcs
$l = @("# $ver", "# SFRC flexure lcs = max(h - c, 0.2 h): is the 0.2 h floor active on the provided steel of the F0 beams?")
$l += [HK]::Lcs($pF, $xF0, "F0")
$l | Set-Content -Encoding utf8 (Join-Path $Out "lcs_floor_F0.txt")

# (5) break-even scale
$b = @("# $ver", "# Fibre mass floor from the smallest FEASIBLE fibre-beam section (g3 eps_t, g8, shear, g5 ...), others at the P sections; NOT the shallowest pool beam")
$b += [HK]::BEBound($xP, 236456.22, 234190.76)
$b | Set-Content -Encoding utf8 (Join-Path $Out "breakeven_upper_scale.txt"); "done " + $sw.Elapsed.TotalSeconds
