"""Independent re-implementation of the SFRC beam flexure used in RcCore.SfrcBeam (V3 prerequisite, Critic Z7).
Same model assumptions (ACI 318M-14 rectangular compression block, bilinear steel, fib MC2010 linear sigma-w tension law with
fFts=0.45 fR1, fFtu from Eq. 5.6-6, wu=min(lcs*0.02, 2.5 mm), crack width w = lcs*eps_cu*y/c, gamma_F=1.5), but solved by
brute-force numerical integration (slices) and bisection, not by the closed forms of the VB code. It checks the algebra,
integrals and solver; it does NOT validate the model against tests (that needs literature experiments, listed separately).
Usage: python -I sfrc_independent_check.py grid.csv"""
import csv, sys, math

EPS_CU, ES = 0.003, 200000.0

def beta1(fc):
    return 0.85 if fc <= 28 else (0.65 if fc >= 55 else 0.85 - 0.05 * (fc - 28) / 7)

def fib_sigma(w, fr1, fr3, wu, gamma=1.5):
    fts = 0.45 * fr1
    ftu = max(0.0, fts - wu / 2.5 * (fts - 0.5 * fr3 + 0.2 * fr1))
    if w > wu:
        return 0.0
    return (fts - (fts - ftu) * w / wu) / gamma

def tension(c, h, lcs, fr1, fr3, n=20000):
    wu = min(lcs * 0.02, 2.5)
    ymax = h - c
    if ymax <= 0:
        return 0.0, 0.0
    f1 = f2 = 0.0
    dy = ymax / n
    for i in range(n):
        y = (i + 0.5) * dy
        s = fib_sigma(lcs * EPS_CU * y / c, fr1, fr3, wu)
        f1 += s * dy
        f2 += s * y * dy
    return f1, f2

def flex(b, h, d, As, fc, fy, fr1, fr3, lcs):
    b1 = beta1(fc)
    def bal(c):
        cc = 0.85 * fc * b * min(b1 * c, h)
        fs = min(fy, ES * EPS_CU * (d - c) / c)
        f1, _ = tension(c, h, lcs, fr1, fr3, 4000)
        return cc - As * fs - f1 * b
    lo, hi = 1e-4 * d, h
    for _ in range(60):
        mid = 0.5 * (lo + hi)
        if bal(mid) > 0: hi = mid
        else: lo = mid
    c = 0.5 * (lo + hi)
    a = min(b1 * c, h)
    epst = EPS_CU * (d - c) / c
    fs = min(fy, ES * epst)
    f1, f2 = tension(c, h, lcs, fr1, fr3)
    mn = As * fs * (d - a / 2) + b * ((c - a / 2) * f1 + f2)
    return c, epst, mn

rows = list(csv.DictReader(open(sys.argv[1])))
worst_c = worst_m = 0.0
bad = 0
for r in rows:
    v = {k: float(x) for k, x in r.items()}
    c, e, m = flex(v['b'], v['h'], v['d'], v['As'], v['fc'], v['fy'], v['FR1'], v['FR3'], v['lcs'])
    dc = abs(c / v['C'] - 1); dm = abs(m / v['Mn'] - 1)
    worst_c = max(worst_c, dc); worst_m = max(worst_m, dm)
    if dm > 0.01: bad += 1
print(f"cases {len(rows)}; max |dc| {worst_c*100:.4f} %; max |dMn| {worst_m*100:.4f} %; cases with |dMn|>1 %: {bad}")
