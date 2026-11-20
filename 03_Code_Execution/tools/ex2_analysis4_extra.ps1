# Example 2 (Ö2) campaign 4: deterministic EXTRA tools that the frozen Ex2Runner does not provide (no optimisation, no change to the binary).
# The C# below only CALLS the public API of the frozen campaign-4 DLLs (Ex2Model f7207d66..., FemCore, FemRcModel, RcCore) loaded from a SHA-checked COPY.
#  (1) scan2      : two-coordinate neighbourhood scan (every pair of variables, every value pair) around a design (dış hakem R7)
#  (2) deflpat    : ACI 24.2 deflection report under the ACI 6.4.2 live-load patterns (same 24 patterns as RcCombos2D.PatternCombos) (dış hakem R4);
#                   same formulas as Deflection2D.Report (Ie 24.2.3.5/24.2.3.6 per group, lambda = 2, sustained = D); the "full" row reproduces Report
#  (3) breakeven  : single-design break-even fibre price over ALL campaign-4 fp0 final vectors and all their fibre-group subsets, fR x 0.6/0.8/1.0/1.2,
#                   formats delta (main) and mc2010; reference P = cheapest plain design found (fR independent)
#  (4) cardinality: size of the discrete design space (plain / fibre)
# Usage (Windows PowerShell 5.1): powershell -File ex2_analysis4_extra.ps1 <copied_bin_dir> <analysis4_dir> <campaign4_dir>
param([string]$Bin, [string]$Out, [string]$Camp)
$ErrorActionPreference = "Stop"
$dlls = "Ex2Model.dll", "FemCore.dll", "FemRcModel.dll", "RcCore.dll"
$refs = $dlls | ForEach-Object { Join-Path $Bin $_ }
foreach ($r in $refs) { [void][Reflection.Assembly]::LoadFrom($r) }
$ver = ($dlls | ForEach-Object { "$_ sha256=" + (Get-FileHash -Algorithm SHA256 (Join-Path $Bin $_)).Hash.ToLower() }) -join "; "

$src = @"
using System; using System.Collections.Generic; using System.Linq; using System.IO; using System.Globalization; using System.Threading.Tasks; using System.Text;
using Ex2Model; using FemCore; using FemRcModel; using RcCore;
public static class Ex2Extra {
  static CultureInfo ci = CultureInfo.InvariantCulture;
  static string F(double v, string f) { return v.ToString(f, ci); }
  public static Ex2Problem Make(bool fibre, double fp, string fmt, double fr) {
    FrameSpec2D spec = new FrameSpec2D(); spec.Groups = 9;
    PriceSet pr = new PriceSet(); pr.Fibre = fp;
    Ex2Problem p = new Ex2Problem(spec, pr, fibre, ShearModels.ByName(fmt, fibre), fr);
    p.Opts.QCheck = true; p.Opts.Patterned = true; p.Opts.BeamAxial = true; p.Opts.FlexCheck = true; p.Opts.DeflCheck = true; p.Opts.DeflPatterned = true;
    return p;
  }
  public static int[] ReadX(string path) {
    string line = File.ReadAllLines(path).First(s => s.Trim() != "" && !s.TrimStart().StartsWith("#") && s.StartsWith("x,"));
    return line.Substring(2).Split(',').Select(s => int.Parse(s.Trim())).ToArray();
  }
  public static string XStr(int[] x) { return string.Join(";", x.Select(v => v.ToString())); }

