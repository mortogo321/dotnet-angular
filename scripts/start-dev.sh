#!/bin/bash

echo "🚀 Starting TaskBoard in Development mode..."
echo "================================================"

# Navigate to docker directory
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_ROOT/docker"

# Stop any running containers
echo "📦 Stopping existing containers..."
docker-compose -f docker-compose.yml down

# Start only PostgreSQL for local development
echo "🐘 Starting PostgreSQL..."
docker-compose -f docker-compose.dev.yml up -d

echo ""
echo "✅ Development environment ready!"
echo "================================================"
echo "📊 PostgreSQL: localhost:5432"
echo "   Database: taskboard_dev"
echo "   Username: postgres"
echo "   Password: postgres"
echo ""
echo "To start the backend:"
echo "   cd backend/src/TaskBoard.Api && dotnet run"
echo ""
echo "To start the frontend:"
echo "   cd frontend && pnpm start"
echo "================================================"
