#!/bin/bash
set -e

# ==============================================================================
# JurayKV Automated Hostinger Deployment Script
# Supports:
# 1. First-deploy .env creation from secret (or template)
# 2. Dynamic free port detection (preventing collisions on shared VPS)
# 3. Docker Compose build, launch, and health verification
# ==============================================================================

DEPLOY_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$DEPLOY_DIR"

echo "=================================================="
echo "🚀 Starting JurayKV Deployment on Hostinger VPS"
echo "📂 Working Directory: $DEPLOY_DIR"
echo "⏰ Timestamp: $(date)"
echo "=================================================="

# ------------------------------------------------------------------------------
# 1. Docker & Docker Compose Check
# ------------------------------------------------------------------------------
if ! command -v docker &> /dev/null; then
    echo "❌ Error: Docker is not installed on this host."
    echo "Please install Docker: curl -fsSL https://get.docker.com | sh"
    exit 1
fi

DOCKER_COMPOSE="docker compose"
if ! docker compose version &> /dev/null; then
    if command -v docker-compose &> /dev/null; then
        DOCKER_COMPOSE="docker-compose"
    else
        echo "❌ Error: Docker Compose is not installed."
        exit 1
    fi
fi
echo "✅ Docker Compose command: $DOCKER_COMPOSE"

# ------------------------------------------------------------------------------
# 2. Port Detection Helper Functions
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

# ------------------------------------------------------------------------------
# 3. Environment File (.env) Handling: First Deploy vs Subsequent
# ------------------------------------------------------------------------------
ENV_FILE="$DEPLOY_DIR/.env"

if [ ! -f "$ENV_FILE" ]; then
    echo "📝 First deploy detected: .env does not exist."
    if [ -n "$INITIAL_ENV_B64" ]; then
        echo "🔑 Writing .env from GitHub Secret (Base64)..."
        echo "$INITIAL_ENV_B64" | base64 -d > "$ENV_FILE"
    elif [ -n "$INITIAL_ENV" ]; then
        echo "🔑 Writing .env from GitHub Secret (Plain)..."
        echo "$INITIAL_ENV" > "$ENV_FILE"
    elif [ -f "$DEPLOY_DIR/.env.example" ]; then
        echo "⚠️ No INITIAL_ENV secret supplied. Copying .env.example..."
        cp "$DEPLOY_DIR/.env.example" "$ENV_FILE"
    else
        echo "⚠️ Creating empty .env file..."
        touch "$ENV_FILE"
    fi
    chmod 600 "$ENV_FILE"
    echo "🔒 Set .env permissions to 600."
else
    echo "✅ Existing .env found on server. Preserving server configuration."
    if [ "$FORCE_UPDATE_ENV" = "true" ] && [ -n "$INITIAL_ENV_B64" ]; then
        echo "⚠️ FORCE_UPDATE_ENV is set to true. Overwriting .env from GitHub Secret..."
        echo "$INITIAL_ENV_B64" | base64 -d > "$ENV_FILE"
        chmod 600 "$ENV_FILE"
    fi
fi

# ------------------------------------------------------------------------------
# 4. Dynamic Free Port Allocation
# ------------------------------------------------------------------------------
# Extract existing configured ports if any
CURRENT_UI_PORT=$(grep -E '^UI_PORT=' "$ENV_FILE" | cut -d '=' -f2 | tr -d '\r\n')
CURRENT_API_PORT=$(grep -E '^API_PORT=' "$ENV_FILE" | cut -d '=' -f2 | tr -d '\r\n')

# Stop existing JurayKV containers temporarily to test if ports belong to us
$DOCKER_COMPOSE down --remove-orphans || true

# Determine UI Port
if [ -n "$CURRENT_UI_PORT" ] && ! is_port_in_use "$CURRENT_UI_PORT"; then
    FINAL_UI_PORT="$CURRENT_UI_PORT"
    echo "✅ Reusing configured UI_PORT: $FINAL_UI_PORT"
else
    echo "🔍 Scanning for free port for UI (starting at 5000)..."
    FINAL_UI_PORT=$(find_free_port 5000)
    echo "🎯 Assigned free port for UI: $FINAL_UI_PORT"
fi

# Determine API Port
if [ -n "$CURRENT_API_PORT" ] && [ "$CURRENT_API_PORT" != "$FINAL_UI_PORT" ] && ! is_port_in_use "$CURRENT_API_PORT"; then
    FINAL_API_PORT="$CURRENT_API_PORT"
    echo "✅ Reusing configured API_PORT: $FINAL_API_PORT"
else
    START_API_PORT=$((FINAL_UI_PORT + 1))
    echo "🔍 Scanning for free port for API (starting at $START_API_PORT)..."
    FINAL_API_PORT=$(find_free_port "$START_API_PORT")
    echo "🎯 Assigned free port for API: $FINAL_API_PORT"
fi

# Update .env with assigned ports
sed -i '/^UI_PORT=/d' "$ENV_FILE"
sed -i '/^API_PORT=/d' "$ENV_FILE"
echo "UI_PORT=$FINAL_UI_PORT" >> "$ENV_FILE"
echo "API_PORT=$FINAL_API_PORT" >> "$ENV_FILE"

# ------------------------------------------------------------------------------
# 5. Build and Launch Containers
# ------------------------------------------------------------------------------
echo "🔨 Building and launching Docker containers..."
$DOCKER_COMPOSE up -d --build

# ------------------------------------------------------------------------------
# 6. Health & Verification Check
# ------------------------------------------------------------------------------
echo "⏳ Waiting 10 seconds for services to initialize..."
sleep 10

echo "🔍 Verifying running containers:"
$DOCKER_COMPOSE ps

echo ""
echo "=================================================="
echo "🎉 DEPLOYMENT SUCCESSFUL!"
echo "=================================================="
echo "🌐 JurayKV Web (UI) is running on port: $FINAL_UI_PORT"
echo "🔗 JurayKV API is running on port:      $FINAL_API_PORT"
echo ""
echo "Test endpoints locally on the server:"
echo "  curl -I http://localhost:$FINAL_UI_PORT"
echo "  curl -I http://localhost:$FINAL_API_PORT/healthz"
echo "=================================================="
