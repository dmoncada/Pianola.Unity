#!/usr/local/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

BUILD_PID=$(pwsh -NoLogo -NoProfile -File "${SCRIPT_DIR}/build.ps1")

START_TIME=$(date +%s)

while ps -p "$BUILD_PID" > /dev/null; do
  clear
  ps -p "$BUILD_PID" -o pid,etime,%cpu,%mem,command
  sleep 1
done

END_TIME=$(date +%s)
ELAPSED=$((END_TIME - START_TIME))

if [ "$ELAPSED" -lt 60 ]; then
    echo "Finish building in ${ELAPSED} seconds."
else
    ELAPSED_MINS=$((ELAPSED / 60))
    ELAPSED_SECS=$((ELAPSED % 60))
    echo "Finish building in ${ELAPSED_MINS} minutes, ${ELAPSED_SECS} seconds."
fi
