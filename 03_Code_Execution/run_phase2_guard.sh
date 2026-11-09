#!/bin/bash
# Waits for run_phase2.sh to end, then re-runs it (it skips finished runs) until all 540 CSVs exist (max 8 passes). Runner exited silently twice mid-arm on 2026-11-04/05.
cd "$(dirname "$0")"
OUT=../04_Results/phase2
count() { find $OUT -name "*_fr1.00_run*.csv" | wc -l; }
while ps -ef | grep -v grep | grep -q "run_phase2.sh"; do sleep 60; done
for pass in 1 2 3 4 5 6 7 8; do
  n=$(count); echo "guard pass $pass: $n/540 ($(date))" >> $OUT/phase2_log.txt
  [ "$n" -ge 540 ] && break
  bash ./run_phase2.sh >> $OUT/launcher.err 2>&1
done
echo "GUARD DONE $(count)/540 ($(date))" >> $OUT/phase2_log.txt
