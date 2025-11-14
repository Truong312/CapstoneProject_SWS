#!/usr/bin/env bash
set -euo pipefail

# Usage: ./run_mac.sh [MODEL] [COMPUTE]
# Example: ./run_mac.sh small float16

PORT=8001
STATIC_PORT=8080
MODEL=${1:-small}
COMPUTE=${2:-}

# Change to script directory (project root)
cd "$(dirname "$0")"

echo "Using MODEL=${MODEL}, COMPUTE='${COMPUTE}'"

# Kill any process listening on the port
pids=$(lsof -t -iTCP:${PORT} -sTCP:LISTEN -n -P || true)
if [ -n "$pids" ]; then
  echo "Killing processes on :${PORT} -> $pids"
  kill $pids || true
fi

# Ensure virtualenv exists and dependencies installed
if [ ! -d ".venv" ]; then
  echo "Creating virtualenv .venv and installing requirements..."
  python3 -m venv .venv
  .venv/bin/python -m pip install --upgrade pip setuptools wheel
  .venv/bin/python -m pip install -r requirements.txt
fi

export WHISPER_MODEL="$MODEL"
export WHISPER_COMPUTE="$COMPUTE"

# Start uvicorn in background and redirect logs
echo "Starting uvicorn on 127.0.0.1:${PORT} (logs -> uvicorn.log)"
if [ -n "$COMPUTE" ]; then
  WHISPER_MODEL="$MODEL" WHISPER_COMPUTE="$COMPUTE" .venv/bin/uvicorn main:app --host 127.0.0.1 --port ${PORT} --log-level info > uvicorn.log 2>&1 &
else
  WHISPER_MODEL="$MODEL" .venv/bin/uvicorn main:app --host 127.0.0.1 --port ${PORT} --log-level info > uvicorn.log 2>&1 &
fi
uv_pid=$!
echo "uvicorn pid: $uv_pid"

# Start a static server for test.html
http_pids=$(lsof -t -iTCP:${STATIC_PORT} -sTCP:LISTEN -n -P || true)
if [ -n "$http_pids" ]; then
  echo "Killing processes on :${STATIC_PORT} -> $http_pids"
  kill $http_pids || true
fi

echo "Starting static http.server on ${STATIC_PORT} (serving test.html) -> httpserver.log"
python3 -m http.server ${STATIC_PORT} > httpserver.log 2>&1 &
http_pid=$!

echo "static server pid: $http_pid"

echo "Waiting a moment for servers to initialize..."
sleep 2

echo "Server URLs:"
echo "  API: http://127.0.0.1:${PORT}/"
echo "  Test page: http://127.0.0.1:${STATIC_PORT}/test.html"

echo "Tailing uvicorn.log (press Ctrl-C to stop). Logs are also in uvicorn.log and httpserver.log"

# Follow the log file to keep the script in foreground
if [ -f uvicorn.log ]; then
  tail -n +1 -f uvicorn.log
else
  echo "(uvicorn.log not found yet)"
  sleep 1
  tail -n +1 -f uvicorn.log
fi

