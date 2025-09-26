#!/bin/bash
sqlite3 /tmp/chirp.db < Data/schema.sql
sqlite3 /tmp/chirp.db < Data/dump.sql
