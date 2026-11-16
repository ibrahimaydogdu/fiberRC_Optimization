"""Phase 3 statistics (PREDESIGN_PROTOCOL.md section 9). Descriptive; no superiority claims.
Part 1: MaxFE curves. Per cell (scenario, arm, method; 10 runs, same seeds at 20k and 30k) median best feasible cost at total-FE checkpoints
10000/15000/20000/25000/30000 (pre-design FEs included; no feasible design = inf), improvement between checkpoints, bootstrap 95 % CI of 10k->30k improvement.
Cells settled at 20k have no 30k run (20k values only).
Part 2: BTO all-dimension (Phase 3 stage 3, arms A and C, 30 runs, 10 000 FE) vs Phase 2 BTO one-dimension (PCR-12) in the same arm at 10 000 FE and
vs Baseline-A BTO (random start, 44 variables): Mann-Whitney U two-sided + Holm, A12 (P first group cheaper), medians.
Usage: python -I phase3_stats.py <phase3_dir> <phase2_dir> <baseline_dir> [out.md]"""
import sys, os, re, glob, statistics, random
from scipy.stats import mannwhitneyu
p3, p2, bd = sys.argv[1:4]
out = sys.argv[4] if len(sys.argv) > 4 else None
INF = float("inf")
random.seed(2468)
med = statistics.median
CP = (10000, 15000, 20000, 25000, 30000)


def kv(l):
    p = l.split(",")
    return {p[i]: p[i + 1] for i in range(0, len(p) - 1, 2)}


def load(path):
    L = open(path, encoding="utf-8").read().splitlines()
    pre = int(kv(L[1]).get("predesignEvals", 0))
    fh = next(l for l in L if l.startswith("feasible_history")).split(",", 1)[1]
    h = [(int(t.split(":")[0]) + pre, float(t.split(":")[1])) for t in fh.split(";")] if fh.strip() else []
    r = kv(L[2])
    return h, (float(r["cost"]) if float(r["T"]) == 0.0 else INF)


def at(h, c):
    b = INF
    for fe, v in h:
        if fe <= c:
            b = min(b, v)
    return b


def fmt(x):
    return "inf" if x == INF else "%.0f" % x


def a12(x, y):
    return (sum(1 for a in x for b in y if a < b) + 0.5 * sum(1 for a in x for b in y if a == b)) / (len(x) * len(y))


def boot_impr(v0, v1, n=2000):
    r = []
    for _ in range(n):
        i = [random.randrange(len(v0)) for _ in v0]
        a, b = med([v0[k] for k in i]), med([v1[k] for k in i])
        r.append((a - b) / a * 100)
    r.sort()
    return r[int(.025 * n)], r[int(.975 * n)]


# ---- Part 1
cells = {}
for stage in ("20k", "30k"):
    for sd in sorted(glob.glob(os.path.join(p3, stage, "*_*"))):
        m = re.match(r"(plain|fibre)_(A|ABC)$", os.path.basename(sd))
        if not m:
            continue
        for f in sorted(glob.glob(os.path.join(sd, "*.csv"))):
            mm = re.match(r"(YAKO|ATO)_.*_run(\d+)\.csv$", os.path.basename(f))
            if mm:
                h, _ = load(f)
                cells.setdefault((m.group(1), m.group(2), mm.group(1)), {}).setdefault(stage, {})[int(mm.group(2))] = h
md = ["# Phase 3 statistics (descriptive; pre-design FEs included in totals)", "",
      "## Part 1: MaxFE curves (10 runs per cell, seeds identical at 20k and 30k)", "",
      "Plateau is not an optimum. Cells settled at 20k (pre-registered rule) were not re-run at 30k.", "",
      "| Scenario | Arm | Method | n(20k) | n(30k) | med 10k | 15k | 20k | 25k | 30k | impr 10→20 | 20→30 | 25→30 | CI95 impr 10→30 | infeasible at last cp | status |",
      "|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|"]
