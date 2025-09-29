#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Brug CHIRPDBPATH hvis sat, ellers /tmp/chirp.db (matcher din C#-fallback)
DB="${CHIRPDBPATH:-/tmp/chirp.db}"

sqlite3 "$DB" < "${SCRIPT_DIR}/schema.sql"
sqlite3 "$DB" < "${SCRIPT_DIR}/dump.sql"

echo "✔ DB klar: $DB"