  // (1) two-coordinate scan
  public static void Scan2(Ex2Problem prob, int[] x, string outPath, string ver) {
    Ex2Result b = prob.Evaluate(x); double c0 = b.Cost.Total;
    var moves = new List<int[]>();
    for (int i = 0; i < prob.NVars; i++) for (int j = i + 1; j < prob.NVars; j++)
      for (int vi = prob.Lb[i]; vi <= prob.Ub[i]; vi++) { if (vi == x[i]) continue;
        for (int vj = prob.Lb[j]; vj <= prob.Ub[j]; vj++) { if (vj == x[j]) continue; moves.Add(new int[] { i, vi, j, vj }); } }
    var cheaper = new System.Collections.Concurrent.ConcurrentBag<Tuple<double, string, int, int>>();
    int nFeas = 0, nInfeas = 0, near01 = 0;
    Parallel.ForEach(moves, new ParallelOptions { MaxDegreeOfParallelism = 8 }, mv => {
      int[] xx = (int[])x.Clone(); xx[mv[0]] = mv[1]; xx[mv[2]] = mv[3];
      Ex2Result r = prob.Evaluate(xx);
      if (r.T == 0.0) { System.Threading.Interlocked.Increment(ref nFeas);
        if (r.Cost.Total < c0 - 1e-6) cheaper.Add(Tuple.Create(r.Cost.Total, XStr(xx), mv[0], mv[2]));
        else if (r.Cost.Total <= c0 * 1.001) System.Threading.Interlocked.Increment(ref near01); }
      else System.Threading.Interlocked.Increment(ref nInfeas);
    });
    var sb = new StringBuilder();
    sb.AppendLine("# " + ver);
    sb.AppendLine("tool,scan2 (two-coordinate: every pair of variables i<j, every value pair with both values changed; single-coordinate moves are in *_scan.txt),fibre," + (prob.UseFibre ? 1 : 0) + ",fibreprice," + F(prob.Prices.Fibre, "R") + ",fRscale," + F(prob.FRScale, "R") + ",patterned,True,beamaxial,True,flexcheck,True,deflcheck,True,deflpattern,True");
    sb.AppendLine("base,cost," + F(c0, "F2") + ",T," + F(b.T, "R") + ",x," + XStr(x));
    sb.AppendLine("evaluations," + moves.Count + ",feasible," + nFeas + ",infeasible," + nInfeas + ",feasibleCheaper," + cheaper.Count + ",feasibleNotCheaperWithin0.1%," + near01);
    sb.AppendLine("two_coordinate_local_best," + (cheaper.Count == 0 ? "YES" : "NO"));
    foreach (var t in cheaper.OrderBy(t => t.Item1).Take(20)) sb.AppendLine("cheaper,vars " + t.Item3 + "&" + t.Item4 + "," + F(t.Item1, "F2") + "," + F(t.Item1 - c0, "F2") + ",x," + t.Item2);
    File.WriteAllText(outPath, sb.ToString());
  }

