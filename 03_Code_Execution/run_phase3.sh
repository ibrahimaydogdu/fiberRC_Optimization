#!/bin/bash
# Phase 3 pipeline (PREDESIGN_PROTOCOL.md section 9; IA decision 2026-11-09), unattended, resumable (finished run files are skipped; each cell is retried up to 4 passes because the runner exited silently mid-cell before).
#  Stage 1: 20 000 FE, ATO + YAKO, arms A and ABC, plain and fibre, 10 runs (seeds 1001-1010). Fibre ABC uses the fibre-capacity x0 (PCR-13, --fibrex0 1); plain uses the plain x0.
#  Stage 2: maxfe_decision.py (pre-registered rule); unsettled cells are re-run with 30 000 FE (upper limit).
#  Stage 3: LAST, supplementary only: BTO all-dimension moves (no --onedim) vs Phase 2 one-dimension BTO, arms A and C, plain and fibre, 30 runs, 10 000 FE.
# Frozen binaries: phase3_bin (SHA256SUMS_phase3_*.txt). Log: ../04_Results/phase3/phase3_log.txt
cd "$(dirname "$0")/phase3_bin" || exit 1
OUT=../../04_Results/phase3
mkdir -p $OUT
LOG=$OUT/phase3_log.txt
flags_for() {   # arm scenario(0/1)
  case $1 in
    A)   echo "--derive 1 --onedim 1" ;;
    ABC) if [ "$2" = 1 ]; then echo "--derive 1 --predesign 1 --usebox 1 --onedim 1 --fibrex0 1"; else echo "--derive 1 --predesign 1 --usebox 1 --onedim 1"; fi ;;
  esac
}
runcell() {     # dir method fibre fes runs flags
  local dir=$1 meth=$2 fibre=$3 fes=$4 runs=$5; shift 5
  local scn=$([ "$fibre" = 1 ] && echo fibre || echo plain)
  mkdir -p $dir
  for pass in 1 2 3 4; do
    local n=$(ls $dir 2>/dev/null | grep -c "^${meth}_${scn}_.*csv$")
    [ "$n" -ge "$runs" ] && break
    echo "--- cell $dir $meth pass $pass ($n/$runs) $(date)" >> $LOG
    ./Ex1Runner.exe --method $meth --fibre $fibre --runs $runs --fes $fes --fr 1.0 --out $dir --dop 8 "$@" >> $LOG 2>&1
  done
}
echo "=== STAGE 1 (20 000 FE) $(date)" >> $LOG
for scn in 0 1; do for arm in A ABC; do for meth in ATO YAKO; do
  name=$([ "$scn" = 1 ] && echo fibre || echo plain)
  runcell $OUT/20k/${name}_$arm $meth $scn 20000 10 $(flags_for $arm $scn)
done; done; done
echo "=== STAGE 2 decision $(date)" >> $LOG
python -I ../tools/maxfe_decision.py $OUT/20k $OUT/maxfe_decision_20k.txt >> $LOG 2>&1
if grep -q "^NOT_SETTLED" $OUT/maxfe_decision_20k.txt; then
  echo "=== STAGE 2b 30 000 FE for unsettled cells $(date)" >> $LOG
  grep "^RERUN" $OUT/maxfe_decision_20k.txt | while read _ scn arm meth; do
    f=$([ "$scn" = fibre ] && echo 1 || echo 0)
    runcell $OUT/30k/${scn}_$arm $meth $f 30000 10 $(flags_for $arm $f)
  done
fi
echo "=== STAGE 3 BTO all-dimension control (supplementary) $(date)" >> $LOG
for scn in 0 1; do for arm in A C; do
  name=$([ "$scn" = 1 ] && echo fibre || echo plain)
  if [ $arm = A ]; then fl="--derive 1"; else fl="--derive 1 --predesign 1 --usebox 0"; fi
  runcell $OUT/bto_alldim/${name}_$arm BTO $scn 10000 30 $fl
done; done
echo "ALL DONE $(date)" >> $LOG
