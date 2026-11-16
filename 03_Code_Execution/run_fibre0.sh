#!/bin/bash
# Fibre price = 0 experiment (IA decision 2026-11-16, after internal Critic + external referee): BTO all-dimension, arm A, fibre scenario, fibre price 0 TL/kg, 10 runs (seeds 1001-1010), 10 000 FE, phase3_bin.
# Pre-registered rule (referee): a price sweep is opened only if the fibre median differs from the plain A BTO all-dim median (04_Results/phase3/bto_alldim/plain_A, price 90 TL/kg plain) by >= 1 % in cost or >= 5/10 runs have fibreKg > 0. IA: otherwise no price sweep.
# Starts after the DLL regression finished (04_Results/regression_dll/done.txt). Resumable.
cd "$(dirname "$0")/phase3_bin" || exit 1
OUT=../../04_Results/phase3
while [ ! -f ../../04_Results/regression_dll/done.txt ]; do sleep 120; done
echo "=== FIBRE PRICE 0 experiment $(date)" >> $OUT/phase3_log.txt
dir=$OUT/fibre_price0/fibre_A; mkdir -p $dir
for pass in 1 2 3 4; do
  n=$(ls $dir 2>/dev/null | grep -c "^BTO_fibre_.*csv$")
  [ "$n" -ge 10 ] && break
  echo "--- cell $dir BTO price0 pass $pass ($n/10) $(date)" >> $OUT/phase3_log.txt
  ./Ex1Runner.exe --method BTO --fibre 1 --fibreprice 0 --runs 10 --fes 10000 --fr 1.0 --out $dir --dop 8 --derive 1 >> $OUT/phase3_log.txt 2>&1
done
echo "ALL DONE fibre0 $(date)" >> $OUT/phase3_log.txt
