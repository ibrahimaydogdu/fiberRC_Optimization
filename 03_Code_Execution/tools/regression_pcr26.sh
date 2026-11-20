#!/bin/bash
# PCR-26/26b regression record (referee K8, 2026-11-19): with --deflcheck 0 --deflpattern 0 the campaign-3 and campaign-4 binaries must reproduce the campaign-2
# binary (cost, T, x, history) bit for bit. 3 seeds x 3 methods x {plain, fibre fp0, fibre fp90}, 500 FE each. No optimisation campaign; outputs only here.
# Usage: bash regression_pcr26.sh <work_dir> ; compares with cmp on lines 2-end of each run file (line 1 = version header, differs by SHA).
set -e
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"; WORK="$1"; OUT="$ROOT/04_Results/ex2/regression_pcr26"
mkdir -p "$WORK" "$OUT"; : > "$OUT/regression_pcr26.txt"
for b in 2 3 4; do rm -rf "$WORK/bin$b"; cp -r "$ROOT/03_Code_Execution/ex2_campaign${b}_bin" "$WORK/bin$b"; done
for b in 2 3 4; do for cell in "plain 0 90" "fp0 1 0" "fp90 1 90"; do set -- $cell
  d="$WORK/out_c${b}_$1"; rm -rf "$d"
  ( cd "$WORK/bin$b" && ./Ex2Runner.exe --method ALL --fibre $2 --fibreprice $3 --runs 3 --fes 500 --fr 1.0 --seed0 1000 --groups 9 --shearformat delta --qcheck 1 --patterned 1 --beamaxial 1 --flexcheck 1 --deflcheck 0 --deflpattern 0 --dop 4 --out "$d" > /dev/null 2>&1 )
done; done
n=0; bad=0
for cell in plain fp0 fp90; do for f in "$WORK/out_c2_$cell"/EX2_*.csv; do base=$(basename "$f")
  for b in 3 4; do n=$((n+1))
    # drop the two header lines that carry the version/SHA and wall time (line 1 '# ...' and line 2 meta with wallSeconds/deflcheck flags)
    if cmp -s <(tail -n +3 "$f") <(tail -n +3 "$WORK/out_c${b}_$cell/$base"); then echo "OK   c2==c$b $cell $base" >> "$OUT/regression_pcr26.txt"; else echo "DIFF c2!=c$b $cell $base" >> "$OUT/regression_pcr26.txt"; bad=$((bad+1)); fi
  done; done; done
echo "compared $n file pairs, differences $bad" | tee -a "$OUT/regression_pcr26.txt"
for b in 2 3 4; do ( cd "$WORK/bin$b" && sha256sum Ex2Model.dll ) | sed "s/^/c$b /" >> "$OUT/regression_pcr26.txt"; done
