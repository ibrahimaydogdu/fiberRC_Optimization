"""Example 2 (Ö2) campaign statistics, pre-registered in EX2_SPEC.md §11.1 (rules 1-8) and §11 (fibre-output hierarchy).
Descriptive per cell (scenario x method): n, FEs check, T = 0 count, best / mean / median / worst / std of the final best cost (TL),
runs with fibre dose > 0, fibre kg, number of distinct EFFECTIVE best designs (decoded: column b,h; beam b,h; fibre type + repaired dose only
when dose > 0), runs reaching the cell's lowest cost, storeys classified sway (Q > 0.05) in the run's best design.
Tests (pre-registered §11.1 rule 8: fibre <-> plain and fp0 <-> fp90): WITHIN each method only, Mann-Whitney U two-sided, one Holm family
over all 9 tests (3 methods x {plain vs fp90, plain vs fp0, fp90 vs fp0}), A12 = P(first group cheaper), bootstrap 95 % CI (2000 resamples,
independent groups, seed 2468) of the median difference (second - first, TL and %). NO between-method tests (AGENTS rule 8; rule 7).
Cross-check (no ranking): do the three methods' lowest-cost designs coincide (effective-design identity, Hamming distance on raw x)?
2026-11-17 (Critic K3 / ön kayıt kabul denetimi): violation_counts (search-time evaluations violating each constraint, summed over the runs of a cell)
is reported in §1.1; tests whose two groups are all identical (ties) are written "uygulanamaz (bağ)" instead of p = 1 (Holm keeps p = 1 for them).
Usage: python -I ex2_stats.py <campaign_dir> [out.md] [runs.csv] [label]   (label: e.g. "campaign2"; default: campaign 1 wording)"""
import sys, os, re, glob, statistics, random
from scipy.stats import mannwhitneyu

sys.stdout.reconfigure(encoding="utf-8")
camp = sys.argv[1]
out = sys.argv[2] if len(sys.argv) > 2 else None
runs_csv = sys.argv[3] if len(sys.argv) > 3 else None
label = sys.argv[4] if len(sys.argv) > 4 else ""
random.seed(2468)
med = statistics.median
BP = [250, 300, 350, 400, 450]; HP = [300, 350, 400, 450, 500, 550, 600]
DOSES = [0, 20, 25, 30, 35, 40]; TYPES = ["4D", "5D"]; DR = {"4D": (20, 40), "5D": (25, 35)}
COLS = ["C_Edge_S1", "C_Edge_S2", "C_Edge_S3", "C_Inter_S1", "C_Inter_S2", "C_Inter_S3"]; BEAMS = ["B_S1", "B_S2", "B_S3"]
CELLS = [("plain", "plain"), ("fibre_fp90", "fibre 90 TL/kg"), ("fibre_fp0", "fibre 0 TL/kg")]
METHODS = ["YAKO", "BTO", "ATO"]


def kv(l):
    p = l.rstrip("\n").split(",")
    return {p[i]: p[i + 1] for i in range(0, len(p) - 1, 2)}


def effective(x, fibre):
    """decoded design: tuple of (group, b, h[, type, dose]) with repaired dose; type ignored when dose = 0"""
    d, k = [], 0
    for g in COLS:
        d.append((g, BP[x[k]], HP[x[k + 1]])); k += 2
    for g in BEAMS:
        if fibre:
            dose = DOSES[x[k + 3]]; t = TYPES[x[k + 2]]
            if dose > 0:
                lo, hi = DR[t]; dose = max(lo, min(hi, dose))
                d.append((g, BP[x[k]], HP[x[k + 1]], t, dose))
            else:
                d.append((g, BP[x[k]], HP[x[k + 1]], "-", 0))
            k += 4
        else:
            d.append((g, BP[x[k]], HP[x[k + 1]], "-", 0)); k += 2
    return tuple(d)


