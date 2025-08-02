#!/usr/bin/env bash
set -euo pipefail

# load API key from config.env (assumes config.env sits next to this script)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck disable=SC1090
source "$SCRIPT_DIR/../config.env"

# run your Go app (from the repo root’s go folder)
cd "$SCRIPT_DIR/../go"
go run .