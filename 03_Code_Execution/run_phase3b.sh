#!/bin/bash
# Phase 3b (IA decision 2026-11-13, after internal Critic + external referee): ATO all-dimension control, arm A, plain and fibre, 30 runs, 10 000 FE (no --onedim). Supplementary, same frozen binary phase3_bin.
# Waits for run_phase3.sh (BTO all-dim stage) to finish first so the two never share the CPU. Resumable (finished run files are skipped).
cd "$(dirname "$0")/phase3_bin" || exit 1
OUT=../../04_Results/phase3
LOG=$OUT/phase3_log.txt
while ps -ef | grep -v grep | grep -q "run_phase3.sh"; do sleep 60; done
echo "=== STAGE 3b ATO all-dimension control (supplementary) $(date)" >> $LOG
for scn in 0 1; do
  name=$([ "$scn" = 1 ] && echo fibre || echo plain)
  dir=$OUT/ato_alldim/${name}_A; mkdir -p $dir
  for pass in 1 2 3 4; do
    n=$(ls $dir 2>/dev/null | grep -c "^ATO_${name}_.*csv$")
    [ "$n" -ge 30 ] && break
    echo "--- cell $dir ATO pass $pass ($n/30) $(date)" >> $LOG
    ./Ex1Runner.exe --method ATO --fibre $scn --runs 30 --fes 10000 --fr 1.0 --out $dir --dop 8 --derive 1 >> $LOG 2>&1
  done
done
echo "ALL DONE 3b $(date)" >> $LOG
