#!/bin/bash
set -e

# ==============================================================================
# JurayKV API Deployment Script (Bare-Metal Systemd)
# ==============================================================================

DEPLOY_BASE="${1:-$HOME/apps/juraykv}"
API_DIR="$DEPLOY_BASE/api"
STAGING_DIR="$DEPLOY_BASE/api-staging"
ENV_FILE="$DEPLOY_BASE/.env"

CURRENT_USER=$(whoami)
SUDO=""
if [ "$(id -u)" -ne 0 ]; then
    SUDO="sudo"
fi

echo "=================================================="
echo "🚀 Deploying JurayKV API (.NET 7) to Systemd"
echo "📂 Target Directory: $API_DIR"
echo "👤 Run As User:      $CURRENT_USER"
echo "=================================================="

# Ensure directories exist
mkdir -p "$API_DIR" "$STAGING_DIR"

# ------------------------------------------------------------------------------
# 1. Environment File (.env) Handling: First Deploy vs Subsequent
# ------------------------------------------------------------------------------
if [ ! -f "$ENV_FILE" ]; then
    echo "📝 First deploy: .env does not exist."
    if [ -n "$INITIAL_ENV_B64" ]; then
        echo "🔑 Writing .env from GitHub Secret (Base64)..."
        echo "$INITIAL_ENV_B64" | base64 -d > "$ENV_FILE"
    elif [ -n "$INITIAL_ENV" ]; then
        echo "🔑 Writing .env from GitHub Secret (Plain)..."
        echo "$INITIAL_ENV" > "$ENV_FILE"
    else
        echo "⚠️ Creating empty .env file..."
        touch "$ENV_FILE"
    fi
    chmod 600 "$ENV_FILE"
    echo "🔒 Set .env permissions to 600."
else
    echo "✅ Existing .env found. Preserving current secrets."
    if [ "$FORCE_UPDATE_ENV" = "true" ] && [ -n "$INITIAL_ENV_B64" ]; then
        echo "⚠️ FORCE_UPDATE_ENV is true. Overwriting .env..."
        echo "$INITIAL_ENV_B64" | base64 -d > "$ENV_FILE"
        chmod 600 "$ENV_FILE"
    fi
fi

# ------------------------------------------------------------------------------
# 2. Port Detection Helpers
# ------------------------------------------------------------------------------
is_port_in_use() {
    local port=$1
    if command -v ss >/dev/null 2>&1; then
        ss -tuln | grep -E "(:|\])${port}\b" >/dev/null 2>&1
        return $?
    elif command -v netstat >/dev/null 2>&1; then
        netstat -tuln | grep -E "(:|\])${port}\b" >/dev/null 2>&1
        return $?
    elif command -v lsof >/dev/null 2>&1; then
        lsof -i ":${port}" >/dev/null 2>&1
        return $?
    else
        (echo >/dev/tcp/127.0.0.1/"${port}") >/dev/null 2>&1
        return $?
    fi
}

find_free_port() {
    local candidate=$1
    local max_port=65000
    while [ "$candidate" -le "$max_port" ]; do
        if ! is_port_in_use "$candidate"; then
            echo "$candidate"
            return 0
        fi
        candidate=$((candidate + 1))
    done
    echo "$1"
    return 1
}

# Stop API service temporarily to test if port belongs to us
$SUDO systemctl stop juraykv-api 2>/dev/null || true

CURRENT_API_PORT=$(grep -E '^API_PORT=' "$ENV_FILE" 2>/dev/null | cut -d '=' -f2 | tr -d '\r\n')
if [ -n "$CURRENT_API_PORT" ] && ! is_port_in_use "$CURRENT_API_PORT"; then
    FINAL_API_PORT="$CURRENT_API_PORT"
    echo "✅ Reusing configured API_PORT: $FINAL_API_PORT"
else
    echo "🔍 Scanning for free port for API (starting at 5001)..."
    FINAL_API_PORT=$(find_free_port 5001)
    echo "🎯 Assigned free port for API: $FINAL_API_PORT"
fi

sed -i '/^API_PORT=/d' "$ENV_FILE"
echo "API_PORT=$FINAL_API_PORT" >> "$ENV_FILE"

# ------------------------------------------------------------------------------
# 3. Swap Staging Binaries into Live Directory
# ------------------------------------------------------------------------------
if [ -d "$STAGING_DIR" ] && [ "$(ls -A "$STAGING_DIR" 2>/dev/null)" ]; then
    echo "📦 Moving published binaries to $API_DIR..."
    cp -rf "$STAGING_DIR"/* "$API_DIR"/
    rm -rf "$STAGING_DIR"/*
else
    echo "⚠️ No staging binaries found; assuming files already in $API_DIR."
fi

# ------------------------------------------------------------------------------
# 4. Create / Update Systemd Service Unit
# ------------------------------------------------------------------------------
SERVICE_FILE="/etc/systemd/system/juraykv-api.service"
echo "⚙️ Configuring Systemd service at $SERVICE_FILE..."

$SUDO bash -c "cat > $SERVICE_FILE" <<EOF
[Unit]
Description=JurayKV Web API (.NET 7)
After=network.target

[Service]
WorkingDirectory=$API_DIR
ExecStart=/usr/bin/dotnet $API_DIR/JurayKV.Api.dll --urls "http://127.0.0.1:$FINAL_API_PORT"
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=juraykv-api
User=$CURRENT_USER
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
EnvironmentFile=$ENV_FILE

[Install]
WantedBy=multi-user.target
EOF

# ------------------------------------------------------------------------------
# 5. Reload & Restart Service
# ------------------------------------------------------------------------------
echo "🔄 Reloading Systemd and restarting juraykv-api..."
$SUDO systemctl daemon-reload
$SUDO systemctl enable juraykv-api
$SUDO systemctl restart juraykv-api

echo "⏳ Waiting 5 seconds for API to initialize..."
sleep 5

# ------------------------------------------------------------------------------
# 6. Service Verification
# ------------------------------------------------------------------------------
if $SUDO systemctl is-active --quiet juraykv-api; then
    echo "=================================================="
    echo "🎉 JURAYKV API DEPLOYED SUCCESSFULLY!"
    echo "🔗 Active on: http://127.0.0.1:$FINAL_API_PORT"
    echo "=================================================="
else
    echo "❌ juraykv-api failed to start. Last service logs:"
    $SUDO journalctl -u juraykv-api -n 30 --no-pager
    exit 1
fi
