#!/bin/bash
set -e

# Get the directory where the script is located
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Change to the script directory
cd "$SCRIPT_DIR"

echo "🚀 Building dynamic-scheduler action from $(pwd)..."

# Clean previous build
echo "🧹 Cleaning previous build..."
rm -rf dist/
rm -rf node_modules/

# Install dependencies
echo "📦 Installing dependencies..."
if [ -f "package-lock.json" ]; then
    npm ci
else
    echo "⚠️ No package-lock.json found, using npm install..."
    npm install
fi

# Build TypeScript
echo "🔨 Building TypeScript..."
npm run build

# Package the action
echo "📦 Packaging action..."
npm run package

# Verify the build
if [ ! -f "dist/index.js" ]; then
    echo "❌ Build failed: dist/index.js not found"
    exit 1
fi

echo "✅ Build completed successfully!" 