#!/bin/bash
# PCR-23/24/25 (2026-11-17): deterministic re-evaluation (--evalonly) of the 270 final vectors of the pre-registered Ex2 campaign 1
# (04_Results/ex2/campaign/, read-only). No optimisation. Three evaluations per vector:
#   old  = campaign-1 binary (ex2_campaign_bin, SHA-checked), campaign-1 rules
#   p23  = new binary, campaign-1 rules + PCR-23 only (--patterned 0 --beamaxial 0): effect of the SFRC flexure fix
#   new  = new binary, campaign-2 rules (PCR-23 + PCR-24 beam axial tension + PCR-25 ACI 6.4.2 patterns in the design envelope)
# Usage: bash ex2_reeval_pcr23_25.sh <new_bin_dir> <out_dir>
set -e
NEW="$(cd "$1" && pwd)"; mkdir -p "$2"; OUT="$(cd "$2" && pwd)"
HERE="$(cd "$(dirname "$0")" && pwd)"
OLD="$HERE/../ex2_campaign_bin"
CAMP="$HERE/../../04_Results/ex2/campaign"
mkdir -p "$OUT/x" "$OUT/old" "$OUT/p23" "$OUT/new"
( cd "$OLD" && sha256sum -c SHA256SUMS_ex2_campaign_2026-11-16.txt ) > "$OUT/sha_check_old.txt"
( cd "$NEW" && sha256sum Ex2Model.dll Ex2Runner.exe FemCore.dll FemRcModel.dll RcCore.dll ) > "$OUT/sha_new.txt"
: > "$OUT/jobs.txt"
for cell in plain fibre_fp90 fibre_fp0; do
  case $cell in plain) fib=0; fp=90;; fibre_fp90) fib=1; fp=90;; fibre_fp0) fib=1; fp=0;; esac
  for f in "$CAMP/$cell"/EX2_*.csv; do
    b=$(basename "$f" .csv); n="${cell}__$b"
    sed -n 4p "$f" > "$OUT/x/$n.txt"
    echo "$OLD/Ex2Runner.exe|--groups 9 --qcheck 1 --fibre $fib --fibreprice $fp --evalonly x/$n.txt|old/$n.txt" >> "$OUT/jobs.txt"
    echo "$NEW/Ex2Runner.exe|--groups 9 --qcheck 1 --fibre $fib --fibreprice $fp --patterned 0 --beamaxial 0 --evalonly x/$n.txt|p23/$n.txt" >> "$OUT/jobs.txt"
    echo "$NEW/Ex2Runner.exe|--groups 9 --qcheck 1 --fibre $fib --fibreprice $fp --patterned 1 --beamaxial 1 --evalonly x/$n.txt|new/$n.txt" >> "$OUT/jobs.txt"
  done
done
cd "$OUT"
cat jobs.txt | xargs -P 3 -I{} bash -c 'IFS="|" read -r exe args out <<< "{}"; "$exe" $args > "$out"'
echo "done $(date)" > done.txt
