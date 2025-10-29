#!/usr/local/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

BUILD_PID=$(pwsh -NoLogo -NoProfile -File "${SCRIPT_DIR}/build.ps1")

while ps -p "$BUILD_PID" > /dev/null; do
  clear
  ps -p "$BUILD_PID" -o pid,etime,%cpu,%mem,command
  sleep 1
done