def fmt_eff(e):
    return "; ".join("%s %dx%d" % (t[0], t[1], t[2]) + (" %s/%g" % (t[3], t[4]) if len(t) > 3 and t[4] > 0 else "") for t in e)


def load(path):
    L = open(path, encoding="utf-8").read().splitlines()
    h = kv(L[1]); c = kv(L[2])
    x = [int(v) for v in L[3].split(",")[1:]]
    st = next(l for l in L if l.startswith("stability"))
    q = [float(m) for m in re.findall(r"Q=([0-9.]+)", st)]
    sway = len(re.findall(r"\(SWAY\)", st))
    fibre = h["fibre"] == "1"
    vl = next((l for l in L if l.startswith("violation_counts")), "violation_counts")
    viol = {}
    for item in vl.split(",")[1:]:
        if "=" in item:
            k, v = item.rsplit("=", 1); viol[k] = int(v)
    return dict(viol=viol, file=os.path.relpath(path, camp).replace("\\", "/"), method=h["method"], run=int(h["run"]), seed=int(h["seed"]), FEs=int(h["FEs"]),
                cost=float(c["cost"]), T=float(c["T"]), fibreKg=float(c["fibreKg"]), fibreTL=float(c["fibreCost"]), x=x,
                eff=effective(x, fibre), nFibreGroups=(sum(1 for g in range(3) if x[12 + 4 * g + 3] > 0) if fibre else 0),
                Q=q, nSway=sway, drift=[float(m) for m in re.findall(r"drift_mm=([-0-9.]+)", st)])


def a12(x, y):
    return (sum(1 for a in x for b in y if a < b) + 0.5 * sum(1 for a in x for b in y if a == b)) / (len(x) * len(y))


def boot_diff(a, b, n=2000):
    r = []
    for _ in range(n):
        ma = med([random.choice(a) for _ in a]); mb = med([random.choice(b) for _ in b])
        r.append(mb - ma)
    r.sort()
    return r[int(.025 * n)], r[int(.975 * n) - 1]


data = {}
for cell, _ in CELLS:
    for f in sorted(glob.glob(os.path.join(camp, cell, "EX2_*_run??.csv"))):
        r = load(f)
        data.setdefault((cell, r["method"]), []).append(r)

campRel = "04_Results/ex2/" + os.path.basename(os.path.normpath(camp))
md = ["# Örnek 2 (Ö2) kampanya istatistikleri (betimsel; ön kayıt EX2_SPEC.md §11.1%s)" % (", " + label if label else ""), "",
      "Üretici: `03_Code_Execution/tools/ex2_stats.py` (bu dosya otomatik üretilir; elle düzenlenmez). Kaynak: `" + campRel + "/{plain,fibre_fp90,fibre_fp0}/EX2_*_run??.csv` "
      "(her dosyanın 3. satırı `cost`, `T`, `fibreKg`; 4. satırı `x`; `stability` satırı kat Q). Koşu başına satırlar: `" +
      (("04_Results/ex2/" + os.path.basename(os.path.dirname(os.path.abspath(runs_csv))) + "/" + os.path.basename(runs_csv)) if runs_csv else "04_Results/ex2/analysis/ex2_runs.csv") + "`.", "",
      "Kurallar: algoritma üstünlüğü testi yok (AGENTS kural 8, §11.1 kural 7); \"optimum\" yok; maliyet = son en iyi uygun çözümün toplam maliyeti (TL, ÇŞİDB 2026). "
      "Lifli uzay lifsiz uzayı kapsadığından lifli ↔ lifsiz test \"arama verimliliği\" ile \"lif etkisi\"ni ayıramaz (§11 madde 2).", "",
      "## 1. Kabul / kalite denetimi (§11.1)", "",
      "| Hücre | Yöntem | n | FEs = 10 000 | T = 0 (uygun) |", "|---|---|---|---|---|"]
