#!/bin/bash
LOG="$(dirname "$0")/../04_Results/phase3/phase3_log.txt"
while ! grep -q "ALL DONE 3c" "$LOG"; do sleep 300; done
echo "PHASE3C_DONE $(date)"
