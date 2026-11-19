#!/bin/bash
# Example 2 (Ö2) campaign 4 = PCR-26b re-run (IA decision 2026-11-19, "Ie env"): campaign-2 rules + g5 l/480 under ACI 6.4.2 live-load patterns with envelope Ie (--deflcheck 1 --deflpattern 1). Campaigns 1-2 (04_Results/ex2/campaign*/,
# ex2_campaign_bin) is kept unchanged as the superseded series; both series are reported together.
# Frozen binary ex2_campaign4_bin/ (SHA256SUMS_ex2_campaign4_2026-11-19.txt). Rules: campaign 2 + PCR-26b g5 (Deflection2D.PatternedEnvelope).
# PCR-24 (beam axial tension: flexure ACI 22.2, shear ACI 22.5.7.1), PCR-25 (ACI 6.4.2 live-load patterns in the design envelope of beams and columns).
# Everything else as the pre-registered campaign 1 (§11.1): arm A only, 9 groups, Q17 shear format, qcheck on, fR x1.0, 10 000 FE, seeds 1001-1030
# (--seed0 1000), BTO/ATO all-dimension moves, YAKO one-dimension, --dop 8.
# Cells: plain (90), fibre_fp90 (90), fibre_fp0 (90) = 270 runs (PCR-24/25 change the plain path too -> all three cells re-run).
# Resumable: the runner skips existing run files; each cell is retried up to 4 passes until 90 files exist.
# Start: cd 03_Code_Execution; nohup bash ./run_ex2_campaign4.sh > /dev/null 2>&1 &
cd "$(dirname "$0")/ex2_campaign4_bin" || exit 1
OUT=../../04_Results/ex2/campaign4
mkdir -p $OUT
LOG=$OUT/campaign4_log.txt
echo "=== EX2 CAMPAIGN4 start $(date)" >> $LOG
sha256sum -c SHA256SUMS_ex2_campaign4_2026-11-19.txt >> $LOG 2>&1 || { echo "SHA CHECK FAILED, abort $(date)" >> $LOG; exit 1; }
cell() {   # name fibre price
  local name=$1 fib=$2 price=$3
  local dir=$OUT/$name; mkdir -p $dir
  for pass in 1 2 3 4; do
    local n=$(ls $dir 2>/dev/null | grep -c "^EX2_.*csv$")
    [ "$n" -ge 90 ] && break
    echo "--- cell $name pass $pass ($n/90) $(date)" >> $LOG
    ./Ex2Runner.exe --method ALL --fibre $fib --fibreprice $price --runs 30 --fes 10000 --fr 1.0 --seed0 1000 --groups 9 --shearformat delta --qcheck 1 --patterned 1 --beamaxial 1 --flexcheck 1 --deflcheck 1 --deflpattern 1 --dop 8 --out $dir >> $LOG 2>&1
  done
  echo "--- cell $name done ($(ls $dir 2>/dev/null | grep -c "^EX2_.*csv$")/90) $(date)" >> $LOG
}
cell plain 0 90
cell fibre_fp90 1 90
cell fibre_fp0 1 0
echo "ALL DONE ex2 campaign4 $(date)" >> $LOG
