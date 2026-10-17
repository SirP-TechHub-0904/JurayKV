#!/bin/bash
set -e

# ==============================================================================
# Helper Script: Install ASP.NET Core 7.0 Runtime on Hostinger Linux VPS
# ==============================================================================

echo "=================================================="
echo "📥 Installing ASP.NET Core 7.0 Runtime on VPS..."
echo "=================================================="

SUDO=""
if [ "$(id -u)" -ne 0 ]; then
    SUDO="sudo"
fi

# Download official Microsoft installer script
wget -q https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
chmod +x /tmp/dotnet-install.sh

# Install ASP.NET Core 7.0 runtime to /usr/share/dotnet
$SUDO /tmp/dotnet-install.sh --channel 7.0 --runtime aspnetcore --install-dir /usr/share/dotnet

# Create global symlink
$SUDO ln -sf /usr/share/dotnet/dotnet /usr/bin/dotnet

echo "✅ ASP.NET Core 7.0 Runtime installed successfully!"
dotnet --info