for cell, lab in CELLS:
    for m in METHODS:
        R = data.get((cell, m), [])
        md.append("| %s | %s | %d | %d/%d | %d/%d |" % (lab, m, len(R), sum(r["FEs"] == 10000 for r in R), len(R), sum(r["T"] == 0 for r in R), len(R)))
md += ["", "### 1.1 `violation_counts` (ön kayıt kabul denetimi; arama süresince kısıtı ihlal eden değerlendirme sayısı, hücre × yöntem 30 koşu toplamı)", "",
       "Bu sayılar en iyi çözüme değil, arama sırasında değerlendirilen tüm tasarımlara aittir (her koşuda 10 000 FE). En iyi çözümler §1'de T = 0 sütunuyla denetlenir.", ""]
vnames = sorted({k for R in data.values() for r in R for k in r["viol"]})
short = {n: n.split(" (")[0] for n in vnames}
md += ["| Hücre | Yöntem | " + " | ".join(short[n] for n in vnames) + " |", "|---|---|" + "---|" * len(vnames)]
for cell, lab in CELLS:
    for m in METHODS:
        R = data.get((cell, m), [])
        md.append("| %s | %s | " % (lab, m) + " | ".join("%d (%d koşu)" % (sum(r["viol"].get(n, 0) for r in R), sum(1 for r in R if r["viol"].get(n, 0) > 0)) for n in vnames) + " |")
md += ["", "Sütun adları (tam): " + "; ".join("`%s`" % n for n in vnames) + ".", ""]
md += ["", "## 2. Hücre başına betimsel istatistik (TL; uygun koşular)", "",
       "| Hücre | Yöntem | n | En iyi | Ortalama | Medyan | Kötü | Std. sapma | Lif > 0 koşu | Lif kg (min–maks) | Farklı etkin en iyi tasarım | Yöntemin kendi en düşüğüne ulaşan koşu |" + (" Farklı ham x |" if label else ""),
       "|---|---|---|---|---|---|---|---|---|---|---|---|" + ("---|" if label else "")]
for cell, lab in CELLS:
    for m in METHODS:
        R = [r for r in data[(cell, m)] if r["T"] == 0]
        v = [r["cost"] for r in R]
        lo = min(v)
        fk = [r["fibreKg"] for r in R]
        md.append("| %s | %s | %d | %.2f | %.2f | %.2f | %.2f | %.2f | %s | %s | %d | %d |" % (
            lab, m, len(v), lo, statistics.mean(v), med(v), max(v), statistics.stdev(v),
            ("%d/%d" % (sum(r["nFibreGroups"] > 0 for r in R), len(R))) if cell != "plain" else "—",
            ("%.0f–%.0f" % (min(fk), max(fk))) if cell != "plain" else "—",
            len(set(r["eff"] for r in R)), sum(abs(r["cost"] - lo) < 0.005 for r in R)) + ((" %d |" % len(set(tuple(r["x"]) for r in R))) if label else ""))
md.append("")

# fibre run detail
md += ["### 2.1 Lif > 0 olan koşular (hücre, yöntem, koşu, maliyet, lif kg, kiriş grubu tip/doz)", ""]
for cell, lab in CELLS[1:]:
    for m in METHODS:
        F = [r for r in data[(cell, m)] if r["nFibreGroups"] > 0]
        if cell == "fibre_fp0":
            from collections import Counter
            cnt = Counter(tuple((t[0], t[3], t[4]) for t in r["eff"][6:]) for r in F)
            md.append("- %s %s: %d/30 koşuda lif > 0; kiriş lif deseni sayımı: %s" % (lab, m, len(F), "; ".join("%s ×%d" % (" ".join("%s %s/%g" % z for z in k), c) for k, c in cnt.most_common())))
        else:
            md.append("- %s %s: %d/30 koşuda lif > 0%s" % (lab, m, len(F), ("; " + "; ".join("run%02d %.2f TL, %.0f kg (%s)" % (r["run"], r["cost"], r["fibreKg"], ", ".join("%s %s/%g" % (t[0], t[3], t[4]) for t in r["eff"][6:] if t[4] > 0)) for r in F)) if F else ""))
