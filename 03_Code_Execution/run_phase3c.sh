#!/bin/bash
# Phase 3c (IA decision 2026-11-13, after internal Critic + external referee): BTO and ATO with ALL-dimension moves (no --onedim) in the remaining cells, so that
# BTO/ATO all-dimension results exist for every arm/scenario. Together with Phase 3 stage 3 (BTO A, C) and 3b (ATO A) this gives a full all-dimension set:
#   BTO: A, C (stage 3), ABC (here); ATO: A (3b), C and ABC (here). Plain and fibre, 30 runs (seeds 1001-1030), 10 000 FE total.
# Fibre cells use the fibre-capacity x0 (--fibrex0 1, IA decision 2026-11-09; PCR-13). NOTE: the stage-3 BTO fibre C cell ran without --fibrex0 (x0 identical, 363 FE difference).
# Starts only after run_phase3b.sh wrote "ALL DONE 3b" to the log. Resumable. Same frozen binary phase3_bin.
cd "$(dirname "$0")/phase3_bin" || exit 1
OUT=../../04_Results/phase3
LOG=$OUT/phase3_log.txt
while ! grep -q "ALL DONE 3b" $LOG; do sleep 120; done
echo "=== STAGE 3c all-dimension BTO ABC, ATO C and ABC $(date)" >> $LOG
cell() {   # method arm scn(0/1)
  local meth=$1 arm=$2 scn=$3
  local name=$([ "$scn" = 1 ] && echo fibre || echo plain)
  local fl="--derive 1 --predesign 1"
  [ $arm = C ] && fl="$fl --usebox 0"
  [ $arm = ABC ] && fl="$fl --usebox 1"
  [ "$scn" = 1 ] && fl="$fl --fibrex0 1"
  local sub=$([ $meth = BTO ] && echo bto_alldim || echo ato_alldim)
  local dir=$OUT/$sub/${name}_$arm; mkdir -p $dir
  for pass in 1 2 3 4; do
    local n=$(ls $dir 2>/dev/null | grep -c "^${meth}_${name}_.*csv$")
    [ "$n" -ge 30 ] && break
    echo "--- cell $dir $meth pass $pass ($n/30) $(date)" >> $LOG
    ./Ex1Runner.exe --method $meth --fibre $scn --runs 30 --fes 10000 --fr 1.0 --out $dir --dop 8 $fl >> $LOG 2>&1
  done
}
for scn in 0 1; do cell ATO C $scn; cell ATO ABC $scn; cell BTO ABC $scn; done
echo "ALL DONE 3c $(date)" >> $LOG
