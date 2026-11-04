"""Ablation Phase 1 statistics (PREDESIGN_PROTOCOL.md section 7, frozen before the results).
Per scenario (plain / fibre), arm (R, A, C, ABC) and method (YAKO, BTO, ATO): runs, feasible at the end, best / median / mean / worst / std of the best feasible cost at the total FE budget
(runs without a feasible design are censored as +inf and excluded from mean/std, counted separately), first feasible FE, feasible evaluations among the first 100, success rates against
H1 (Baseline-A median at 10 000 FE of the same method), H2 / H3 (best known +5 % / +2 %; best known = cheapest feasible design found in this study, not an optimum).
Tests: Mann-Whitney U (two-sided) between arms (pooled over methods and per method) with Holm correction, A12 effect size, paired Wilcoxon (same method and run index = same seed) as sensitivity.
Phase 2 arm rule: the arm (A, C, ABC) with the lowest pooled median over all methods, per scenario.
Usage: python -I ablation_stats.py <ablation_dir> <baseline_campaign_dir> [out.md]"""
import sys, os, re, glob, math, statistics, itertools
from scipy.stats import mannwhitneyu, wilcoxon

adir, bdir = sys.argv[1], sys.argv[2]
out = sys.argv[3] if len(sys.argv) > 3 else None
INF = float("inf")
METHODS = ("YAKO", "BTO", "ATO")


def kv(line):
    p = line.split(",")
    d = {}
    for i in range(0, len(p) - 1, 2):
        d[p[i]] = p[i + 1]
    return d


def parse(path):
    L = open(path, encoding="utf-8").read().splitlines()
    meta = kv(L[1])
    fs = next((kv(l.split(",", 1)[1]) for l in L if l.startswith("feas_summary")), None)
    res = kv(L[2])
    return meta, fs, res


runs = {}  # (scn, arm, method) -> {run: dict}
for d in sorted(glob.glob(os.path.join(adir, "*_*"))):
    m = re.match(r"(plain|fibre)_(R|A|C|ABC)$", os.path.basename(d))
    if not m:
        continue
    for f in sorted(glob.glob(os.path.join(d, "*.csv"))):
        mm = re.match(r"(YAKO|BTO|ATO)_(plain|fibre)_fr[0-9.]+_run(\d+)\.csv$", os.path.basename(f))
        if not mm:
            continue
        meta, fs, res = parse(f)
        best = INF
        first = None
        f100 = None
        if fs is not None:
            best = float(fs["bestFeasible"]) if fs["bestFeasible"] != "none" else INF
            first = int(fs["firstFeasibleFE"]) if int(fs["firstFeasibleFE"]) > 0 else None
            f100 = int(fs["feasibleInFirst100"])
        else:  # legacy file without PCR-11 lines: feasible only if T == 0
            best = float(res["cost"]) if float(res["T"]) == 0.0 else INF
        runs.setdefault((m.group(1), m.group(2), mm.group(1)), {})[int(mm.group(3))] = {
            "best": best, "first": first, "f100": f100, "pre": int(meta.get("predesignEvals", 0)), "fe": int(meta["FEs"])}

# Baseline-A (10 000 FE, fR 1.00) medians per scenario and method, feasible runs only
base = {}
for f in glob.glob(os.path.join(bdir, "*_fr1.00_run*.csv")):
    mm = re.match(r"(YAKO|BTO|ATO)_(plain|fibre)_fr1.00_run(\d+)\.csv$", os.path.basename(f))
    if not mm:
        continue
    meta, fs, res = parse(f)
    base.setdefault((mm.group(2), mm.group(1)), []).append(float(res["cost"]) if float(res["T"]) == 0.0 else INF)
H1 = {k: statistics.median(v) for k, v in base.items()}
allbest = [r["best"] for v in runs.values() for r in v.values() if r["best"] < INF] + [x for v in base.values() for x in v if x < INF]
BKS = min(allbest) if allbest else INF


def fmt(x):
    return "inf" if x == INF else "%.0f" % x


def stat(vals):
    fin = [v for v in vals if v < INF]
    med = statistics.median(vals) if vals else INF
    return (len(vals), len(fin), min(fin) if fin else INF, med, statistics.mean(fin) if fin else INF, max(fin) if fin else INF, statistics.pstdev(fin) if len(fin) > 1 else 0.0)


def a12(x, y):
    # probability that a value of x is smaller than one of y (lower cost = better), ties count half
    gt = sum(1 for a in x for b in y if a < b) + 0.5 * sum(1 for a in x for b in y if a == b)
    return gt / (len(x) * len(y))