  // (2) patterned deflection
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
  public static void DeflPat(Ex2Problem prob, int[] x, string outPath, string ver) {
    Ex2Design d = prob.Decode(x); Ex2Result ev = prob.Evaluate(x); Analysis2D a = ev.Analysis; FrameSpec2D spec = prob.Spec; FemModel m = a.Frame.Model;
    var cases = new List<Tuple<string, List<int>>>();
    var all = new List<int>(); for (int ei = 0; ei < m.Elements.Count; ei++) if (!a.Frame.IsColumn(ei)) all.Add(ei);
    cases.Add(Tuple.Create("full (all spans, all storeys; = Deflection2D.Report)", all));
    foreach (Combo2D c in RcCombos2D.PatternCombos(a.Frame)) cases.Add(Tuple.Create(c.Name, c.QOn));
    Func<string, double> igOf = g => d.Beams[g].B * Math.Pow(d.Beams[g].H, 3) / 12.0;
    Func<string, double, double, double> ratioOf = (g, neg, pos) => 0.5 * (Deflection2D.Ie(d.Beams[g].B, d.Beams[g].H, ev.BeamRes[g].D, ev.BeamRes[g].AsTop, d.Beams[g].Fc, neg * 1e6) + Deflection2D.Ie(d.Beams[g].B, d.Beams[g].H, ev.BeamRes[g].D, ev.BeamRes[g].AsBot, d.Beams[g].Fc, pos * 1e6)) / igOf(g);
    var rD = new Dictionary<string, double>();
    foreach (string g in a.Frame.BeamGroups) { double n, p; Moments(a, g, new List<int>(), out n, out p); rD[g] = ratioOf(g, n, p); }
    // envelope Ie (smallest Ie/Ig of the group over all cases) for the conservative variant
    var rEnv = new Dictionary<string, double>(); var rCase = new List<Dictionary<string, double>>();
    foreach (var cs in cases) { var r = new Dictionary<string, double>();
      foreach (string g in a.Frame.BeamGroups) { double n, p; Moments(a, g, cs.Item2, out n, out p); r[g] = ratioOf(g, n, p); rEnv[g] = rEnv.ContainsKey(g) ? Math.Min(rEnv[g], r[g]) : r[g]; }
      rCase.Add(r); }
    FrameModel2D fmD; double[] uD = Solve(spec, d, rD, null, out fmD);
    var sb = new StringBuilder();
    sb.AppendLine("# " + ver);
    sb.AppendLine("# ACI 24.2 deflection under ACI 6.4.2 live-load patterns (REPORT, not a constraint). Same rules as Deflection2D.Report: Ie per group (24.2.3.5/24.2.3.6) from the group's largest service moments of THAT load case (variant 'env': smallest Ie/Ig of the group over all 25 cases), lambda = 2.0 (rho' = 0), sustained = D (SW + G), L transient; dAfter = 2 dD + dL; limits l/480, l/240, l/360 (L). mm.");
    sb.AppendLine("case,IeVariant,element,group,span_m,loadedL,dD,dDL,dL,dAfter,ratio480,ratio240,ratioL360");
    double w480 = 0, w240 = 0, w360 = 0, w480e = 0; string wc = "", wce = "";
    var summ = new StringBuilder();
    for (int ci_ = 0; ci_ < cases.Count; ci_++) {
      foreach (string variant in new[] { "case", "env" }) {
        var ratio = variant == "case" ? rCase[ci_] : rEnv;
        FrameModel2D fmL; double[] uL = Solve(spec, d, ratio, cases[ci_].Item2, out fmL);
        double c480 = 0;
        for (int ei = 0; ei < m.Elements.Count; ei++) {
          if (a.Frame.IsColumn(ei)) continue;
          var e = m.Elements[ei]; var bd = d.Beams[e.Group];
          double ecm = 4700.0 * Math.Sqrt(bd.Fc) * 1000.0, ig = bd.B * Math.Pow(bd.H, 3) / 12.0 * 1e-12, wSW = spec.ConcreteUnitWeight * bd.B * bd.H * 1e-6;
          bool loaded = cases[ci_].Item2.Contains(ei);
          double dD = Deflection2D.MidspanDeflection(new double[][] { uD }, fmD, ei, wSW + spec.DeadLine, ecm * ig * rD[e.Group]) * 1000.0;
          double dDL = Deflection2D.MidspanDeflection(new double[][] { uL }, fmL, ei, wSW + spec.DeadLine + (loaded ? spec.LiveLine : 0.0), ecm * ig * ratio[e.Group]) * 1000.0;
          double dL = dDL - dD, dA = 2.0 * dD + dL, L = e.Length * 1000.0;
          double r480 = dA / (L / 480.0), r240 = dA / (L / 240.0), r360 = dL / (L / 360.0);
          sb.AppendLine(string.Join(",", new[] { cases[ci_].Item1, variant, e.Name, e.Group, F(e.Length, "R"), loaded ? "1" : "0", F(dD, "F3"), F(dDL, "F3"), F(dL, "F3"), F(dA, "F3"), F(r480, "F4"), F(r240, "F4"), F(r360, "F4") }));
          c480 = Math.Max(c480, r480);
          if (variant == "case") { if (r480 > w480) { w480 = r480; wc = cases[ci_].Item1 + " " + e.Name; } w240 = Math.Max(w240, r240); w360 = Math.Max(w360, r360); }
          else if (r480 > w480e) { w480e = r480; wce = cases[ci_].Item1 + " " + e.Name; }
        }
        summ.AppendLine("case_worst," + cases[ci_].Item1 + "," + variant + ",ratio480," + F(c480, "F4"));
      }
    }
    sb.Append(summ.ToString());
    sb.AppendLine("worst_ratio(Ie per case),l480," + F(w480, "F4") + ",at," + wc + ",l240," + F(w240, "F4") + ",l360(L)," + F(w360, "F4") + ",verdict_l480," + (w480 <= 1.0 ? "OK" : "EXCEEDED"));
    sb.AppendLine("worst_ratio(Ie envelope),l480," + F(w480e, "F4") + ",at," + wce + ",verdict_l480," + (w480e <= 1.0 ? "OK" : "EXCEEDED"));
    File.WriteAllText(outPath, sb.ToString());
  }

