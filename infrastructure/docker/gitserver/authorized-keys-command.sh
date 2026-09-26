#!/usr/bin/env bash

OS_USER="$1"
KEY_TYPE="$2"
KEY_BLOB="$3"

: "${AUTH_API_URL:?AUTH_API_URL must be set}"

RESPONSE=$(curl -fsS --max-time 3 -H "Content-Type: application/json" -d "{\"key_type\":\"${KEY_TYPE}\",\"key\":\"${KEY_BLOB}\"}" "${AUTH_API_URL}/internal/authorized-keys" 2>/dev/null)" || exit 0)

printf "%s\n" "$RESPONSE"
