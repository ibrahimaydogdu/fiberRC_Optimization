"""Ex2 V1 / V2-a: summary of the EtabsEx2 raw outputs (04_Results/ex2/v1_v2a_etabs/raw) -> summary_*.csv and stdout.
Usage: python -I summarize.py <rawDir> <outDir>
"""
import csv, math, os, sys

raw, out = sys.argv[1], sys.argv[2]
FC, FY = 30.0, 420.0
H = {'PILOT': 550.0, 'K1': 500.0}
lines = []


def P(s=''):
    print(s)
    lines.append(s)


def rd(name):
    return list(csv.DictReader(open(os.path.join(raw, name), encoding='utf-8')))


def wr(name, header, rows):
    with open(os.path.join(out, name), 'w', newline='', encoding='utf-8') as f:
        w = csv.writer(f)
        w.writerow(header)
        w.writerows(rows)


# 1 reactions
rows = []
for c in ('T1', 'PILOT', 'K1'):
    for r in rd(f'{c}_reactions.csv'):
        rows.append([c, r['case'], r['ETABS_Fz_kN'], r['FEM_kN'], r['diff_pct']])
wr('summary_V1_reactions.csv', ['design', 'case', 'ETABS_Fz_kN', 'FEM_kN', 'diff_pct'], rows)
P('V1(b) reactions (kN):')
for x in rows:
    P('  ' + ' | '.join(x))

# 2 forces
rows = []
for c in ('T1', 'PILOT', 'K1'):
    st = rd(f'{c}_station_forces.csv')
    for case in ('SW', 'G', 'Q', 'U1', 'U2'):
        for q in ('P', 'V2', 'M3'):
            for typ in ('column', 'beam'):
                sel = [r for r in st if r['case'] == case and r['type'] == typ]
                mref = max(abs(float(r[f'{q}_FEM_' + ('kN' if q != 'M3' else 'kNm')])) for r in sel)
                mdif = max(abs(float(r[f'{q}_ETABS_' + ('kN' if q != 'M3' else 'kNm')]) - float(r[f'{q}_FEM_' + ('kN' if q != 'M3' else 'kNm')])) for r in sel)
                rows.append([c, case, typ, q, f'{mref:.3f}', f'{mdif:.2e}', f'{100 * mdif / max(mref, 1e-9):.2e}'])
wr('summary_V2a_forces.csv', ['design', 'case', 'member', 'quantity', 'max_abs_FEM', 'max_abs_diff', 'diff_pct_of_max'], rows)
worst = max(rows, key=lambda r: float(r[6]))
P(f'V2-a forces: {len(rows)} (design, case, member type, quantity) cells; worst relative difference {worst[6]} % ({worst[:4]})')
# per-end relative (ends with |value| > 1 % of the case maximum)
mx_end = 0.0
for c in ('T1', 'PILOT', 'K1'):
    ee = rd(f'{c}_end_forces.csv')
    for r in ee:
        for q, u in (('P', 'kN'), ('V2', 'kN'), ('M3', 'kNm')):
            fem = float(r[f'{q}_FEM_{u}'])
            if abs(fem) > 1.0:
                mx_end = max(mx_end, abs(float(r[f'{q}_ETABS_{u}']) - fem) / abs(fem) * 100)
P(f'V2-a forces: largest end-force relative difference (|FEM| > 1 kN or kNm): {mx_end:.2e} %')