  // (3) break-even over all fp0 final vectors and their fibre-group subsets
  public static void BreakEven(string[] files, double pCost, string outCsv, string outTxt, string ver) {
    var probs = new Dictionary<string, Ex2Problem>();
    string[] fmts = { "delta", "mc2010" }; double[] frs = { 0.6, 0.8, 1.0, 1.2 };
    foreach (string f in fmts) foreach (double r in frs) probs[f + "|" + F(r, "R")] = Make(true, 0.0, f, r);
    var vecs = new Dictionary<string, Tuple<int[], string>>();
    foreach (string fl in files) { int[] x = ReadX(fl); string k = XStr(x); if (!vecs.ContainsKey(k)) vecs[k] = Tuple.Create(x, Path.GetFileName(fl)); }
    var jobs = new List<Tuple<string, int[], string, int>>();
    foreach (var kv in vecs) {
      int[] x = kv.Value.Item1; var fg = new List<int>(); for (int g = 0; g < 3; g++) if (x[12 + 4 * g + 3] > 0) fg.Add(g);
      for (int mask = 1; mask < (1 << fg.Count); mask++) {
        int[] xx = (int[])x.Clone(); for (int t = 0; t < fg.Count; t++) if ((mask & (1 << t)) == 0) xx[12 + 4 * fg[t] + 3] = 0;
        string keep = string.Join("+", Enumerable.Range(0, fg.Count).Where(t => (mask & (1 << t)) != 0).Select(t => "B_S" + (fg[t] + 1)));
        foreach (string pk in probs.Keys) jobs.Add(Tuple.Create(pk, xx, kv.Value.Item2 + " keep " + keep, mask == (1 << fg.Count) - 1 ? 1 : 0));
      }
    }
    var res = new System.Collections.Concurrent.ConcurrentBag<string>();
    var best = new System.Collections.Concurrent.ConcurrentDictionary<string, Tuple<double, string>>();
    var bestFull = new System.Collections.Concurrent.ConcurrentDictionary<string, Tuple<double, string>>();
    Parallel.ForEach(jobs, new ParallelOptions { MaxDegreeOfParallelism = 8 }, j => {
      Ex2Result r = probs[j.Item1].Evaluate(j.Item2);
      double nf = r.Cost.Total - r.Cost.Fibre, kg = r.FibreKg, be = (r.T == 0.0 && kg > 0) ? (pCost - nf) / kg : double.NaN;
      res.Add(string.Join(",", new[] { j.Item1.Replace("|", ",fR="), j.Item3, j.Item4.ToString(), F(r.T, "R"), F(nf, "F2"), F(kg, "F2"), double.IsNaN(be) ? "na" : F(be, "F4"), XStr(j.Item2) }));
      if (!double.IsNaN(be)) {
        best.AddOrUpdate(j.Item1, Tuple.Create(be, j.Item3), (k, o) => be > o.Item1 ? Tuple.Create(be, j.Item3) : o);
        if (j.Item4 == 1) bestFull.AddOrUpdate(j.Item1, Tuple.Create(be, j.Item3), (k, o) => be > o.Item1 ? Tuple.Create(be, j.Item3) : o);
      }
    });
    File.WriteAllText(outCsv, "# " + ver + "\nformat,fRscale,design,fullVector,T,nonFibreCost_TL,fibreKg,breakEven_TLkg,x\n" + string.Join("\n", res.OrderBy(s => s)) + "\n");
    var sb = new StringBuilder();
    sb.AppendLine("# " + ver);
    sb.AppendLine("# single-design break-even fibre price b = (P - nonFibreCost) / fibreKg over " + vecs.Count + " distinct campaign-4 fp0 final vectors (" + files.Length + " runs) and all subsets of their fibre groups (others dose 0, sections fixed, steel re-derived); P = " + F(pCost, "F2") + " TL (cheapest plain design found; plain cost independent of fR and format). Feasible (T = 0) designs only. LOWER bound of the true break-even (no re-optimisation).");
    sb.AppendLine("format,fRscale,maxBreakEven_allSubsets_TLkg,argmax,maxBreakEven_fullVectors_TLkg,argmax_full,evaluations");
    foreach (string pk in probs.Keys) {
      Tuple<double, string> b, bf; best.TryGetValue(pk, out b); bestFull.TryGetValue(pk, out bf);
      sb.AppendLine(pk.Replace("|", ",") + "," + (b == null ? "na" : F(b.Item1, "F4")) + "," + (b == null ? "" : b.Item2) + "," + (bf == null ? "na" : F(bf.Item1, "F4")) + "," + (bf == null ? "" : bf.Item2) + "," + jobs.Count(t => t.Item1 == pk));
    }
    File.WriteAllText(outTxt, sb.ToString());
  }

