#!/bin/bash

echo "🛑 Stopping all TaskBoard services..."
echo "================================================"

# Navigate to docker directory
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_ROOT/docker"

# Stop all compose files
docker-compose -f docker-compose.yml down
docker-compose -f docker-compose.dev.yml down
docker-compose -f docker-compose.staging.yml down
docker-compose -f docker-compose.prod.yml down

echo ""
echo "✅ All services stopped!"
echo "================================================"
