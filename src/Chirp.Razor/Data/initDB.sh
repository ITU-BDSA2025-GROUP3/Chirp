#!/usr/bin/env bash
set -euo pipefail

# Kør altid ift. mappen hvor scriptet ligger
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DB="${SCRIPT_DIR}/../chirp.db"   # lægger DB én mappe op

sqlite3 "$DB" < "${SCRIPT_DIR}/schema.sql"
sqlite3 "$DB" < "${SCRIPT_DIR}/dump.sql"

echo "✔ DB klar: $DB"
