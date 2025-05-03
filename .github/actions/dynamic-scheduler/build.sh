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
echo "🔍 Verifying build files..."
required_files=(
    "dist/index.js"
    "dist/index.js.map"
    "dist/licenses.txt"
    "dist/sourcemap-register.js"
)

for file in "${required_files[@]}"; do
    if [ ! -f "$file" ]; then
        echo "❌ Build failed: $file not found"
        exit 1
    fi
done

echo "✅ Build completed successfully!" 