md.append("")

# tests within method
tests = []
pairs = [("plain", "fibre_fp90"), ("plain", "fibre_fp0"), ("fibre_fp90", "fibre_fp0")]
for m in METHODS:
    for a, b in pairs:
        A = [r["cost"] for r in data[(a, m)] if r["T"] == 0]; B = [r["cost"] for r in data[(b, m)] if r["T"] == 0]
        tie = len(set(A + B)) == 1
        if tie:
            p = 1.0
        else:
            p = mannwhitneyu(A, B, alternative="two-sided").pvalue
        lo, hi = boot_diff(A, B)
        tests.append(dict(tie=tie, m=m, a=a, b=b, nA=len(A), nB=len(B), medA=med(A), medB=med(B), p=p, a12=a12(A, B), lo=lo, hi=hi))
order = sorted(range(len(tests)), key=lambda i: tests[i]["p"])
prev = 0.0
for rank, i in enumerate(order):
    prev = max(prev, min(1.0, (len(tests) - rank) * tests[i]["p"])); tests[i]["holm"] = prev
lab = dict(CELLS)
md += ["## 3. Yöntem içi senaryo karşılaştırması (Mann–Whitney U iki yönlü; Holm, tek aile m = %d)" % len(tests), "",
       "A12 = P(birinci grubun maliyeti < ikinci grubun). Fark = medyan(ikinci) − medyan(birinci); GA95 = bağımsız bootstrap (2 000, tohum 2468). "
       "İki grubun tüm değerleri aynı ise (bağ; 30/30 aynı son çözüm) test **uygulanamaz** ve p/A12 sütunlarına \"uygulanamaz (bağ)\" yazılır (Holm hesabında p = 1); "
       "standart sapması 0 olan hücrelerde p ve A12 etki büyüklüğü değil, \"30/30 aynı son çözüm\" bilgisidir. fp0 ↔ fp90 testi farklı amaç fonksiyonlarını karşılaştırır "
       "(lif fiyatı maliyete girer); ön kayıtta olduğu için verilir, yorum için kullanılmaz. Yöntemler arası test YAPILMADI.", "",
       "| Yöntem | Birinci | İkinci | n1 | n2 | Medyan 1 (TL) | Medyan 2 (TL) | Fark (TL) | Fark (%) | GA95 fark (TL) | p | p (Holm) | A12 |",
       "|---|---|---|---|---|---|---|---|---|---|---|---|---|"]
for t in tests:
    if t["tie"]:
        md.append("| %s | %s | %s | %d | %d | %.2f | %.2f | %.2f | %.3f | %.2f … %.2f | uygulanamaz (bağ) | uygulanamaz (bağ) | uygulanamaz (bağ) |" % (
            t["m"], lab[t["a"]], lab[t["b"]], t["nA"], t["nB"], t["medA"], t["medB"], t["medB"] - t["medA"], (t["medB"] - t["medA"]) / t["medA"] * 100, t["lo"], t["hi"]))
        continue
    md.append("| %s | %s | %s | %d | %d | %.2f | %.2f | %.2f | %.3f | %.2f … %.2f | %.3g | %.3g | %.3f |" % (
        t["m"], lab[t["a"]], lab[t["b"]], t["nA"], t["nB"], t["medA"], t["medB"], t["medB"] - t["medA"], (t["medB"] - t["medA"]) / t["medA"] * 100, t["lo"], t["hi"], t["p"], t["holm"], t["a12"]))
md.append("")

# cross-check
md += ["## 4. Çapraz teyit (sıralama değil): üç yöntemin en düşük maliyetli tasarımları", "",
       "Etkin tasarım = kolon b×h; kiriş b×h; lif yalnız doz > 0 ise (tip/onarılmış doz). Hamming = ham x vektörlerinde farklı gen sayısı.", ""]
