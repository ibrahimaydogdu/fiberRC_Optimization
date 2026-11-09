"""Phase 2 statistics (PREDESIGN_PROTOCOL.md section 8, frozen before the runs).
Per scenario (plain/fibre), arm (A, C, ABC), method: best feasible cost (TL) at the total-FE checkpoints 1000/2000/3000/5000/10000 (pre-design FEs included; runs without a feasible design = +inf, censored),
median with bootstrap 95 % CI, relative median improvement between consecutive checkpoints, plateau = first checkpoint after which the next improvements are all < 1 %.
Tests at 10 000 FE: Mann-Whitney U (two-sided) + Holm, A12 (P first group cheaper): arms within scenario; fibre vs plain within arm. Baseline-A (random start, 44 variables, 10 000 FE) medians for reference.
Usage: python -I phase2_stats.py <phase2_dir> <baseline_campaign_dir> [out.md]"""
import sys, os, re, glob, statistics, itertools, random
from scipy.stats import mannwhitneyu

pdir, bdir = sys.argv[1], sys.argv[2]
out = sys.argv[3] if len(sys.argv) > 3 else None
INF = float("inf")
METHODS = ("YAKO", "BTO", "ATO")
CP = (1000, 2000, 3000, 5000, 10000)
random.seed(12345)


def kv(line):
    p = line.split(",")
    return {p[i]: p[i + 1] for i in range(0, len(p) - 1, 2)}


def load(path):
    L = open(path, encoding="utf-8").read().splitlines()
    meta = kv(L[1])
    pre = int(meta.get("predesignEvals", 0))
    fh = next(l for l in L if l.startswith("feasible_history")).split(",", 1)[1]
    hist = []
    if fh.strip():
        for t in fh.split(";"):
            a, b = t.split(":")
            hist.append((int(a) + pre, float(b)))
    res = kv(L[2])
    return hist, float(res["cost"]) if float(res["T"]) == 0.0 else INF


def at(hist, c):
    best = INF
    for fe, v in hist:
        if fe <= c:
            best = min(best, v)
    return best


runs = {}
for d in sorted(glob.glob(os.path.join(pdir, "*_*"))):
    m = re.match(r"(plain|fibre)_(A|C|ABC)$", os.path.basename(d))
    if not m:
        continue
    for f in sorted(glob.glob(os.path.join(d, "*.csv"))):
        mm = re.match(r"(YAKO|BTO|ATO)_(plain|fibre)_fr[0-9.]+_run(\d+)\.csv$", os.path.basename(f))
        if not mm:
            continue
        hist, final = load(f)
        runs.setdefault((m.group(1), m.group(2), mm.group(1)), {})[int(mm.group(3))] = {c: at(hist, c) for c in CP}

base = {}
for f in glob.glob(os.path.join(bdir, "*_fr1.00_run*.csv")):
    mm = re.match(r"(YAKO|BTO|ATO)_(plain|fibre)_fr1.00_run(\d+)\.csv$", os.path.basename(f))
    if mm:
        L = open(f, encoding="utf-8").read().splitlines()
        r = kv(L[2])
        base.setdefault((mm.group(2), mm.group(1)), []).append(float(r["cost"]) if float(r["T"]) == 0.0 else INF)

allbest = [v[10000] for d in runs.values() for v in d.values() if v[10000] < INF] + [x for v in base.values() for x in v if x < INF]
BKS = min(allbest)


def fmt(x):
    return "inf" if x == INF else "%.0f" % x


def med(v):
    return statistics.median(v)


def boot(v, n=2000):
    ms = sorted(med([random.choice(v) for _ in v]) for _ in range(n))
    return ms[int(0.025 * n)], ms[int(0.975 * n)]


def a12(x, y):
    gt = sum(1 for a in x for b in y if a < b) + 0.5 * sum(1 for a in x for b in y if a == b)
    return gt / (len(x) * len(y))