# 3 beams
vc_e = lambda b, d: 2.0 * math.sqrt(FC * 145.0377) / 145.0377 * b * d  # ETABS: 2 sqrt(f'c psi) bw d (US form), N
FYT_E = 60000 / 145.0377  # 60 ksi = 413.69 MPa
rows = []
for c in ('PILOT', 'K1'):
    for suf, lab in (('', 'PatLLF=0 (main)'), ('_patLLF075', 'PatLLF=0.75 (ETABS default)'), ('_sdcB', 'PatLLF=0, SDC B')):
        for r in rd(f'{c}_beam_groups{suf}.csv'):
            b, d = float(r['b_mm']), float(r['d_mm'])
            vu = float(r['prog_Vu_d_kN']) * 1e3
            avs_h = max((vu / 0.75 - vc_e(b, d)) / (FYT_E * d), 0.35 * b / FY)
            rows.append([c, lab, r['group'], r['b_mm'], r['h_mm'], r['prog_AsTopReq'], r['ETABS_TopReq_max'], r['B1_top'], r['prog_AsBotReq'], r['ETABS_BotReq_max'], r['B1_bot'],
                         r['prog_Av_s'], r['ETABS_Av_s_max_node'], r['B2_node'], r['ETABS_Av_s_at_d'], r['B2_at_d'], f'{avs_h:.5f}', f'{avs_h / float(r["ETABS_Av_s_at_d"]):.4f}',
                         r['stirrupGov'], r['prog_s_mm'], r['prog_DCflex'], r['prog_DCshear'], r['ETABSreq_over_progProv_top'], r['ETABSreq_over_progProv_bot'], r['ETABSd_over_progProv'], r['ETABSnode_over_progProv']])
hdr = ['design', 'ETABS_run', 'group', 'b_mm', 'h_mm', 'prog_AsTopReq_mm2', 'ETABS_TopReq_mm2', 'B1_top', 'prog_AsBotReq_mm2', 'ETABS_BotReq_mm2', 'B1_bot',
       'prog_Av_s_mm2pmm', 'ETABS_Av_s_node', 'B2_node', 'ETABS_Av_s_at_d', 'B2_at_d', 'prog_formula_with_ETABS_constants_Av_s', 'B2_at_d_harmonised',
       'stirrup_gov', 's_mm', 'prog_DC_flex', 'prog_DC_shear', 'ETABSreqTop_over_progProv', 'ETABSreqBot_over_progProv', 'ETABS_Av_s_d_over_prog_provided', 'ETABS_Av_s_node_over_prog_provided']
wr('summary_V2a_beams.csv', hdr, rows)
P('V2-a beams (B1 = program/ETABS required As; B2 = program/ETABS Av/s):')
for x in rows:
    P(f'  {x[0]} {x[1]:28s} {x[2]} B1top {x[7]} B1bot {x[10]} | B2 node {x[13]} at d {x[15]} harmonised {x[17]} | {x[18]} s={x[19]}')

# 3b shear constants fit and axial tension in flexure (main run)
P('ETABS beam shear fit (main run): Vu/phi = Vc + (Av/s) fyt d over the stations with Av/s > Av,min:')
for c in ('PILOT', 'K1'):
    stf = {}
    for r in rd(f'{c}_station_forces.csv'):
        if r['type'] == 'beam':
            stf[(r['element'], r['case'], round(float(r['x_m']), 3))] = (float(r['P_FEM_kN']), float(r['V2_FEM_kN']), float(r['M3_FEM_kNm']))
    X, Y, top = [], [], []
    for r in rd(f'{c}_beam_stations.csv'):
        e, x = r['element'], round(float(r['x_m']), 3)
        av = float(r['ETABS_Av_s_mm2pmm'])
        u1, u2 = stf[(e, 'U1', x)], stf[(e, 'U2', x)]
        if av > 0.2084:
            X.append(av)
            Y.append(max(abs(u1[1]), abs(u2[1])) * 1e3 / 0.75)
        tr = float(r['ETABS_TopReq_mm2'])
        if tr > 500:
            dd = H[c] - 60
            a = tr * FY / (0.85 * FC * 250)
            mu_e = 0.9 * tr * FY * (dd - a / 2) / 1e6
            gov = u2 if -u2[2] >= -u1[2] else u1
            n = gov[0]
            mu_n = -gov[2] + (max(n, 0.0) * (dd - H[c] / 2) / 1e3)
            top.append((e, x, -gov[2], n, mu_e, mu_n))
    n = len(X)
    mx, my = sum(X) / n, sum(Y) / n
    B = sum((a - mx) * (b - my) for a, b in zip(X, Y)) / sum((a - mx) ** 2 for a in X)
    A = my - B * mx
    res = max(abs(y - (A + B * x)) for x, y in zip(X, Y))
    dd = H[c] - 60
    P(f'  {c}: n={n}, intercept Vc = {A / 1e3:.3f} kN (program 0.17 sqrt(fc) bw d = {0.17 * math.sqrt(FC) * 250 * dd / 1e3:.3f}; 2 sqrt(fc psi) bw d = {vc_e(250, dd) / 1e3:.3f}), '
      f'slope/d = fyt {B / dd:.2f} MPa (60 ksi = {FYT_E:.2f}), max residual {res / 1e3:.4f} kN')
    worst = max(top, key=lambda t: abs(t[4] - t[5]))
    worst0 = max(top, key=lambda t: abs(t[4] - t[2]))
    P(f'  {c}: ETABS top-steel moment vs FEM Mu: largest |diff| without axial {abs(worst0[4] - worst0[2]):.2f} kNm ({worst0[0]} x={worst0[1]}, N={worst0[3]:.2f} kN tension+); '
      f'with Mu + N_t (d - h/2): largest |diff| {abs(worst[4] - worst[5]):.2f} kNm')

