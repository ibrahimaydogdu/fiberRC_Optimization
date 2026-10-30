"""Campaign statistics (AGENTS rule 8): per scenario and method best / mean / worst / std of the penalised cost Fpen over the independent runs (feasible = T = 0 listed separately),
two-sided Mann-Whitney U between methods inside a scenario and between scenarios (plain vs fibre, same method) with Holm correction.
Usage: python -I campaign_stats.py <campaign_dir> [out.md]
Run file name: <METHOD>_<plain|fibre>_fr<scale>_run<NN>.csv; line 3: 'Fpen,<v>,cost,<v>,T,<v>,...'."""
import sys, os, re, glob, itertools, statistics
from scipy.stats import mannwhitneyu

d = sys.argv[1]
out = sys.argv[2] if len(sys.argv) > 2 else None
runs = {}
vers = set()
for f in sorted(glob.glob(os.path.join(d, "*.csv"))):
    m = re.match(r"(YAKO|BTO|ATO)_(plain|fibre)_fr([0-9.]+)_run(\d+)\.csv$", os.path.basename(f))
    if not m:
        continue
    lines = open(f, encoding="utf-8").read().splitlines()
    vers.add(lines[0])
    kv = lines[2].split(",")
    row = {kv[i]: float(kv[i + 1]) for i in range(0, len(kv) - 1, 2)}
    meta = lines[1].split(",")
    row["FEs"] = float(meta[meta.index("FEs") + 1])
    runs.setdefault((m.group(2), m.group(3)), {}).setdefault(m.group(1), []).append(row)

md = ["# Campaign statistics", "", "Program versions in the run files: " + "; ".join(sorted(vers)), ""]
for scen in sorted(runs):
    md += ["## Scenario %s, fR x %s" % scen, "", "| Method | runs | feasible | best | mean | worst | std | FEs |", "|---|---|---|---|---|---|---|---|"]
    for meth in ("YAKO", "BTO", "ATO"):
        rs = runs[scen].get(meth, [])
        if not rs:
            continue
        v = [r["Fpen"] for r in rs]
        feas = sum(1 for r in rs if r["T"] == 0)
        md.append("| %s | %d | %d | %.0f | %.0f | %.0f | %.0f | %d |" % (meth, len(v), feas, min(v), statistics.mean(v), max(v), statistics.pstdev(v) if len(v) > 1 else 0.0, rs[0]["FEs"]))
    md.append("")
# tests
pairs = []
for scen in runs:
    for a, b in itertools.combinations([m for m in ("YAKO", "BTO", "ATO") if m in runs[scen]], 2):
        x = [r["Fpen"] for r in runs[scen][a]]; y = [r["Fpen"] for r in runs[scen][b]]
        if len(x) > 1 and len(y) > 1:
            pairs.append(("methods %s vs %s, %s fR %s" % (a, b, scen[0], scen[1]), x, y))
for meth in ("YAKO", "BTO", "ATO"):
    plain = [(s, runs[s][meth]) for s in runs if s[0] == "plain" and meth in runs[s]]
    for sp, rp in plain:
        for sf in [s for s in runs if s[0] == "fibre" and meth in runs[s]]:
            x = [r["Fpen"] for r in rp]; y = [r["Fpen"] for r in runs[sf][meth]]
            if len(x) > 1 and len(y) > 1:
                pairs.append(("%s plain vs fibre fR %s" % (meth, sf[1]), x, y))
res = []
for name, x, y in pairs:
    u, p = mannwhitneyu(x, y, alternative="two-sided")
    res.append([name, u, p, statistics.mean(x), statistics.mean(y)])
order = sorted(range(len(res)), key=lambda i: res[i][2])
m_ = len(res)
adj = [0.0] * m_
prev = 0.0
for rank, i in enumerate(order):
    a = min(1.0, (m_ - rank) * res[i][2])
    prev = max(prev, a)
    adj[i] = prev
md += ["## Mann-Whitney U (two-sided) with Holm correction (m = %d tests)" % m_, "", "| Comparison | U | p | p (Holm) | mean A | mean B |", "|---|---|---|---|---|---|"]
for i, r in enumerate(res):
    md.append("| %s | %.0f | %.4g | %.4g | %.0f | %.0f |" % (r[0], r[1], r[2], adj[i], r[3], r[4]))
text = "\n".join(md) + "\n"
if out:
    open(out, "w", encoding="utf-8").write(text)
print(text)
