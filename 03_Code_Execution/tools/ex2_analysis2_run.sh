#!/bin/bash
# Example 2 (Ö2) CAMPAIGN 2 post-campaign deterministic tools (EX2_SPEC §11 fibre-output hierarchy, §11.1 rules 2-4, §11.2 post-hoc PCR-23/24/25).
# Same tool set as ex2_analysis_run.sh (campaign 1), new binary (ex2_campaign2_bin, Ex2Model a61b0c6b...), new designs. No optimisation.
# Campaign-2 rules are the runner defaults (--patterned 1 --beamaxial 1 --flexcheck 1); they are passed explicitly below.
# Run from a COPY of ex2_campaign2_bin (SHA-checked); campaign outputs are read-only.
# Usage: bash ex2_analysis2_run.sh <copied_bin_dir> <analysis_out_dir>
# Designs (lowest-cost run files, 04_Results/ex2/campaign2/; ex2_stats_campaign2.md §4):
#   P   = plain/EX2_ATO_plain_g9_fr1.00_fp90_run02.csv           (plain lowest 236 456.22 TL; ATO 20/30, BTO 15/30 same vector)
#   PY  = plain/EX2_YAKO_plain_g9_fr1.00_fp90_run03.csv          (YAKO lowest plain 236 711.62 TL; marginal table per method)
#   F90 = fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv      (fibre 90 TL/kg lowest; dose 0 in all groups)
#   F0  = fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv        (fibre 0 TL/kg lowest 233 916.61 TL)
set -e
BIN="$1"; OUT="$2"
CAMP="$(cd "$(dirname "$0")/../../04_Results/ex2/campaign2" && pwd)"
mkdir -p "$OUT/x"
( cd "$BIN" && sha256sum -c SHA256SUMS_ex2_campaign2_2026-11-17.txt ) > "$OUT/sha_check.txt"
sed -n 4p "$CAMP/plain/EX2_ATO_plain_g9_fr1.00_fp90_run02.csv" > "$OUT/x/P.txt"
sed -n 4p "$CAMP/plain/EX2_YAKO_plain_g9_fr1.00_fp90_run03.csv" > "$OUT/x/PY.txt"
sed -n 4p "$CAMP/fibre_fp90/EX2_ATO_fibre_g9_fr1.00_fp90_run01.csv" > "$OUT/x/F90.txt"
sed -n 4p "$CAMP/fibre_fp0/EX2_ATO_fibre_g9_fr1.00_fp0_run01.csv" > "$OUT/x/F0.txt"
R="$BIN/Ex2Runner.exe"
COMMON="--groups 9 --qcheck 1 --patterned 1 --beamaxial 1 --flexcheck 1"
run() { local name=$1; shift; "$R" $COMMON "$@" > "$OUT/$name.txt"; echo "$name: $*" >> "$OUT/commands.txt"; }
: > "$OUT/commands.txt"
X="$OUT/x"
# --- plain lowest (P)
run P_evalonly     --fibre 0 --evalonly "$X/P.txt"
run P_scan         --fibre 0 --scan "$X/P.txt"
run P_patterns     --fibre 0 --patterns "$X/P.txt"
run P_deflection   --fibre 0 --deflection "$X/P.txt"
run P_marginal_main      --fibre 0 --fibreprice 90 --shearformat delta  --fr 1.0 --marginal "$X/P.txt"
run P_marginal_mc2010    --fibre 0 --fibreprice 90 --shearformat mc2010 --fr 1.0 --marginal "$X/P.txt"
run P_marginal_o1        --fibre 0 --fibreprice 90 --shearformat o1     --fr 1.0 --marginal "$X/P.txt"
run P_marginal_fr0.8     --fibre 0 --fibreprice 90 --shearformat delta  --fr 0.8 --marginal "$X/P.txt"
run P_marginal_fr0.6     --fibre 0 --fibreprice 90 --shearformat delta  --fr 0.6 --marginal "$X/P.txt"
run P_marginal_fr1.2     --fibre 0 --fibreprice 90 --shearformat delta  --fr 1.2 --marginal "$X/P.txt"
run PY_evalonly          --fibre 0 --evalonly "$X/PY.txt"
run PY_marginal_main     --fibre 0 --fibreprice 90 --shearformat delta  --fr 1.0 --marginal "$X/PY.txt"
# --- fibre 90 TL/kg lowest (F90)
run F90_evalonly   --fibre 1 --fibreprice 90 --evalonly "$X/F90.txt"
run F90_scan       --fibre 1 --fibreprice 90 --scan "$X/F90.txt"
run F90_patterns   --fibre 1 --fibreprice 90 --patterns "$X/F90.txt"
run F90_deflection --fibre 1 --fibreprice 90 --deflection "$X/F90.txt"
run F90_zerofibre  --fibre 1 --fibreprice 90 --zerofibre "$X/F90.txt"
# --- fibre 0 TL/kg lowest (F0)
run F0_evalonly    --fibre 1 --fibreprice 0 --evalonly "$X/F0.txt"
run F0_scan        --fibre 1 --fibreprice 0 --scan "$X/F0.txt"
run F0_patterns    --fibre 1 --fibreprice 0 --patterns "$X/F0.txt"
run F0_deflection  --fibre 1 --fibreprice 0 --deflection "$X/F0.txt"
run F0_zerofibre   --fibre 1 --fibreprice 0 --zerofibre "$X/F0.txt"
# F0Z = F0 sections with every beam dose index set to 0 (item breakdown of the zerofibre "ALL" row)
awk -F, 'BEGIN{OFS=","} {$17=0; $21=0; $25=0; print}' "$X/F0.txt" > "$X/F0Z.txt"
run F0Z_evalonly   --fibre 1 --fibreprice 0 --evalonly "$X/F0Z.txt"
run F0Z_deflection --fibre 1 --fibreprice 0 --deflection "$X/F0Z.txt"
# sensitivity of F0 (deterministic re-evaluation, sections fixed, steel/stirrups re-derived)
for v in "mc2010 1.0" "o1 1.0" "delta 0.8" "delta 0.6" "delta 1.2" "mc2010 0.8" "mc2010 1.2"; do
  set -- $v
  run "F0_evalonly_${1}_fr$2"  --fibre 1 --fibreprice 0 --shearformat $1 --fr $2 --evalonly "$X/F0.txt"
  run "F0_zerofibre_${1}_fr$2" --fibre 1 --fibreprice 0 --shearformat $1 --fr $2 --zerofibre "$X/F0.txt"
done
for p in 45 90 135 180; do
  run "F0_evalonly_fp$p" --fibre 1 --fibreprice $p --evalonly "$X/F0.txt"
done
echo "done $(date)" >> "$OUT/commands.txt"
