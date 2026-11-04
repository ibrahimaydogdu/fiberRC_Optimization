#!/bin/bash
# Phase 2 (PREDESIGN_PROTOCOL.md section 8): 10 000 FE total (pre-design FEs included), arms ABC, A, C, plain and fibre (fR x1.00), 3 methods x 30 runs, seeds 1001-1030.
# BTO/ATO change one random dimension per move (--onedim 1, PCR-12; YAKO already does). Frozen binaries: phase2_bin (SHA256SUMS_phase2_2026-11-04.txt). Resumable.
cd "$(dirname "$0")/phase2_bin" || exit 1
OUT=../../04_Results/phase2
mkdir -p $OUT
for arm in ABC A C; do
  for fibre in 0 1; do
    scn=$([ "$fibre" = 1 ] && echo fibre || echo plain)
    case $arm in
      A)   flags="--derive 1" ;;
      C)   flags="--derive 1 --predesign 1 --usebox 0" ;;
      ABC) flags="--derive 1 --predesign 1 --usebox 1" ;;
    esac
    mkdir -p $OUT/${scn}_$arm
    echo "=== $scn arm $arm: $flags --onedim 1 ($(date))" >> $OUT/phase2_log.txt
    ./Ex1Runner.exe --method ALL --fibre $fibre --runs 30 --fes 10000 --fr 1.0 --out $OUT/${scn}_$arm --dop 8 --onedim 1 $flags >> $OUT/phase2_log.txt 2>&1
  done
done
echo "ALL DONE ($(date))" >> $OUT/phase2_log.txt