md = ["# Phase 2 statistics (10 000 FE total, pre-design FEs included; PCR-12 one-dimension moves for BTO/ATO)", "",
      "Best known (this study, feasible, not an optimum): **%s TL**. Plateau = checkpoint after which every later checkpoint improves the median by < 1 %% (a settling point, not an optimum)." % fmt(BKS), ""]
for scn in ("plain", "fibre"):
    md += ["## Scenario %s" % scn, "",
           "| Arm | Method | n | infeasible at 10 000 | median 1000 | 2000 | 3000 | 5000 | 10000 | 95 % CI (10000) | best | worst | impr. 1k→2k | 2k→3k | 3k→5k | 5k→10k | plateau | gap median to best known (10000) |",
           "|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|"]
    for arm in ("A", "C", "ABC"):
        for meth in METHODS:
            r = runs.get((scn, arm, meth))
            if not r:
                continue
            cols = {c: [x[c] for x in r.values()] for c in CP}
            meds = [med(cols[c]) for c in CP]
            imp = [(meds[i] - meds[i + 1]) / meds[i] * 100 if meds[i] < INF and meds[i + 1] < INF else float("nan") for i in range(4)]
            plateau = "not reached"
            for i in range(4):
                if all(imp[j] < 1.0 for j in range(i, 4)):
                    plateau = str(CP[i])
                    break
            lo, hi = boot(cols[10000])
            fin = [v for v in cols[10000] if v < INF]
            md.append("| %s | %s | %d | %d | %s | %s | %s | %s | %s | %s–%s | %s | %s | %s | %s | %s | %s | %s | %.1f %% |" % (
                arm, meth, len(r), len(cols[10000]) - len(fin), *[fmt(m) for m in meds], fmt(lo), fmt(hi), fmt(min(fin)) if fin else "inf", fmt(max(fin)) if fin else "inf",
                *["%.2f %%" % i for i in imp], plateau, (meds[4] / BKS - 1) * 100))
    md += ["", "Baseline-A (random start, 44 variables, 10 000 FE, feasible-run median): " + ", ".join("%s %s (n=%d)" % (m, fmt(med([x for x in base.get((scn, m), [INF]) if x < INF] or [INF])), len(base.get((scn, m), []))) for m in METHODS), ""]

tests = []
for scn in ("plain", "fibre"):
    for a, b in itertools.combinations(("A", "C", "ABC"), 2):
        for meth in METHODS + ("pooled",):
            x = [v[10000] for m in (METHODS if meth == "pooled" else (meth,)) for v in runs[(scn, a, m)].values()]
            y = [v[10000] for m in (METHODS if meth == "pooled" else (meth,)) for v in runs[(scn, b, m)].values()]
            tests.append(["%s %s: %s vs %s" % (scn, meth, a, b), mannwhitneyu(x, y, alternative="two-sided").pvalue, a12(x, y)])
for arm in ("A", "C", "ABC"):
    for meth in METHODS + ("pooled",):
        x = [v[10000] for m in (METHODS if meth == "pooled" else (meth,)) for v in runs[("fibre", arm, m)].values()]
        y = [v[10000] for m in (METHODS if meth == "pooled" else (meth,)) for v in runs[("plain", arm, m)].values()]
        tests.append(["arm %s %s: fibre vs plain" % (arm, meth), mannwhitneyu(x, y, alternative="two-sided").pvalue, a12(x, y)])
order = sorted(range(len(tests)), key=lambda i: tests[i][1])
adj = [0.0] * len(tests)
prev = 0.0
for rank, i in enumerate(order):
    prev = max(prev, min(1.0, (len(tests) - rank) * tests[i][1]))
    adj[i] = prev
md += ["## Tests at 10 000 FE (Mann-Whitney U, two-sided, Holm over m = %d; A12 = P(first group cheaper))" % len(tests), "", "| Comparison | p | p (Holm) | A12 |", "|---|---|---|---|"]
for i, t in enumerate(tests):
    md.append("| %s | %.4g | %.4g | %.2f |" % (t[0], t[1], adj[i], t[2]))
text = "\n".join(md) + "\n"
if out:
    open(out, "w", encoding="utf-8").write(text)
print(text)
