"""PCR-23/24/25 (2026-11-17): summary of the --evalonly re-evaluation of the 270 campaign-1 final vectors (tools/ex2_reeval_pcr23_25.sh).
Per vector and rule set (old = campaign-1 binary; p23 = new binary, campaign-1 load rules + PCR-23 incl. g8 (--flexcheck 1 default);
new = campaign-2 rules): cost, T, violated constraints, beam flexural D/C (max over groups), fibre kg.
Usage: python -I ex2_reeval_summary.py <reeval_dir> <out.md> <out.csv>"""
import sys, os, glob, statistics, collections
sys.stdout.reconfigure(encoding="utf-8")

d, out_md, out_csv = sys.argv[1], sys.argv[2], sys.argv[3]
SETS = ["old", "p23", "new"]


def parse(path):
    r = dict(viol=[], dc={}, fibre=0.0)
    for line in open(path, encoding="utf-8"):
        f = line.rstrip("\n").split(",")
        if f[0] == "Fpen":
            kv = dict(zip(f[0::2], f[1::2])); r["cost"] = float(kv["cost"]); r["T"] = float(kv["T"])
        elif f[0] == "g" and len(f) >= 3:
            if float(f[-1]) > 0: r["viol"].append((f[1], float(f[-1])))
        elif f[0] == "beam" and len(f) > 2 and f[1].startswith("B_"):
            kv = dict(zip(f[0::2], f[1::2])); r["dc"][f[1]] = float(kv["DCflex"])
            if "dose" in kv: r["fibre"] += float(kv["dose"]) > 0
    return r


rows = []
for x in sorted(glob.glob(os.path.join(d, "x", "*.txt"))):
    n = os.path.basename(x)[:-4]
    cell, run = n.split("__")
    rr = dict(cell=cell, run=run)
    for s in SETS:
        p = os.path.join(d, s, n + ".txt")
        rr[s] = parse(p) if os.path.exists(p) and os.path.getsize(p) > 0 else None
    rows.append(rr)

CELLS = [("plain", "lifsiz"), ("fibre_fp90", "lifli 90 TL/kg"), ("fibre_fp0", "lifli 0 TL/kg")]
md = ["# Kampanya 1 son vektörlerinin düzeltilmiş kodla deterministik yeniden değerlendirmesi (PCR-23/24/25)", "",
      "Üretici: `03_Code_Execution/tools/ex2_reeval_summary.py` (otomatik). Girdi: `04_Results/ex2/reeval_pcr23_25/{x,old,p23,new}/` (`tools/ex2_reeval_pcr23_25.sh`). "
      "**old** = kampanya 1 ikilisi (`ex2_campaign_bin`, d0514e2f…); **p23** = kampanya 2 ikilisi (`ex2_campaign2_bin`, 74d94426…) kampanya 1 yük kurallarıyla "
      "(`--patterned 0 --beamaxial 0`; PCR-23 (1)–(3) açık: tutarlı SFRC kapasitesi, g8, düz beton RequiredAs düzeltmesi); **new** = kampanya 2 kuralları (PCR-23 + PCR-24 eksenel çekme + PCR-25 desenli zarf). "
      "Yeniden optimizasyon DEĞİLDİR: kesitler sabit, donatı/etriye/kolon ρ yeniden türetilir. İstatistik tablolarının yerine geçmez (`EX2_SPEC.md` §11.2).", ""]
md += ["| Hücre | Kural seti | n | T > 0 (uygunsuz) | En çok ihlal edilen kısıt (koşu) | Kiriş eğilme D/C en büyük | D/C > 1 + 10⁻⁶ koşu | Maliyet değişimi medyan (min–maks), % (old'a göre) | En düşük maliyet (TL) |",
       "|---|---|---|---|---|---|---|---|---|"]
for cell, lab in CELLS:
    R = [r for r in rows if r["cell"] == cell]
    base = {r["run"]: r["old"]["cost"] for r in R if r["old"]}
    for s in SETS:
        V = [r for r in R if r[s]]
        if not V: continue
        nT = sum(r[s]["T"] > 0 for r in V)
        cnt = collections.Counter(v[0].split(" (")[0] for r in V for v in r[s]["viol"])
        mdc = max(max(r[s]["dc"].values()) for r in V)
        ndc = sum(max(r[s]["dc"].values()) > 1 + 1e-6 for r in V)
        ch = [(r[s]["cost"] / base[r["run"]] - 1) * 100 for r in V if r["run"] in base]
        feas = [r[s]["cost"] for r in V if r[s]["T"] == 0]
        md.append("| %s | %s | %d | %d | %s | %.4f | %d | %s | %s |" % (
            lab, s, len(V), nT, "; ".join("%s (%d)" % kv for kv in cnt.most_common(3)) or "—", mdc, ndc,
            "%.3f (%.3f – %.3f)" % (statistics.median(ch), min(ch), max(ch)) if ch else "—",
            ("%.2f (T = 0)" % min(feas)) if feas else "uygun yok"))
md.append("")
# focus: campaign-1 lowest designs per cell
md += ["## Hücre en düşük (kampanya 1) tasarımları", "", "| Hücre | Koşu | old maliyet / T / D/C | p23 maliyet / T / D/C | new maliyet / T / D/C | new ihlaller |", "|---|---|---|---|---|---|"]
for cell, lab in CELLS:
    R = [r for r in rows if r["cell"] == cell and r["old"] and r["old"]["T"] == 0]
    lo = min(r["old"]["cost"] for r in R)
    B = [r for r in R if abs(r["old"]["cost"] - lo) < 0.005]
    r = sorted(B, key=lambda r: r["run"])[0]
    f = lambda q: ("%.2f / %.4g / %.4f" % (q["cost"], q["T"], max(q["dc"].values()))) if q else "—"
    md.append("| %s | `%s` (+%d aynı maliyetli) | %s | %s | %s | %s |" % (lab, r["run"], len(B) - 1, f(r["old"]), f(r["p23"]), f(r["new"]),
                                                                    "; ".join("%s = %.4f" % v for v in r["new"]["viol"]) if r["new"] else "—"))
md.append("")
open(out_md, "w", encoding="utf-8").write("\n".join(md) + "\n")
with open(out_csv, "w", encoding="utf-8") as fo:
    fo.write("cell,run," + ",".join("%s_cost,%s_T,%s_maxDCflex,%s_violations" % (s, s, s, s) for s in SETS) + "\n")
    for r in rows:
        parts = [r["cell"], r["run"]]
        for s in SETS:
            q = r[s]
            parts += (["%.6f" % q["cost"], "%g" % q["T"], "%.6f" % max(q["dc"].values()), '"' + "; ".join("%s=%.6g" % v for v in q["viol"]) + '"'] if q else ["", "", "", ""])
        fo.write(",".join(parts) + "\n")
print("\n".join(md))