  public static string Cardinality() {
    var sb = new StringBuilder();
    foreach (bool fib in new[] { false, true }) { Ex2Problem p = Make(fib, 90, "delta", 1.0); double n = 1;
      for (int i = 0; i < p.NVars; i++) n *= (p.Ub[i] - p.Lb[i] + 1);
      sb.AppendLine((fib ? "fibre" : "plain") + ",nVars," + p.NVars + ",raw_cardinality," + n.ToString("E4", ci) + ",ranges," + string.Join(";", Enumerable.Range(0, p.NVars).Select(i => (p.Ub[i] - p.Lb[i] + 1).ToString()))); }
    return sb.ToString();
  }
}
"@
$fw = [Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()
Add-Type -TypeDefinition $src -ReferencedAssemblies ($refs + "System.Core.dll" + (Join-Path $fw "netstandard.dll") + (Join-Path $fw "System.Runtime.dll")) -Language CSharp

$X = Join-Path $Out "x"
$sw = [Diagnostics.Stopwatch]::StartNew()
"# $ver`n" + [Ex2Extra]::Cardinality() | Set-Content -Encoding utf8 (Join-Path $Out "cardinality.txt")
$pPlain = [Ex2Extra]::Make($false, 90, "delta", 1.0)
$pF90 = [Ex2Extra]::Make($true, 90, "delta", 1.0)
$pF0 = [Ex2Extra]::Make($true, 0, "delta", 1.0)
$xP = [Ex2Extra]::ReadX((Join-Path $X "P.txt")); $xF90 = [Ex2Extra]::ReadX((Join-Path $X "F90.txt")); $xF0 = [Ex2Extra]::ReadX((Join-Path $X "F0.txt"))
$xF0Z = [Ex2Extra]::ReadX((Join-Path $X "F0Z.txt"))
[Ex2Extra]::DeflPat($pPlain, $xP, (Join-Path $Out "P_deflection_patterned.txt"), $ver)
[Ex2Extra]::DeflPat($pF0, $xF0, (Join-Path $Out "F0_deflection_patterned.txt"), $ver)
[Ex2Extra]::DeflPat($pF0, $xF0Z, (Join-Path $Out "F0Z_deflection_patterned.txt"), $ver)
"deflpat done " + $sw.Elapsed.TotalSeconds
$pCost = $pPlain.Evaluate($xP).Cost.Total
$files = Get-ChildItem (Join-Path $Camp "fibre_fp0") -Filter "EX2_*_run??.csv" | ForEach-Object FullName
[Ex2Extra]::BreakEven([string[]]$files, $pCost, (Join-Path $Out "breakeven_designs.csv"), (Join-Path $Out "breakeven_summary.txt"), $ver)
"breakeven done " + $sw.Elapsed.TotalSeconds
[Ex2Extra]::Scan2($pPlain, $xP, (Join-Path $Out "P_scan2.txt"), $ver)
"scan2 P done " + $sw.Elapsed.TotalSeconds
[Ex2Extra]::Scan2($pF0, $xF0, (Join-Path $Out "F0_scan2.txt"), $ver)
"scan2 F0 done " + $sw.Elapsed.TotalSeconds
[Ex2Extra]::Scan2($pF90, $xF90, (Join-Path $Out "F90_scan2.txt"), $ver)
"scan2 F90 done " + $sw.Elapsed.TotalSeconds
