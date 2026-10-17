#!/bin/bash
set -e

# ==============================================================================
# JurayKV UI (Web) Deployment Script (Bare-Metal Systemd)
# ==============================================================================

DEPLOY_BASE="${1:-$HOME/apps/juraykv}"
UI_DIR="$DEPLOY_BASE/ui"
STAGING_DIR="$DEPLOY_BASE/ui-staging"
ENV_FILE="$DEPLOY_BASE/.env"

CURRENT_USER=$(whoami)
SUDO=""
if [ "$(id -u)" -ne 0 ]; then
    SUDO="sudo"
fi

echo "=================================================="
echo "🚀 Deploying JurayKV UI (Web .NET 7) to Systemd"
echo "📂 Target Directory: $UI_DIR"
echo "👤 Run As User:      $CURRENT_USER"
echo "=================================================="

# Ensure directories exist
mkdir -p "$UI_DIR" "$STAGING_DIR"

# ------------------------------------------------------------------------------
# 1. Environment File (.env) Handling: First Deploy vs Subsequent
# ------------------------------------------------------------------------------
if [ ! -f "$ENV_FILE" ] || [ $(wc -c < "$ENV_FILE" 2>/dev/null || echo 0) -lt 30 ] || [ "$FORCE_UPDATE_ENV" = "true" ]; then
    if [ -n "$INITIAL_ENV_B64" ]; then
        echo "🔑 Writing .env from GitHub Secret (Base64)..."
        echo "$INITIAL_ENV_B64" | base64 -d > "$ENV_FILE"
        chmod 600 "$ENV_FILE"
        echo "🔒 Set .env permissions to 600."
    elif [ ! -f "$ENV_FILE" ]; then
        echo "⚠️ WARNING: APP_ENV secret is missing from GitHub Secrets! Creating placeholder .env..."
        touch "$ENV_FILE"
        chmod 600 "$ENV_FILE"
    else
        echo "⚠️ Existing .env is almost empty, but no APP_ENV secret was provided to fill it."
    fi
else
    echo "✅ Existing .env found ($(wc -l < "$ENV_FILE") lines). Preserving current secrets."
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

# Stop UI service temporarily to test if port belongs to us
$SUDO systemctl stop juraykv-ui 2>/dev/null || true

CURRENT_UI_PORT=$(grep -E '^UI_PORT=' "$ENV_FILE" 2>/dev/null | cut -d '=' -f2 | tr -d '\r\n')
if [ -n "$CURRENT_UI_PORT" ] && ! is_port_in_use "$CURRENT_UI_PORT"; then
    FINAL_UI_PORT="$CURRENT_UI_PORT"
    echo "✅ Reusing configured UI_PORT: $FINAL_UI_PORT"
else
    echo "🔍 Scanning for free port for UI (starting at 5000)..."
    FINAL_UI_PORT=$(find_free_port 5000)
    echo "🎯 Assigned free port for UI: $FINAL_UI_PORT"
fi

sed -i '/^UI_PORT=/d' "$ENV_FILE"
echo "UI_PORT=$FINAL_UI_PORT" >> "$ENV_FILE"

# ------------------------------------------------------------------------------
# 3. Swap Staging Binaries into Live Directory
# ------------------------------------------------------------------------------
if [ -d "$STAGING_DIR" ] && [ "$(ls -A "$STAGING_DIR" 2>/dev/null)" ]; then
    echo "📦 Moving published binaries to $UI_DIR..."
    cp -rf "$STAGING_DIR"/* "$UI_DIR"/
    rm -rf "$STAGING_DIR"/*
else
    echo "⚠️ No staging binaries found; assuming files already in $UI_DIR."
fi

# ------------------------------------------------------------------------------
# 4. Create / Update Systemd Service Unit
# ------------------------------------------------------------------------------
SERVICE_FILE="/etc/systemd/system/juraykv-ui.service"
echo "⚙️ Configuring Systemd service at $SERVICE_FILE..."

$SUDO bash -c "cat > $SERVICE_FILE" <<EOF
[Unit]
Description=JurayKV Web UI (.NET 7)
After=network.target

[Service]
WorkingDirectory=$UI_DIR
ExecStart=/usr/bin/dotnet $UI_DIR/JurayKV.UI.dll --urls "http://127.0.0.1:$FINAL_UI_PORT"
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=juraykv-ui
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
echo "🔄 Reloading Systemd and restarting juraykv-ui..."
$SUDO systemctl daemon-reload
$SUDO systemctl enable juraykv-ui
$SUDO systemctl restart juraykv-ui

echo "⏳ Waiting 5 seconds for UI to initialize..."
sleep 5

# ------------------------------------------------------------------------------
# 6. Service Verification
# ------------------------------------------------------------------------------
if $SUDO systemctl is-active --quiet juraykv-ui; then
    echo "=================================================="
    echo "🎉 JURAYKV UI DEPLOYED SUCCESSFULLY!"
    echo "🌐 Active on: http://127.0.0.1:$FINAL_UI_PORT"
    echo "=================================================="
else
    echo "❌ juraykv-ui failed to start. Last service logs:"
    $SUDO journalctl -u juraykv-ui -n 50 --no-pager
    if [ -d "$UI_DIR/logs" ]; then
        echo "📄 Application error logs from $UI_DIR/logs:"
        cat "$UI_DIR"/logs/*.log 2>/dev/null | tail -n 40 || true
    fi
    exit 1
fi
