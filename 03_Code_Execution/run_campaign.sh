#!/bin/bash
# Campaign driver (frozen program: 03_Code_Execution/campaign_bin). Resumable: finished run files are skipped.
cd "$(dirname "$0")/campaign_bin"
OUT=../../04_Results/campaign
for cfg in "0 1.0" "1 1.0" "1 0.85" "1 0.70"; do
  set -- $cfg
  ./Ex1Runner.exe --method ALL --fibre $1 --runs 30 --fes 10000 --fr $2 --out $OUT --dop 8 >> $OUT/campaign_log.txt 2>&1
done
echo "ALL DONE" >> $OUT/campaign_log.txt