md = ["# Ablation Phase 1 statistics (1 000 FE total, pre-design FEs included)", "",
      "Best known (this study, feasible, not an optimum): **%s TL**." % fmt(BKS), ""]
scns = sorted({k[0] for k in runs})
pool = {}
for scn in scns:
    md += ["## Scenario %s" % scn, "",
           "| Arm | Method | runs | feasible | best | median | mean | worst | std | median first feasible FE | feasible in first 100 (mean) | pre-design FEs | success H1 | H2 (+5 %) | H3 (+2 %) |",
           "|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|"]
    for arm in ("R", "A", "C", "ABC"):
        allv = []
        for meth in METHODS:
            r = runs.get((scn, arm, meth))
            if not r:
                continue
            v = [x["best"] for x in r.values()]
            allv += v
            n, nf, b, med, mean, w, sd = stat(v)
            ff = [x["first"] for x in r.values() if x["first"]]
            f1 = [x["f100"] for x in r.values() if x["f100"] is not None]
            pre = sorted({x["pre"] for x in r.values()})
            h1 = H1.get((scn, meth), INF)
            md.append("| %s | %s | %d | %d | %s | %s | %s | %s | %.0f | %s | %s | %s | %.2f | %.2f | %.2f |" % (
                arm, meth, n, nf, fmt(b), fmt(med), fmt(mean), fmt(w), sd,
                ("%.0f" % statistics.median(ff)) if ff else "none", ("%.1f" % statistics.mean(f1)) if f1 else "n/a", "/".join(map(str, pre)),
                sum(1 for x in v if x <= h1) / n, sum(1 for x in v if x <= BKS * 1.05) / n, sum(1 for x in v if x <= BKS * 1.02) / n))
        if allv:
            pool[(scn, arm)] = allv
    md.append("")
    md += ["Baseline-A (10 000 FE, random start, feasible-run median, H1 targets): " + ", ".join("%s %s" % (m, fmt(H1.get((scn, m), INF))) for m in METHODS), ""]
    cand = {a: statistics.median(pool[(scn, a)]) for a in ("A", "C", "ABC") if (scn, a) in pool}
    if cand:
        sel = min(cand, key=cand.get)
        md += ["Pooled median (all methods, +inf censored): " + ", ".join("%s %s" % (a, fmt(v)) for a, v in cand.items()) + (", R %s" % fmt(statistics.median(pool[(scn, "R")])) if (scn, "R") in pool else ""),
               "**Phase 2 arm by the frozen rule: %s** (runs counted: %s)." % (sel, ", ".join("%s %d" % (a, len(pool[(scn, a)])) for a in cand)), ""]

# tests between arms
tests = []
for scn in scns:
    for a, b in itertools.combinations(("R", "A", "C", "ABC"), 2):
        if (scn, a) in pool and (scn, b) in pool:
            x, y = pool[(scn, a)], pool[(scn, b)]
            u, p = mannwhitneyu(x, y, alternative="two-sided")
            tests.append(["%s pooled %s vs %s" % (scn, a, b), p, a12(x, y), "pooled"])
            for meth in METHODS:
                ra, rb = runs.get((scn, a, meth)), runs.get((scn, b, meth))
                if ra and rb and len(ra) > 1 and len(rb) > 1:
                    x, y = [r["best"] for r in ra.values()], [r["best"] for r in rb.values()]
                    u, p = mannwhitneyu(x, y, alternative="two-sided")
                    common = sorted(set(ra) & set(rb))
                    pw = float("nan")
                    d = [ra[k]["best"] - rb[k]["best"] for k in common if ra[k]["best"] < INF and rb[k]["best"] < INF]
                    if len(d) > 5 and any(abs(t) > 0 for t in d):
                        pw = wilcoxon(d).pvalue
                    tests.append(["%s %s: %s vs %s (paired Wilcoxon p=%.3g)" % (scn, meth, a, b, pw), p, a12(x, y), meth])
order = sorted(range(len(tests)), key=lambda i: tests[i][1])
m_ = len(tests)
adj = [0.0] * m_
prev = 0.0
for rank, i in enumerate(order):
    prev = max(prev, min(1.0, (m_ - rank) * tests[i][1]))
    adj[i] = prev
md += ["## Mann-Whitney U (two-sided) with Holm correction (m = %d tests); A12 = P(first arm cheaper)" % m_, "", "| Comparison | p | p (Holm) | A12 |", "|---|---|---|---|"]
for i, t in enumerate(tests):
    md.append("| %s | %.4g | %.4g | %.2f |" % (t[0], t[1], adj[i], t[2]))
text = "\n".join(md) + "\n"
if out:
    open(out, "w", encoding="utf-8").write(text)
print(text)
