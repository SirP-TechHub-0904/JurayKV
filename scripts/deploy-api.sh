#!/bin/bash
set -e

# ==============================================================================
# JurayKV API Deployment Script (Bare-Metal Systemd)
# Independent .env located in ~/apps/juraykv/api/.env
# ==============================================================================

DEPLOY_BASE="${1:-$HOME/apps/juraykv}"
API_DIR="$DEPLOY_BASE/api"
STAGING_DIR="$DEPLOY_BASE/api-staging"
ENV_FILE="$API_DIR/.env"

CURRENT_USER=$(whoami)
SUDO=""
if [ "$(id -u)" -ne 0 ]; then
    SUDO="sudo"
fi

echo "=================================================="
echo "🚀 Deploying JurayKV API (.NET 7) to Systemd"
echo "📂 Target Directory: $API_DIR"
echo "📄 Config File:      $ENV_FILE"
echo "👤 Run As User:      $CURRENT_USER"
echo "=================================================="

# Ensure directories exist
mkdir -p "$API_DIR" "$STAGING_DIR"

# ------------------------------------------------------------------------------
# 1. Port Detection Helpers
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

# ------------------------------------------------------------------------------
# 2. Independent API .env File Management
# ------------------------------------------------------------------------------
CURRENT_API_PORT=""
if [ -f "$ENV_FILE" ]; then
    CURRENT_API_PORT=$(grep -E '^API_PORT=' "$ENV_FILE" 2>/dev/null | cut -d '=' -f2 | tr -d '\r\n')
fi

# Detect free port
if [ -n "$CURRENT_API_PORT" ] && ! is_port_in_use "$CURRENT_API_PORT"; then
    FINAL_API_PORT="$CURRENT_API_PORT"
    echo "✅ Reusing configured API_PORT: $FINAL_API_PORT"
else
    echo "🔍 Scanning for free port for API (starting at 5001)..."
    FINAL_API_PORT=$(find_free_port 5001)
    echo "🎯 Assigned free port for API: $FINAL_API_PORT"
fi

if [ ! -f "$ENV_FILE" ]; then
    echo "📝 First deploy: Generating API template .env at $ENV_FILE..."
    cat > "$ENV_FILE" <<EOF
API_PORT=$FINAL_API_PORT
ASPNETCORE_ENVIRONMENT=Production

# Database Connection (update with your actual database password via nano)
ConnectionStrings__DefaultConnection=Server=147.93.128.160,1433;Database=db_a64c98_kview;User Id=user_db;Password=Exwhyzee@123;TrustServerCertificate=True;

# JWT Authentication
Jwt__Key=2E56uS1k+pWFPQcVsflFqyRjeMlNxRVWWkYgcFEV4jdedRMdjJQvjVRBAwlnN1LeukYzMMjm8X8Rzq3AGMVlaA==
Jwt__Issuer=SampleIdentity.com
Jwt__LifeTime=86400

# External Login Providers (OAuth)
ExternalLoginProviders__Google__ClientId=826428690319-r7q1543cl4lmcrkc9pavovin4m3mfvms.apps.googleusercontent.com
ExternalLoginProviders__Google__ClientSecret=Bd15s3IdSHMDu0niCdwar66q
ExternalLoginProviders__Facebook__AppId=2352153101679325
ExternalLoginProviders__Facebook__AppSecret=a1fd8d2739d6130883d29dd4bab55fcd
ExternalLoginProviders__Twitter__ConsumerKey=7RJZ92eHoN3CDih8A4fNQISB0
ExternalLoginProviders__Twitter__ConsumerSecret=FHR6n9r6KoVj5GEzWFJslRL2m295zrI1QGEWsqCpmWIbx7gGX3
ExternalLoginProviders__Microsoft__ApplicationId=e606555b-9ee9-4b34-a886-3fd9db93e9fe
ExternalLoginProviders__Microsoft__Password=yrABB723%]{tkujmMDYI11)
EOF
    chmod 600 "$ENV_FILE"
    echo "🔒 Set $ENV_FILE permissions to 600."
else
    echo "✅ Preserving existing $ENV_FILE. Updating API_PORT..."
    sed -i '/^API_PORT=/d' "$ENV_FILE"
    echo "API_PORT=$FINAL_API_PORT" >> "$ENV_FILE"
fi

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
    echo "🎉 JURAYKV API DEPLOYED & RUNNING!"
    echo "🔗 Active on: http://127.0.0.1:$FINAL_API_PORT"
    echo "=================================================="
else
    echo "⚠️ juraykv-api is not active yet."
    echo "Recent logs from journalctl:"
    $SUDO journalctl -u juraykv-api -n 30 --no-pager || true
    echo "=================================================="
    echo "💡 To configure your API secrets via nano:"
    echo "   nano $ENV_FILE"
    echo "   sudo systemctl restart juraykv-api"
    echo "=================================================="
fi
