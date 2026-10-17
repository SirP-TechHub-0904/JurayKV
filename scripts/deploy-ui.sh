#!/bin/bash
set -e

# ==============================================================================
# JurayKV UI (Web) Deployment Script (Bare-Metal Systemd)
# Independent .env located in ~/apps/juraykv/ui/.env
# ==============================================================================

DEPLOY_BASE="${1:-$HOME/apps/juraykv}"
UI_DIR="$DEPLOY_BASE/ui"
STAGING_DIR="$DEPLOY_BASE/ui-staging"
ENV_FILE="$UI_DIR/.env"

CURRENT_USER=$(whoami)
SUDO=""
if [ "$(id -u)" -ne 0 ]; then
    SUDO="sudo"
fi

echo "=================================================="
echo "🚀 Deploying JurayKV UI (Web .NET 7) to Systemd"
echo "📂 Target Directory: $UI_DIR"
echo "📄 Config File:      $ENV_FILE"
echo "👤 Run As User:      $CURRENT_USER"
echo "=================================================="

# Ensure directories exist
mkdir -p "$UI_DIR" "$STAGING_DIR"

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

# Stop UI service temporarily to test if port belongs to us
$SUDO systemctl stop juraykv-ui 2>/dev/null || true

# ------------------------------------------------------------------------------
# 2. Independent UI .env File Management
# ------------------------------------------------------------------------------
CURRENT_UI_PORT=""
if [ -f "$ENV_FILE" ]; then
    CURRENT_UI_PORT=$(grep -E '^UI_PORT=' "$ENV_FILE" 2>/dev/null | cut -d '=' -f2 | tr -d '\r\n')
fi

# Detect free port
if [ -n "$CURRENT_UI_PORT" ] && ! is_port_in_use "$CURRENT_UI_PORT"; then
    FINAL_UI_PORT="$CURRENT_UI_PORT"
    echo "✅ Reusing configured UI_PORT: $FINAL_UI_PORT"
else
    echo "🔍 Scanning for free port for UI (starting at 5000)..."
    FINAL_UI_PORT=$(find_free_port 5000)
    echo "🎯 Assigned free port for UI: $FINAL_UI_PORT"
fi

if [ ! -f "$ENV_FILE" ]; then
    echo "📝 First deploy: Generating UI template .env at $ENV_FILE..."
    cat > "$ENV_FILE" <<EOF
UI_PORT=$FINAL_UI_PORT
ASPNETCORE_ENVIRONMENT=Production

# Database Connection (update with your actual database password via nano)
ConnectionStrings__DefaultConnection=Server=147.93.128.160,1433;Database=db_a64c98_kview;User Id=user_db;Password=Exwhyzee@123;TrustServerCertificate=True;

# Payment Gateways - Flutterwave / Rave
RaveConfig__IsLive=false
RaveConfig__PbfPubKey=FLWPUBK_TEST-f4bb871959b328ba5c04417f81263902-X
RaveConfig__SecretKey=FLWSECK_TEST-2e538587a52c4836a1e5860199817f71-X
FLWSECK-Key=FLWSECK-3ebc176be7413ec684592804c5cd98b7-X
FLWSECK_Key=FLWSECK-3ebc176be7413ec684592804c5cd98b7-X

# Email - Postmark
PostmarkSettings__ServerToken=dbdbf019-d67e-4752-bd26-12d8d610b805

# Payment Gateways - Interswitch
Interswitch__ClientId=IKIA5AA595B96948BB9AB2352EDC62DCAB7EF74F3715
Interswitch__ClientSecret=xQaenok1q2xOXlUSA+m5ZZk0hl59lwfRBOQtiMw3BCw=
Interswitch__MerchantCode=VNA

# Payment Gateways - Opay
OpayInit__BaseUrl=https://testapi.opaycheckout.com
OpayInit__MarchantId=256623121083870
OpayInit__PublicKey=OPAYPUB17021995948040.14617475491626442
OpayInit__PrivateKey=OPAYPRV17021995948040.4458307725859857

# VTU Service
VTU__BaseUrl=https://vtu.ng/wp-json/api/v1/
VTU__Username=XYZTECH
VTU__SecretKey=AhambuJuray@123

# SMS Service
SmsApiToken=FdxLOHNVfiy9GTe4bsElIw5WA1oDJcvCMP8072rYn3ZhXaRzmBgqtuSQUjk6Kp

# Twilio SMS
TWILIO_ACCOUNT_SID=AC9415f9094a6a0f2f68ab9163e1c89fe6
TWILIO_AUTH_TOKEN=336ebfe3b04709e989042533c2051721

# AWS S3 / Cloud Storage
AwsConfiguration__AWSAccessKey=AKIA4Y267QYK25XMO6WE
AwsConfiguration__AWSSecretKey=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
EOF
    chmod 600 "$ENV_FILE"
    echo "🔒 Set $ENV_FILE permissions to 600."
else
    echo "✅ Preserving existing $ENV_FILE. Updating UI_PORT..."
    sed -i '/^UI_PORT=/d' "$ENV_FILE"
    echo "UI_PORT=$FINAL_UI_PORT" >> "$ENV_FILE"
fi

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
    echo "🎉 JURAYKV UI DEPLOYED & RUNNING!"
    echo "🌐 Active on: http://127.0.0.1:$FINAL_UI_PORT"
    echo "=================================================="
else
    echo "⚠️ juraykv-ui is not active yet."
    echo "Recent logs from journalctl:"
    $SUDO journalctl -u juraykv-ui -n 30 --no-pager || true
    echo "=================================================="
    echo "💡 To configure your UI secrets via nano:"
    echo "   nano $ENV_FILE"
    echo "   sudo systemctl restart juraykv-ui"
    echo "=================================================="
fi
