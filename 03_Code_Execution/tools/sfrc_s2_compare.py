"""S2: SFRC beam flexural capacity model (same model as RcCore.SfrcBeam, verified against it to 0.001 % by sfrc_independent_check.py)
against the measured moments of Nogales et al. (2025), 01_Literature/SFRC_BEAM_TESTS.csv. Mean material values (gammaF = 1, no phi).
Usage: python -I sfrc_s2_compare.py SFRC_BEAM_TESTS.csv > output"""
import csv, sys, math

EPS_CU, ES = 0.003, 200000.0

def beta1(fc):
    return 0.85 if fc <= 28 else (0.65 if fc >= 55 else 0.85 - 0.05 * (fc - 28) / 7)

def sigma(w, fr1, fr3, wu, gamma):
    fts = 0.45 * fr1
    ftu = max(0.0, fts - wu / 2.5 * (fts - 0.5 * fr3 + 0.2 * fr1))
    return 0.0 if w > wu else (fts - (fts - ftu) * w / wu) / gamma

def tension(c, h, lcs, fr1, fr3, gamma, n=4000):
    wu = min(lcs * 0.02, 2.5)
    ymax = h - c
    if ymax <= 0 or fr1 <= 0:
        return 0.0, 0.0
    dy = ymax / n
    f1 = f2 = 0.0
    for i in range(n):
        y = (i + 0.5) * dy
        s = sigma(lcs * EPS_CU * y / c, fr1, fr3, wu, gamma)
        f1 += s * dy; f2 += s * y * dy
    return f1, f2

def flex(b, h, d, As, fc, fy, fr1, fr3, gamma, lcs_mode, fs_cap):
    b1 = beta1(fc)
    lcs = h
    c = 0.1 * d
    for outer in range(8):
        def bal(c):
            cc = 0.85 * fc * b * min(b1 * c, h)
            fs = min(fs_cap, ES * EPS_CU * (d - c) / c)
            f1, _ = tension(c, h, lcs, fr1, fr3, gamma)
            return cc - As * fs - f1 * b
        lo, hi = 1e-4 * d, h
        for _ in range(50):
            mid = 0.5 * (lo + hi)
            if bal(mid) > 0: hi = mid
            else: lo = mid
        c = 0.5 * (lo + hi)
        if lcs_mode == "h": break
        lcs = max(h - c, 0.2 * h)
    a = min(b1 * c, h)
    fs = min(fs_cap, ES * EPS_CU * (d - c) / c)
    f1, f2 = tension(c, h, lcs, fr1, fr3, gamma)
    return As * fs * (d - a / 2) + b * ((c - a / 2) * f1 + f2), c

rows = [r for r in csv.DictReader(open(sys.argv[1], encoding="utf-8")) if r["study"] == "Nogales2025"]
print("Nogales et al. (2025), 9 beams; Mu,exp = 0.45 Pmax (self weight excluded); Mn model: mean fR, gammaF=1, ACI block, steel yield fy=503 MPa or bar tensile strength 644 MPa.")
print("d (mm) = h - 35 - 8 - 4 = 353 (cover 35 mm to stirrup, Phi8 stirrup, Phi8 bar) ; sensitivity d = 365.")
print("%-13s %7s %7s %8s %8s %8s %8s %8s" % ("beam", "Mexp", "d", "Mn,fy", "ratio", "Mn,ft", "ratio", "c/d"))
res = {"fy": [], "ft": []}
for r in rows:
    b, h, As, fy, fc = float(r["b_mm"]), float(r["h_mm"]), float(r["As_mm2"]), float(r["fy_MPa"]), float(r["fc_MPa"])
    fr1 = float(r["fR1_MPa"]) if r["fR1_MPa"] else 0.0
    fr3 = float(r["fR3_MPa"]) if r["fR3_MPa"] else 0.0
    mexp = float(r["Mu_exp_kNm"])
    for d in (353.0, 365.0):
        m1, c1 = flex(b, h, d, As, fc, fy, fr1, fr3, 1.0, "hc", fy)
        m2, c2 = flex(b, h, d, As, fc, fy, fr1, fr3, 1.0, "hc", 644.0)
        print("%-13s %7.1f %7.0f %8.1f %8.3f %8.1f %8.3f %8.3f" % (r["beam_id"], mexp, d, m1 / 1e6, m1 / 1e6 / mexp, m2 / 1e6, m2 / 1e6 / mexp, c1 / d))
        if d == 353.0:
            res["fy"].append(m1 / 1e6 / mexp); res["ft"].append(m2 / 1e6 / mexp)
for k, v in res.items():
    print("%s: n=%d mean ratio %.3f min %.3f max %.3f" % (k, len(v), sum(v) / len(v), min(v), max(v)))
