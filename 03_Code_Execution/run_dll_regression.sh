#!/bin/bash
# DLL regression (referee request, 2026-11-15): phase3_bin (FemRcModel.dll c1a95951...) with --onedim 1 must reproduce the Phase 2 CSVs of phase2_bin (a2b418c2...)
# for plain arm A (flags "--derive 1 --onedim 1"), BTO and ATO, seeds 1001-1003, 10 000 FE, --dop 1 (6 independent single-thread processes).
cd "$(dirname "$0")/phase3_bin" || exit 1
OUT=../../04_Results/regression_dll
for m in BTO ATO; do for k in 0 1 2; do
  mkdir -p $OUT/${m}_s$((1001+k))
  ./Ex1Runner.exe --method $m --fibre 0 --runs 1 --fes 10000 --fr 1.0 --out $OUT/${m}_s$((1001+k)) --dop 1 --seed0 $((1000+k)) --derive 1 --onedim 1 > $OUT/${m}_s$((1001+k)).log 2>&1 &
done; done
wait
echo "REGRESSION RUNS DONE $(date)" > $OUT/done.txt
