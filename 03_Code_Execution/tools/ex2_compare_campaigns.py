"""Example 2 (Ö2): campaign 1 (pre-registered, superseded) <-> campaign 2 (post-hoc PCR-23/24/25, EX2_SPEC §11.2) comparison, per cell x method.
Reads the per-run CSVs written by ex2_stats.py (no re-evaluation). Descriptive only: the two series use different design rules (patterns, beam axial
tension, g8 / consistent SFRC capacity), so differences are "effect of the post-hoc rule change + search", not a test. No between-method ranking.
Usage: python -I ex2_compare_campaigns.py <runs_campaign1.csv> <runs_campaign2.csv> <out.md>"""
import sys, csv, statistics
from collections import Counter

sys.stdout.reconfigure(encoding="utf-8")
c1, c2, out = sys.argv[1], sys.argv[2], sys.argv[3]
CELLS = [("plain", "lifsiz"), ("fibre_fp90", "lifli 90 TL/kg"), ("fibre_fp0", "lifli 0 TL/kg")]
METHODS = ["YAKO", "BTO", "ATO"]


def load(p):
    d = {}
    with open(p, encoding="utf-8") as f:
        for r in csv.DictReader(f):
            d.setdefault((r["cell"], r["method"]), []).append(r)
    return d


A, B = load(c1), load(c2)


def beams(eff):
    return "; ".join(s.strip() for s in eff.split(";") if s.strip().startswith("B_"))


def fibre_pattern(R):
    cnt = Counter(beams(r["effective_design"]) for r in R)
    k, n = cnt.most_common(1)[0]
    return "%s (%d/30)" % (k, n)


md = ["# Ö2 kampanya 1 ↔ kampanya 2 karşılaştırması (betimsel; post-hoc etkisi)", "",
      "Üretici: `03_Code_Execution/tools/ex2_compare_campaigns.py` (otomatik). Kaynak: kampanya 1 `%s`, kampanya 2 `%s` (her ikisi `ex2_stats.py` koşu satırları). "
      "Kampanya 1 = ön kayıtlı seri (ikili Ex2Model `d0514e2f…`; desenli yük yok, kiriş eksenel kuvveti yok, SFRC bayat-lcs hatası; **superseded**); kampanya 2 = post-hoc §11.2 (ikili `a61b0c6b…`; PCR-23/24/25). "
      "İki seri farklı tasarım kurallarıyla arandığından fark \"kural değişikliği + arama\" etkisidir; test yapılmadı, yöntem sıralaması yok." % (c1.replace("\\", "/").split("04_Results/")[-1], c2.replace("\\", "/").split("04_Results/")[-1]), "",
      "## 1. Hücre medyanları ve en düşükler (TL)", "",
      "| Hücre | Yöntem | Medyan K1 | Medyan K2 | Δ medyan (%) | En düşük K1 | En düşük K2 | Δ en düşük (%) | Lif > 0 koşu K1 | Lif > 0 koşu K2 | Lif kg medyanı K1 / K2 |",
      "|---|---|---|---|---|---|---|---|---|---|---|"]
for cell, lab in CELLS:
    for m in METHODS:
        a = A[(cell, m)]; b = B[(cell, m)]
        va = [float(r["cost"]) for r in a]; vb = [float(r["cost"]) for r in b]
        ma, mb = statistics.median(va), statistics.median(vb)
        la, lb = min(va), min(vb)
        fa = sum(int(r["nFibreGroups"]) > 0 for r in a); fb = sum(int(r["nFibreGroups"]) > 0 for r in b)
        ka = statistics.median(float(r["fibreKg"]) for r in a); kb = statistics.median(float(r["fibreKg"]) for r in b)
        md.append("| %s | %s | %.2f | %.2f | %+.3f | %.2f | %.2f | %+.3f | %s | %s | %s |" % (
            lab, m, ma, mb, (mb - ma) / ma * 100, la, lb, (lb - la) / la * 100,
            "—" if cell == "plain" else "%d/30" % fa, "—" if cell == "plain" else "%d/30" % fb,
            "—" if cell == "plain" else "%.0f / %.0f" % (ka, kb)))
md += ["", "## 2. Lif kararı (ön kayıt kural 4 göstergeleri) ve senaryo farkı", "",
       "| Yöntem | Lifsiz → 0 TL/kg medyan farkı K1 (%) | K2 (%) | Lifsiz → 90 TL/kg medyan farkı K1 (%) | K2 (%) | 0 TL/kg en sık kiriş deseni K1 | K2 |",
       "|---|---|---|---|---|---|---|"]
for m in METHODS:
    def d(S, x, y):
        mx = statistics.median(float(r["cost"]) for r in S[(x, m)]); my = statistics.median(float(r["cost"]) for r in S[(y, m)])
        return (my - mx) / mx * 100
    md.append("| %s | %+.3f | %+.3f | %+.3f | %+.3f | %s | %s |" % (m, d(A, "plain", "fibre_fp0"), d(B, "plain", "fibre_fp0"), d(A, "plain", "fibre_fp90"), d(B, "plain", "fibre_fp90"),
                                                         fibre_pattern(A[("fibre_fp0", m)]), fibre_pattern(B[("fibre_fp0", m)])))
md += ["", "## 3. Hücre en düşük etkin tasarımları (BTO/ATO/YAKO birleşik; ilk dosya)", "",
       "| Hücre | K1 en düşük (TL) ve etkin tasarım | K2 en düşük (TL) ve etkin tasarım |", "|---|---|---|"]
for cell, lab in CELLS:
    ra = min((r for m in METHODS for r in A[(cell, m)]), key=lambda r: (float(r["cost"]), r["file"]))
    rb = min((r for m in METHODS for r in B[(cell, m)]), key=lambda r: (float(r["cost"]), r["file"]))
    md.append("| %s | %.2f — %s (`%s`) | %.2f — %s (`%s`) |" % (lab, float(ra["cost"]), ra["effective_design"], ra["file"], float(rb["cost"]), rb["effective_design"], rb["file"]))
md.append("")
text = "\n".join(md) + "\n"
open(out, "w", encoding="utf-8").write(text)
print(text)
