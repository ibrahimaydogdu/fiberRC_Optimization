#!/bin/bash
# Example 2 (Ö2) campaign, pre-registered in EX2_SPEC.md "Kampanya ön kaydı" (2026-11-16/17). Frozen binary ex2_campaign_bin/ (SHA256SUMS_ex2_campaign_2026-11-16.txt = ex2_bin).
# Arm A only (random start, no predesign, no box), 9 groups, main shear format (Q17 delta), qcheck on, fR x1.0, 10 000 FE, seeds 1001-1030 (--seed0 1000).
# BTO/ATO all-dimension moves (runner sets OneDimMove=False; no --onedim flag exists in Ex2Runner); YAKO one-dimension by construction.
# Cells (one folder each, all three methods inside, method in file name):
#   plain        : --fibre 0                      3 x 30 = 90 runs
#   fibre_fp90   : --fibre 1 --fibreprice 90      3 x 30 = 90 runs
#   fibre_fp0    : --fibre 1 --fibreprice 0       3 x 30 = 90 runs   (total 270)
# Resumable: the runner skips run files that already exist; each cell is retried up to 4 passes until 90 files exist.
# Start: cd 03_Code_Execution; nohup bash ./run_ex2_campaign.sh > /dev/null 2>&1 &
cd "$(dirname "$0")/ex2_campaign_bin" || exit 1
OUT=../../04_Results/ex2/campaign
mkdir -p $OUT
LOG=$OUT/campaign_log.txt
echo "=== EX2 CAMPAIGN start $(date)" >> $LOG
sha256sum -c SHA256SUMS_ex2_campaign_2026-11-16.txt >> $LOG 2>&1 || { echo "SHA CHECK FAILED, abort $(date)" >> $LOG; exit 1; }
cell() {   # name fibre price
  local name=$1 fib=$2 price=$3
  local dir=$OUT/$name; mkdir -p $dir
  for pass in 1 2 3 4; do
    local n=$(ls $dir 2>/dev/null | grep -c "^EX2_.*csv$")
    [ "$n" -ge 90 ] && break
    echo "--- cell $name pass $pass ($n/90) $(date)" >> $LOG
    ./Ex2Runner.exe --method ALL --fibre $fib --fibreprice $price --runs 30 --fes 10000 --fr 1.0 --seed0 1000 --groups 9 --shearformat delta --qcheck 1 --dop 8 --out $dir >> $LOG 2>&1
  done
  echo "--- cell $name done ($(ls $dir 2>/dev/null | grep -c "^EX2_.*csv$")/90) $(date)" >> $LOG
}
cell plain 0 90
cell fibre_fp90 1 90
cell fibre_fp0 1 0
echo "ALL DONE ex2 campaign $(date)" >> $LOG