for cell, labc in CELLS:
    md += ["### %s" % labc, "", "| Yöntem | En düşük maliyet (TL) | Bu maliyete ulaşan koşu | Etkin tasarım | Kaynak dosya (ilk) |", "|---|---|---|---|---|"]
    bests = {}
    for m in METHODS:
        R = [r for r in data[(cell, m)] if r["T"] == 0]
        lo = min(r["cost"] for r in R)
        B = [r for r in R if abs(r["cost"] - lo) < 0.005]
        bests[m] = B[0]
        effs = set(r["eff"] for r in B)
        md.append("| %s | %.2f | %d | %s%s | `%s` |" % (m, lo, len(B), fmt_eff(B[0]["eff"]), (" (+%d diğer etkin tasarım aynı maliyette)" % (len(effs) - 1)) if len(effs) > 1 else "", B[0]["file"]))
    same = len(set(bests[m]["eff"] for m in METHODS)) == 1
    ham = {(a, b): sum(1 for i, j in zip(bests[a]["x"], bests[b]["x"]) if i != j) for a in METHODS for b in METHODS if a < b}
    md += ["", "Üç yöntemin en düşük tasarımı etkin olarak aynı mı: **%s**. Ham x Hamming: %s." % ("EVET" if same else "HAYIR", ", ".join("%s–%s %d" % (a, b, v) for (a, b), v in ham.items())), ""]
    allR = [r for m in METHODS for r in data[(cell, m)] if r["T"] == 0]
    glo = min(r["cost"] for r in allR)
    md.append("Hücre genel en düşük: %.2f TL. Genel en düşüğe %%0,1 / %%0,5 / %%1 içinde kalan koşu (yöntem başına, /30): %s." % (
        glo, "; ".join("%s %s" % (m, "/".join(str(sum(r["cost"] <= glo * (1 + t / 100) for r in data[(cell, m)] if r["T"] == 0)) for t in (0.1, 0.5, 1.0))) for m in METHODS)))
    md.append("")

# stability
md += ["## 5. Kat stabilitesi (PCR-22; §A.5: kaç en iyi tasarımda ötelenmeli kat çıktığı)", "",
       "| Hücre | Yöntem | Ötelenmeli katı olan koşu | Ötelenmeli kat sayısı dağılımı (0/1/2/3) | Q en büyük (tüm koşular) | Kat ötelenmesi en büyük mutlak değer (mm) |", "|---|---|---|---|---|---|"]
for cell, labc in CELLS:
    for m in METHODS:
        R = data[(cell, m)]
        md.append("| %s | %s | %d/%d | %s | %.4f | %.4f |" % (labc, m, sum(r["nSway"] > 0 for r in R), len(R), "/".join(str(sum(r["nSway"] == k for r in R)) for k in range(4)),
                                                               max(max(r["Q"]) for r in R), max(max(abs(d) for d in r["drift"]) for r in R)))
md.append("")

text = "\n".join(md) + "\n"
if out:
    open(out, "w", encoding="utf-8").write(text)
if runs_csv:
    with open(runs_csv, "w", encoding="utf-8") as f:
        f.write("cell,method,run,seed,FEs,T,cost,fibreKg,fibreTL,nFibreGroups,nSwayStoreys,Q_S1,Q_S2,Q_S3,x,effective_design,file\n")
        for cell, _ in CELLS:
            for m in METHODS:
                for r in sorted(data[(cell, m)], key=lambda r: r["run"]):
                    f.write("%s,%s,%d,%d,%d,%g,%.6f,%.2f,%.2f,%d,%d,%s,%s,\"%s\",%s\n" % (cell, m, r["run"], r["seed"], r["FEs"], r["T"], r["cost"], r["fibreKg"], r["fibreTL"], r["nFibreGroups"], r["nSway"],
                                                                                        ",".join("%.4f" % q for q in r["Q"]), ";".join(map(str, r["x"])), fmt_eff(r["eff"]), r["file"]))
print(text)
