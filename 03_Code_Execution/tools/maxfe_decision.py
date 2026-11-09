"""Pre-registered MaxFE rule (PREDESIGN_PROTOCOL.md section 9, frozen before the 20 000 FE runs).
A cell (scenario, arm, method; 10 runs, seeds 1001-1010, 20 000 FE total) is 'settled' if
 (i) median best feasible cost at 20 000 improved by < 1 % relative to the median at 10 000 (same runs, same history), and
 (ii) the median improved by < 0.5 % between 15 000 and 20 000, and
 (iii) the upper limit of the bootstrap 95 % CI of the 10 000 -> 20 000 median improvement is <= 1.5 %.
If >= 6 of the 8 cells are settled: MaxFE = 20 000, stop. Otherwise the unsettled cells are re-run with 30 000 FE (30 000 is the upper limit; no further extension).
Usage: python -I maxfe_decision.py <dir_20k> <out_decision.txt>   (writes 'SETTLED' or lines 'RERUN scn arm method')"""
import sys, os, re, glob, statistics, random
d, out = sys.argv[1], sys.argv[2]
random.seed(777)
INF = float("inf")


def kv(l):
    p = l.split(",")
    return {p[i]: p[i + 1] for i in range(0, len(p) - 1, 2)}


def load(path):
    L = open(path, encoding="utf-8").read().splitlines()
    pre = int(kv(L[1]).get("predesignEvals", 0))
    fh = next(l for l in L if l.startswith("feasible_history")).split(",", 1)[1]
    h = [(int(t.split(":")[0]) + pre, float(t.split(":")[1])) for t in fh.split(";")] if fh.strip() else []
    return h


def at(h, c):
    b = INF
    for fe, v in h:
        if fe <= c:
            b = min(b, v)
    return b


cells = {}
for sd in glob.glob(os.path.join(d, "*_*")):
    m = re.match(r"(plain|fibre)_(A|ABC)$", os.path.basename(sd))
    if not m:
        continue
    for f in glob.glob(os.path.join(sd, "*.csv")):
        mm = re.match(r"(YAKO|ATO)_", os.path.basename(f))
        if mm:
            cells.setdefault((m.group(1), m.group(2), mm.group(1)), []).append(load(f))
lines, settled, report = [], 0, []
for k in sorted(cells):
    hs = cells[k]
    v10 = [at(h, 10000) for h in hs]; v15 = [at(h, 15000) for h in hs]; v20 = [at(h, 20000) for h in hs]
    med = statistics.median
    i1 = (med(v10) - med(v20)) / med(v10) * 100
    i2 = (med(v15) - med(v20)) / med(v15) * 100
    bs = []
    for _ in range(2000):
        idx = [random.randrange(len(hs)) for _ in hs]
        a, b = med([v10[i] for i in idx]), med([v20[i] for i in idx])
        bs.append((a - b) / a * 100)
    bs.sort(); up = bs[int(0.975 * len(bs))]
    ok = len(hs) >= 10 and i1 < 1.0 and i2 < 0.5 and up <= 1.5
    settled += ok
    report.append("%s %s %s n=%d med10k=%.0f med15k=%.0f med20k=%.0f impr10-20=%.2f%% impr15-20=%.2f%% CIup=%.2f%% settled=%s" % (k + (len(hs), med(v10), med(v15), med(v20), i1, i2, up, ok)))
    if not ok:
        lines.append("RERUN %s %s %s" % k)
res = ["# settled cells: %d of %d" % (settled, len(cells))] + report
res.append("SETTLED" if settled >= 6 and len(cells) == 8 else "NOT_SETTLED")
res += lines if not (settled >= 6 and len(cells) == 8) else []
open(out, "w", encoding="utf-8").write("\n".join(res) + "\n")
print("\n".join(res))
