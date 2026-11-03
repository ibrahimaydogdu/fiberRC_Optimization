#!/bin/bash
# Ablation Phase 1 (PREDESIGN_PROTOCOL.md section 7): 1000 FE total (pre-design FEs included), arms R, A, C, ABC, plain and fibre (fR x1.00), 3 methods x 30 runs, seeds 1001-1030.
# Frozen binaries: ablation_bin (SHA256SUMS in runs/). Resumable: existing run files are skipped.
cd "$(dirname "$0")/ablation_bin" || exit 1
OUT=../../04_Results/ablation
mkdir -p $OUT
for fibre in 0 1; do
  scn=$([ "$fibre" = 1 ] && echo fibre || echo plain)
  for arm in R A C ABC; do
    case $arm in
      R)   flags="" ;;
      A)   flags="--derive 1" ;;
      C)   flags="--derive 1 --predesign 1 --usebox 0" ;;
      ABC) flags="--derive 1 --predesign 1 --usebox 1" ;;
    esac
    mkdir -p $OUT/${scn}_$arm
    echo "=== $scn arm $arm: $flags ($(date))" >> $OUT/ablation_log.txt
    ./Ex1Runner.exe --method ALL --fibre $fibre --runs 30 --fes 1000 --fr 1.0 --out $OUT/${scn}_$arm --dop 8 $flags >> $OUT/ablation_log.txt 2>&1
  done
done
echo "ALL DONE ($(date))" >> $OUT/ablation_log.txt
