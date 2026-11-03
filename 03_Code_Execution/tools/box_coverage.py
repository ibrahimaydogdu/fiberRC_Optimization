import sys, glob, os, re
d = sys.argv[1]
UB_COL = [8, 8, 12, 1]
UB_BEAM = [5, 10, 1]
def ubs(fibre):
    u = []
    for _ in range(6): u += UB_COL
    for _ in range(4): u += UB_BEAM + [1 if fibre else 0, 11 if fibre else 0]  # dose max replaced below
    return u
runs = {"plain": [], "fibre": []}
for f in glob.glob(os.path.join(d, "*_fr1.00_run*.csv")):
    m = re.match(r"(YAKO|BTO|ATO)_(plain|fibre)_", os.path.basename(f))
    L = open(f, encoding="utf-8").read().splitlines()
    kv = L[2].split(",")
    r = {kv[i]: float(kv[i + 1]) for i in range(0, len(kv) - 1, 2)}
    x = [int(v) for v in L[3].split(",")[1:]]
    runs[m.group(2)].append((r["Fpen"], r["T"], x, m.group(1), f))
for sc in runs:
    rs = sorted([r for r in runs[sc] if r[1] == 0], key=lambda r: r[0])
    n = len(rs[0][2])
    # pool size per variable: use max observed index over ALL runs as lower estimate only when Ub unknown; columns/beams from Ex1Problem
    ub = []
    for _ in range(6): ub += UB_COL
    for _ in range(4): ub += [5, 10, 1, 1 if sc == "fibre" else 0, max(r[2][len(ub) + 4] for r in runs[sc]) if sc == "fibre" else 0]
    assert len(ub) == n, (len(ub), n)
    print("==", sc, "feasible", len(rs), "of", len(runs[sc]), "vars", n)
    for k in (10, 30):
        top = rs[:k]
        lo = [min(r[2][i] for r in top) for i in range(n)]
        hi = [max(r[2][i] for r in top) for i in range(n)]
        frac = sum((hi[i] - lo[i] + 1) / (ub[i] + 1) for i in range(n)) / n
        print(" top%d box mean width fraction %.3f (narrowing %.0f%%)" % (k, frac, 100 * (1 - frac)))
        # leave-one-out coverage of the best 10 by box of the others in top-k (margin 0 and -2/+3)
        for mg in ((0, 0), (2, 3)):
            cov = 0
            for j in range(min(10, k)):
                oth = [r for i, r in enumerate(top) if i != j]
                lo2 = [max(0, min(r[2][i] for r in oth) - mg[0]) for i in range(n)]
                hi2 = [min(ub[i], max(r[2][i] for r in oth) + mg[1]) for i in range(n)]
                ok = all(lo2[i] <= rs[j][2][i] <= hi2[i] for i in range(n))
                cov += ok
            fr2 = sum((min(ub[i], max(r[2][i] for r in top) + mg[1]) - max(0, min(r[2][i] for r in top) - mg[0]) + 1) / (ub[i] + 1) for i in range(n)) / n
            print("   margin -%d/+%d: LOO coverage of best %d = %d/%d, box width fraction %.3f (narrowing %.0f%%)" % (mg[0], mg[1], min(10, k), cov, min(10, k), fr2, 100 * (1 - fr2)))
