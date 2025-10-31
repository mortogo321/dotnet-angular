#!/bin/bash

echo "🚀 Starting Full TaskBoard Stack..."
echo "================================================"

# Navigate to docker directory
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_ROOT/docker"

# Build and start all services
echo "🔨 Building and starting all services..."
docker-compose -f docker-compose.yml up --build -d

echo ""
echo "⏳ Waiting for services to be healthy..."
sleep 10

echo ""
echo "✅ Full stack is running!"
echo "================================================"
echo "🎨 Frontend: http://localhost:4200"
echo "🔧 Backend API: http://localhost:5000"
echo "📊 PostgreSQL: localhost:5432"
echo ""
echo "View logs:"
echo "   docker-compose -f docker/docker-compose.yml logs -f"
echo ""
echo "Stop all services:"
echo "   ./scripts/stop-all.sh"
echo "================================================"