for k in sorted(cells):
    c = cells[k]
    has30 = "30k" in c and len(c["30k"]) > 0
    src = c["30k"] if has30 else c["20k"]
    last = 30000 if has30 else 20000
    cps = [x for x in CP if x <= last]
    cols = {x: [at(h, x) for h in src.values()] for x in cps}
    ms = {x: med(cols[x]) for x in cps}
    def imp(a, b):
        return "%.2f %%" % ((ms[a] - ms[b]) / ms[a] * 100) if a in ms and b in ms and ms[a] < INF and ms[b] < INF else "-"
    ci = "-"
    if has30 and ms[10000] < INF and ms[30000] < INF:
        lo, hi = boot_impr(cols[10000], cols[30000]); ci = "%.2f–%.2f %%" % (lo, hi)
    inf_n = sum(1 for v in cols[last] if v == INF)
    md.append("| %s | %s | %s | %d | %d | %s | %s | %s | %s | %s | %s | %s | %s | %s | %d | %s |" % (
        k[0], k[1], k[2], len(c.get("20k", {})), len(c.get("30k", {})), *[fmt(ms[x]) if x in ms else "-" for x in CP],
        imp(10000, 20000), imp(20000, 30000), imp(25000, 30000), ci, inf_n, "30k run" if has30 else "settled at 20k"))
md.append("")

# ---- Part 2
def cost10k(paths):
    return [at(load(f)[0], 10000) for f in paths]

ref = {}
for meth in ("BTO", "ATO"):
    sub = meth.lower() + "_alldim"
    for scn in ("plain", "fibre"):
        for arm in ("A", "C", "ABC"):
            ad = cost10k(sorted(glob.glob(os.path.join(p3, sub, "%s_%s" % (scn, arm), "%s_*.csv" % meth))))
            od = cost10k(sorted(glob.glob(os.path.join(p2, "%s_%s" % (scn, arm), "%s_*.csv" % meth))))
            ref[(meth, scn, arm)] = (ad, od)
base = {}
for meth in ("BTO", "ATO"):
    for scn in ("plain", "fibre"):
        base[(meth, scn)] = []
        for f in sorted(glob.glob(os.path.join(bd, "**", "%s_%s_fr1.00_run*.csv" % (meth, scn)), recursive=True)):
            r = kv(open(f, encoding="utf-8").read().splitlines()[2])
            base[(meth, scn)].append(float(r["cost"]) if float(r["T"]) == 0.0 else INF)
tests, rows = [], []
for (meth, scn, arm), (ad, od) in sorted(ref.items()):
    if not ad or not od:
        continue
    rows.append("| %s | %s | %s | %d | %s | %d | %s | %.1f %% | %d | %d |" % (meth, scn, arm, len(ad), fmt(med(ad)), len(od), fmt(med(od)), (med(od) - med(ad)) / med(od) * 100, sum(v == INF for v in ad), sum(v == INF for v in od)))
    tests.append(["%s %s %s: all-dim vs one-dim (Phase 2)" % (meth, scn, arm), mannwhitneyu(ad, od, alternative="two-sided").pvalue, a12(ad, od)])
md += ["## Part 2: all-dimension vs one-dimension (PCR-12) moves, BTO and ATO, 10 000 FE total", "",
       "Supplementary (IA decision 2026-11-09/13). Cells with n < 30 are partial. Baseline-A (random start, 44 variables, other build) is context only, not in the Holm family.", "",
       "| Method | Scenario | Arm | n all-dim | median all-dim | n one-dim | median one-dim | one-dim dearer by | infeasible all-dim | infeasible one-dim |", "|---|---|---|---|---|---|---|---|---|---|"] + rows + [""]
md += ["Baseline-A medians (feasible runs, context only): " + ", ".join("%s %s %s (n=%d)" % (k[0], k[1], fmt(med([x for x in v if x < INF] or [INF])), len(v)) for k, v in sorted(base.items())), ""]
order = sorted(range(len(tests)), key=lambda i: tests[i][1])
adj = [0.0] * len(tests); prev = 0.0
for rank, i in enumerate(order):
    prev = max(prev, min(1.0, (len(tests) - rank) * tests[i][1])); adj[i] = prev
md += ["| Comparison | p | p (Holm, m=%d) | A12 (P all-dim cheaper) |" % len(tests), "|---|---|---|---|"]
for i, t in enumerate(tests):
    md.append("| %s | %.4g | %.4g | %.2f |" % (t[0], t[1], adj[i], t[2]))
text = "\n".join(md) + "\n"
if out:
    open(out, "w", encoding="utf-8").write(text)
print(text)