# 4 columns
rows = []
for c in ('PILOT', 'K1'):
    for run in ('A_nonsway', 'A_nonsway_vs_code', 'B_swayQ', 'C_nomag', 'D_etabsdefaults', 'S_sdcB'):
        fn = f'{c}_columns_{run}.csv'
        if not os.path.exists(os.path.join(raw, fn)):
            continue
        rr = rd(fn)
        pk = {'A_nonsway_vs_code': 'program_DC_code', 'B_swayQ': 'program_DC_code', 'C_nomag': 'program_DC_nomag'}.get(run, 'program_DC_ns')
        for r in rr:
            rows.append([c, run, r['element'], r['group'], r['storey'], r['storeyQ'], r['ETABS_PMM'], r['ETABS_combo'], r['ETABS_status'], r['ETABS_message'],
                         r[pk] if pk else '', r['ratio_prog_ETABS'], r['program_DC_code'], r['program_DC_ns'], r['program_DC_nomag'], r['program_maxDelta_inplane_code'], r['program_maxDelta_outofplane']])
        ok = [r for r in rr if not r['ETABS_status'].startswith('O/S') and float(r['ETABS_PMM']) > 0.02]
        os_ = [r for r in rr if r['ETABS_status'].startswith('O/S')]
        pv = lambda r: float(r[pk]) if pk else float('nan')
        cd1 = sum(1 for r in rr if (float(r['ETABS_PMM']) > 1 or r['ETABS_status'].startswith('O/S')) and pv(r) <= 1)
        ratios = [pv(r) / float(r['ETABS_PMM']) for r in ok]
        me = max(float(r['ETABS_PMM']) for r in ok)
        mp = max(pv(r) for r in rr)
        P(f'COLUMNS {c} {run:20s}: CD1={cd1} (O/S {len(os_)}: {",".join(r["element"] for r in os_)}), CD2 min {min(ratios):.4f} max {max(ratios):.4f} (n={len(ok)}), '
          f'CD3 max ETABS (non-O/S) {me:.4f} vs program {mp:.4f} -> unsafe-side {max(0.0, 100 * (me - mp) / me):.2f} %')
wr('summary_V2a_columns.csv', ['design', 'ETABS_run', 'element', 'group', 'storey', 'storeyQ', 'ETABS_PMM', 'ETABS_combo', 'ETABS_status', 'ETABS_message', 'program_DC_compared', 'ratio_prog_ETABS',
                               'program_DC_code', 'program_DC_ns', 'program_DC_nomag', 'program_maxDelta_inplane_code', 'program_maxDelta_outofplane'], rows)
open(os.path.join(out, 'summary_stdout.txt'), 'w', encoding='utf-8').write('\n'.join(lines) + '\n')
