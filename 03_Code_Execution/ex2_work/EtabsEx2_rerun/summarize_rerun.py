"""Summary of the Ex2 V2-a ETABS re-validation (EtabsEx2Rerun raw output). Usage: python -I summarize_rerun.py <rawDir> <outDir>
Reads *_columns_*.csv and *_beam_groups.csv; writes summary_V2a_rerun_columns.csv, summary_V2a_rerun_beams.csv and prints the gate table."""
import csv, os, sys

raw, out = sys.argv[1], sys.argv[2]


def rows(path):
    with open(path, newline='', encoding='utf-8') as f:
        r = csv.reader(f)
        hdr = next(r)
        seen = {}
        names = []
        for h in hdr:  # duplicate headers -> suffix
            k = h if h not in seen else h + '#2'
            seen[h] = 1
            names.append(k)
        return [dict(zip(names, x)) for x in r]


fl = lambda s: float(s) if s not in ('', 'NaN', 'nan') else float('nan')
colrows = []
print('case,run,n_columns,ETABS_OS,CD1(ETABS>1 or O/S & program OK),CD1_formula_only,CD2_min,CD2_max,CD3_ETABSmax,CD3_progmax,unsafe_pct,'
      '626_agree,prog626_max,ETABS626_implied_max,lu_ratio_check_maxabs_mm')
for case in ('PILOT2', 'K1'):
    for run in ('A_nonsway_vs_code', 'A_nonsway_vs_noex', 'B_swayQ_vs_code', 'C_nomag'):
        p = os.path.join(raw, f'{case}_columns_{run}.csv')
        rs = rows(p)
        mode = run.split('_vs_')[-1] if '_vs_' in run else 'nomag'
        nOS = cd1 = cd1f = agree = 0
        rat, eMax, pMax, p626, e626, luerr = [], 0.0, 0.0, 0.0, 0.0, 0.0
        for r in rs:
            os_ = r['ETABS_status'].startswith('O/S')
            pv = fl(r['program_DC_' + mode])
            ok = r['program_ok_code(DC<=1 and 626<=1.4)'] == 'True'
            e = fl(r['ETABS_PMM'])
            nOS += os_
            if (e > 1 or os_) and ok: cd1 += 1
            if (e > 1 or os_) and pv <= 1: cd1f += 1
            if os_ == (fl(r['prog626_code']) > 1.4): agree += 1
            if not os_ and e > 0.02:
                rat.append(pv / e); eMax = max(eMax, e)
            pMax = max(pMax, pv)
            p626 = max(p626, fl(r['prog626_code']))
            e626 = max(e626, fl(r['ETABS626_in_implied']), fl(r['ETABS626_out_implied']))
            luerr = max(luerr, abs(fl(r['lu_ETABS_major_mm']) - fl(r['lu_prog_mm'])))
            colrows.append({'case': case, 'run': run, 'element': r['element'], 'group': r['group'], 'storey': r['storey'], 'sway_prog': r['sway_prog'],
                            'b_mm': r['b_mm'], 'h_mm': r['h_mm'], 'lu_prog_mm': r['lu_prog_mm'], 'lu_ETABS_mm': r['lu_ETABS_major_mm'],
                            'ETABS_PMM': r['ETABS_PMM'], 'ETABS_status': r['ETABS_status'], 'ETABS_message': r['ETABS_message'],
                            'program_DC_mode': r['program_DC_' + mode], 'ratio_prog_ETABS': r['ratio_prog_ETABS'],
                            'program_DC_noex': r['program_DC_noex'], 'ratio_noex_ETABS': r['ratio_noex_ETABS'],
                            'program_ok_code': r['program_ok_code(DC<=1 and 626<=1.4)'], 'prog626_code': r['prog626_code'], 'prog626_noex': r['prog626_noex'],
                            'ETABS626_in_implied': r['ETABS626_in_implied'], 'ETABS626_out_implied': r['ETABS626_out_implied'],
                            'prog_delta3_code': r['prog_delta3_code'], 'prog_delta2_code': r['prog_delta2_code'],
                            'prog_delta3_noex': r['prog_delta3_noex'], 'prog_delta2_noex': r['prog_delta2_noex']})
        unsafe = max(0.0, 100 * (eMax - pMax) / eMax) if eMax > 0 else float('nan')
        print(f'{case},{run},{len(rs)},{nOS},{cd1},{cd1f},{min(rat):.4f},{max(rat):.4f},{eMax:.4f},{pMax:.4f},{unsafe:.2f},{agree}/{len(rs)},{p626:.4f},{e626:.4f},{luerr:.2f}')
with open(os.path.join(out, 'summary_V2a_rerun_columns.csv'), 'w', newline='', encoding='utf-8') as f:
    w = csv.DictWriter(f, fieldnames=list(colrows[0].keys()))
    w.writeheader(); w.writerows(colrows)

print()
print('case,group,B1_top_raw,B1_bot_raw,B1_top_axialadj,B1_bot_axialadj,N_t_max_kN,B2_at_d_raw,B2_at_d_ETABSconst,B2_node_raw,stirrupGov,s_mm')
brows = []
for case in ('PILOT2', 'K1'):
    for r in rows(os.path.join(raw, f'{case}_beam_groups.csv')):
        d = {'case': case, 'group': r['group'], 'b_mm': r['b_mm'], 'h_mm': r['h_mm'], 'B1_top_raw': r['B1_top'], 'B1_bot_raw': r['B1_bot'],
             'B1_top_axialadj': r['B1_top_axialadj'], 'B1_bot_axialadj': r['B1_bot_axialadj'], 'N_tension_max_kN': r['N_tension_max_kN'],
             'prog_Av_s': r['prog_Av_s'], 'ETABS_Av_s_at_d': r['ETABS_Av_s_at_d'], 'B2_at_d_raw': r['B2_at_d'],
             'progETABSconst_Av_s_at_d': r['progETABSconst_Av_s_at_d'], 'B2_at_d_ETABSconst': r['B2_at_d_ETABSconst'],
             'ETABS_Av_s_max_node': r['ETABS_Av_s_max_node'], 'B2_node_raw': r['B2_node'], 'stirrupGov': r['stirrupGov'], 's_mm': r['prog_s_mm'],
             'prog_AvS_strength': r['prog_AvS_strength'], 'prog_AvS_min': r['prog_AvS_min']}
        brows.append(d)
        print(','.join([case, d['group'], d['B1_top_raw'], d['B1_bot_raw'], d['B1_top_axialadj'], d['B1_bot_axialadj'], d['N_tension_max_kN'],
                        d['B2_at_d_raw'], d['B2_at_d_ETABSconst'], d['B2_node_raw'], d['stirrupGov'], d['s_mm']]))
with open(os.path.join(out, 'summary_V2a_rerun_beams.csv'), 'w', newline='', encoding='utf-8') as f:
    w = csv.DictWriter(f, fieldnames=list(brows[0].keys()))
    w.writeheader(); w.writerows(brows)
