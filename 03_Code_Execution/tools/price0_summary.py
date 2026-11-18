"""Ex2 fibre price = 0 sanity check summary (outside the campaign; seeds 9101-9105). Usage: python -I price0_summary.py <runDir>
Reads EX2_*_run??.csv written by ex2_bin/Ex2Runner.exe and writes <runDir>/price0_summary.csv (one row per run) + prints group medians.
Fibre dose > 0 per beam group from the design vector: 24 variables = 6 column groups x (b, h) + 3 beam groups x (b, h, type, dose index); dose index 0 = no fibre."""
import csv, glob, os, statistics, sys

d = sys.argv[1]
rows = []
for p in sorted(glob.glob(os.path.join(d, 'EX2_*_run??.csv'))):
    info = {}
    with open(p, encoding='utf-8') as f:
        for line in f:
            t = line.rstrip('\n').split(',')
            if t[0] == 'method':
                kv = dict(zip(t[0::2], t[1::2])); info.update({k: kv[k] for k in ('method', 'run', 'seed', 'FEs', 'fibre', 'fibreprice', 'wallSeconds')})
            elif t[0] == 'Fpen':
                kv = dict(zip(t[0::2], t[1::2])); info.update({k: kv[k] for k in ('Fpen', 'cost', 'T', 'fibreCost', 'fibreKg', 'steelKg', 'concreteM3')})
            elif t[0] == 'x':
                info['x'] = ';'.join(t[1:])
    x = [int(v) for v in info['x'].split(';')]
    if info['fibre'] == '1':
        doses = [x[12 + 4 * g + 3] for g in range(3)]
        types = [x[12 + 4 * g + 2] for g in range(3)]
        info['doseIdx_BS1_BS2_BS3'] = '/'.join(map(str, doses))
        info['typeIdx_BS1_BS2_BS3'] = '/'.join(map(str, types))
        info['fibreDosePositive'] = any(v > 0 for v in doses)
        info['nGroupsWithFibre'] = sum(v > 0 for v in doses)
    else:
        info['doseIdx_BS1_BS2_BS3'] = info['typeIdx_BS1_BS2_BS3'] = 'n/a'
        info['fibreDosePositive'] = 'n/a'; info['nGroupsWithFibre'] = 'n/a'
    info['scenario'] = ('fibre_fp' + info['fibreprice']) if info['fibre'] == '1' else 'plain'
    info['file'] = os.path.basename(p)
    rows.append(info)

cols = ['scenario', 'fibre', 'method', 'run', 'seed', 'FEs', 'fibreprice', 'Fpen', 'cost', 'T', 'fibreKg', 'fibreCost', 'steelKg', 'concreteM3', 'fibreDosePositive', 'nGroupsWithFibre', 'doseIdx_BS1_BS2_BS3', 'typeIdx_BS1_BS2_BS3', 'wallSeconds', 'x', 'file']
with open(os.path.join(d, 'price0_summary.csv'), 'w', newline='', encoding='utf-8') as f:
    w = csv.DictWriter(f, fieldnames=cols); w.writeheader(); w.writerows(rows)
print('scenario,method,n,median_cost,min_cost,max_cost,feasible(T=0),runs_with_fibre_dose>0,median_fibreKg')
for sc in ('fibre_fp0', 'fibre_fp90', 'plain'):
    for m in ('BTO', 'ATO'):
        rs = [r for r in rows if r['scenario'] == sc and r['method'] == m]
        if not rs: continue
        c = [float(r['cost']) for r in rs]
        fib = sum(1 for r in rs if r['fibreDosePositive'] is True)
        print(f"{sc},{m},{len(rs)},{statistics.median(c):.2f},{min(c):.2f},{max(c):.2f},{sum(float(r['T']) == 0 for r in rs)}/{len(rs)},"
              f"{fib if sc != 'plain' else 'n/a'},{statistics.median(float(r['fibreKg']) for r in rs):.2f}")
