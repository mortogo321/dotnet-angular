# 🎯 TaskBoard - Real-time Trello-like Task Management System

A modern, full-stack task management application built with the latest .NET 9 and Angular 18+ technologies, featuring real-time collaboration powered by SignalR.

## 🚀 Tech Stack

### Backend (.NET 9)

- **Framework**: ASP.NET Core 9.0 with Minimal APIs
- **Real-time**: SignalR for WebSocket communication
- **Database**: PostgreSQL with Entity Framework Core 9
- **Architecture**: Clean Architecture (Core, Infrastructure, API layers)
- **Features**:
  - Minimal API endpoints for performance
  - Repository pattern with full EF Core support
  - Automatic database migrations
  - Multi-environment configuration (Dev, Staging, Prod)

### Frontend (Angular 18+)

- **Framework**: Angular 18+ with Standalone Components
- **State Management**: Angular Signals for reactive state
- **UI Features**:
  - Drag-and-drop card/list management (Angular CDK)
  - Responsive design with SCSS
  - Real-time updates across clients
  - Lazy-loaded routes

### DevOps & Infrastructure

- **Containerization**: Multi-stage Docker builds
- **Orchestration**: Docker Compose for all environments
- **Reverse Proxy**: Nginx for production
- **CI/CD**: GitHub Actions pipeline
- **Database**: PostgreSQL 16 with health checks

## 📁 Project Structure

```
taskboard/
├── backend/                    # .NET 9 Backend
│   ├── src/
│   │   ├── TaskBoard.Api/      # Web API & SignalR Hub
│   │   │   ├── Endpoints/      # Minimal API endpoints
│   │   │   ├── Hubs/          # SignalR hubs
│   │   │   ├── DTOs/          # Data transfer objects
│   │   │   └── Program.cs     # Application entry point
│   │   ├── TaskBoard.Core/     # Domain Models & Interfaces
│   │   │   ├── Entities/      # Domain models (Board, List, Card)
│   │   │   └── Interfaces/    # Repository interfaces
│   │   └── TaskBoard.Infrastructure/  # Data Access & Repositories
│   │       ├── Data/
│   │       │   ├── Configurations/  # EF Core configurations
│   │       │   ├── Migrations/      # Database migrations
│   │       │   └── TaskBoardDbContext.cs
│   │       └── Repositories/        # Repository implementations
│   ├── .env.example           # Environment template
│   ├── .env.development       # Dev environment
│   ├── .env.staging           # Staging environment
│   ├── .env.production        # Production environment
│   ├── .dockerignore
│   ├── .gitignore
│   ├── Dockerfile             # Multi-stage Docker build
│   └── TaskBoard.sln          # Solution file
│
├── frontend/                   # Angular 18+ Frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/     # Standalone components
│   │   │   │   ├── board/      # Board view with drag-drop
│   │   │   │   └── boards-list/  # Boards listing
│   │   │   ├── services/       # API & SignalR Services
│   │   │   │   ├── api.service.ts      # HTTP client
│   │   │   │   ├── signalr.service.ts  # SignalR client
│   │   │   │   └── board.store.ts      # Signal-based state
│   │   │   ├── models/         # TypeScript interfaces
│   │   │   ├── app.config.ts   # App configuration
│   │   │   └── app.routes.ts   # Routing config
│   │   └── environments/       # Environment configs
│   │       ├── environment.ts
│   │       ├── environment.development.ts
│   │       └── environment.production.ts
│   ├── .env.example           # Environment template
│   ├── .env.development       # Dev environment
│   ├── .env.staging           # Staging environment
│   ├── .env.production        # Production environment
│   ├── .dockerignore
│   ├── .gitignore
│   ├── Dockerfile             # Multi-stage with Nginx
│   ├── angular.json           # Angular CLI config
│   └── package.json           # Dependencies (pnpm)
│
├── docker/                     # Docker Orchestration
│   ├── nginx/                  # Nginx configuration
│   │   ├── nginx.conf         # Main nginx config
│   │   └── default.conf       # Server config with proxy
│   ├── docker-compose.yml      # Full stack compose
│   ├── docker-compose.dev.yml  # Dev (PostgreSQL only)
│   ├── docker-compose.staging.yml  # Staging environment
│   └── docker-compose.prod.yml     # Production environment
│
├── scripts/                    # Helper Scripts
│   ├── start-dev.sh           # Start dev environment
│   ├── start-full.sh          # Start full stack
│   └── stop-all.sh            # Stop all services
│
├── .github/workflows/          # CI/CD Pipelines
│   └── ci-cd.yml              # GitHub Actions workflow
│
├── .gitignore                  # Root gitignore
└── README.md                   # This file
```

## 🏃 Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [pnpm](https://pnpm.io/) (Install: `npm install -g pnpm`)
- [Docker](https://www.docker.com/) & Docker Compose
- [PostgreSQL 16](https://www.postgresql.org/) (or use Docker)

### 🐳 Option 1: Full Stack with Docker (Recommended for POC)

```bash
# Start entire application with Docker
./scripts/start-full.sh

# Access the application
# Frontend: http://localhost:4200
# Backend API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### 💻 Option 2: Local Development

#### 0. Configure Environment (Optional)

The project includes environment files for both backend and frontend:

**Backend** (`backend/.env.*`):

```bash
# Uses .env.development by default
# Or create your own .env from .env.example
cp backend/.env.example backend/.env
```

**Frontend** (`frontend/.env.*`):

```bash
# Uses .env.development by default
# Or create your own .env from .env.example
cp frontend/.env.example frontend/.env
```

#### 1. Start PostgreSQL (Docker)

```bash
./scripts/start-dev.sh
```

Or manually:

```bash
docker-compose -f docker/docker-compose.dev.yml up -d
```

#### 2. Run Backend

```bash
cd backend/src/TaskBoard.Api
dotnet restore
dotnet ef database update --project ../../src/TaskBoard.Infrastructure/TaskBoard.Infrastructure.csproj
dotnet run
```

Backend will be available at: http://localhost:5000

#### 3. Run Frontend

```bash
cd frontend
pnpm install
pnpm start
```

Frontend will be available at: http://localhost:4200

### 📖 API Documentation (Swagger)

The backend API includes interactive Swagger documentation for easy testing and exploration:

**Development Mode:**
- **Swagger UI**: http://localhost:5000/swagger
- **OpenAPI Spec**: http://localhost:5000/swagger/v1/swagger.json

**Features:**
- Interactive API endpoint testing
- Request/response schema documentation
- Try out endpoints directly in the browser
- Automatic OpenAPI specification generation

**Note**: Swagger is only enabled in Development environment for security. To enable in other environments, modify `Program.cs` in the backend.

## 🔧 Environment Configuration

### Development

- **Backend**: Uses `appsettings.Development.json`
- **Frontend**: Uses `environment.development.ts`
- **Database**: PostgreSQL on `localhost:5432`

### Staging

```bash
# Configure staging environment
cp docker/.env.example docker/.env.staging
# Edit docker/.env.staging with your credentials

# Start staging
./docker/scripts/start-staging.sh
```

### Production

```bash
# Configure production environment
cp docker/.env.example docker/.env.production
# Edit docker/.env.production with your credentials

# Deploy production
docker-compose -f docker/docker-compose.prod.yml up -d
```

## 🎨 Key Features

### Backend Features

- ✅ **Minimal APIs** - Modern .NET 9 endpoint routing
- ✅ **SignalR Hub** - Real-time board updates
- ✅ **EF Core 9** - Code-first migrations with PostgreSQL
- ✅ **Repository Pattern** - Clean separation of concerns
- ✅ **CORS Configuration** - Secure cross-origin requests
- ✅ **Auto-migrations** - Database updates on startup
- ✅ **Health Checks** - Application health monitoring

### Frontend Features

- ✅ **Standalone Components** - Modern Angular architecture
- ✅ **Angular Signals** - Reactive state management
- ✅ **SignalR Client** - Real-time WebSocket connection
- ✅ **Drag & Drop** - Angular CDK drag-drop
- ✅ **Lazy Loading** - Route-based code splitting
- ✅ **HTTP Client** - RESTful API communication
- ✅ **Responsive UI** - Mobile-friendly design

### Real-time Features

- 🔄 Live board updates
- 🔄 Instant card movements
- 🔄 Real-time list creation/updates
- 🔄 Multi-user collaboration
- 🔄 Automatic reconnection

## 📚 API Endpoints

### Boards

- `GET /api/boards` - Get all boards
- `GET /api/boards/{id}` - Get board by ID
- `POST /api/boards` - Create new board
- `PUT /api/boards/{id}` - Update board
- `DELETE /api/boards/{id}` - Delete board

### Lists

- `POST /api/lists` - Create new list
- `PUT /api/lists/{id}` - Update list
- `DELETE /api/lists/{id}` - Delete list
- `PATCH /api/lists/{id}/move` - Move list position

### Cards

- `POST /api/cards` - Create new card
- `PUT /api/cards/{id}` - Update card
- `DELETE /api/cards/{id}` - Delete card
- `PATCH /api/cards/{id}/move` - Move card to different list

### SignalR Hub

- **Endpoint**: `/hubs/taskboard`
- **Events**: BoardCreated, BoardUpdated, ListCreated, ListUpdated, CardCreated, CardUpdated, CardMoved

## 🔨 Development Scripts

### Backend Commands

```bash
# Build solution
dotnet build backend/TaskBoard.sln

# Run tests
dotnet test backend/TaskBoard.sln

# Add migration
dotnet ef migrations add MigrationName \
  --project backend/src/TaskBoard.Infrastructure \
  --startup-project backend/src/TaskBoard.Api

# Update database
dotnet ef database update \
  --project backend/src/TaskBoard.Infrastructure \
  --startup-project backend/src/TaskBoard.Api
```

### Frontend Commands

```bash
cd frontend

# Install dependencies
pnpm install

# Start dev server
pnpm start

# Build for production
pnpm build

# Run linting
pnpm lint

# Run tests
pnpm test
```

### Docker Commands

```bash
# Start dev (PostgreSQL only)
./docker/scripts/start-dev.sh

# Start full stack
./docker/scripts/start-full.sh

# Start staging
./docker/scripts/start-staging.sh

# Stop all services
./docker/scripts/stop-all.sh

# Clean up Docker resources
./docker/scripts/clean.sh

# View logs
docker-compose -f docker/docker-compose.yml logs -f

# Rebuild images
docker-compose -f docker/docker-compose.yml up --build
```

## 🏗️ Architecture Highlights

### Backend Clean Architecture

```
TaskBoard.Api (Presentation)
├── Endpoints/          # Minimal API endpoints
├── Hubs/              # SignalR hubs
└── DTOs/              # Data transfer objects

TaskBoard.Core (Domain)
├── Entities/          # Domain models
└── Interfaces/        # Repository interfaces

TaskBoard.Infrastructure (Data)
├── Data/
│   ├── Configurations/  # EF Core configurations
│   └── Migrations/      # Database migrations
└── Repositories/        # Repository implementations
```

### Frontend Architecture

```
app/
├── components/          # Standalone components
│   ├── board/          # Board view
│   └── boards-list/    # Boards listing
├── services/
│   ├── api.service.ts      # HTTP API client
│   ├── signalr.service.ts  # SignalR client
│   └── board.store.ts      # State management
└── models/             # TypeScript interfaces
```

## 🔐 Security Features

- ✅ CORS configuration
- ✅ Environment-based secrets
- ✅ Non-root Docker containers
- ✅ Nginx security headers
- ✅ Input validation
- ✅ SQL injection prevention (EF Core)

## 📊 Database Schema

```sql
Boards
├── Id (PK)
├── Title
├── Description
├── CreatedAt
└── UpdatedAt

Lists
├── Id (PK)
├── Title
├── Position
├── BoardId (FK)
├── CreatedAt
└── UpdatedAt

Cards
├── Id (PK)
├── Title
├── Description
├── Position
├── ListId (FK)
├── DueDate
├── Priority
├── Status
├── CreatedAt
└── UpdatedAt
```

## 🚢 Deployment

### Docker Production Deployment

```bash
# 1. Configure environment
cp docker/.env.example docker/.env.production
vim docker/.env.production

# 2. Build and deploy
docker-compose -f docker/docker-compose.prod.yml up -d

# 3. Check health
docker-compose -f docker/docker-compose.prod.yml ps
```

### CI Pipeline

The project includes a GitHub Actions workflow (`.github/workflows/ci-cd.yml`) that automatically:

1. ✅ Builds and tests backend (.NET 9)
2. ✅ Builds and tests frontend (Angular 18+)

The workflow runs on every push and pull request to `main` and `develop` branches.

## 🐛 Troubleshooting

### Backend issues

```bash
# Check backend logs
docker logs taskboard-backend

# Verify database connection
docker exec -it taskboard-postgres psql -U postgres -d taskboard_dev
```

### Frontend issues

```bash
# Check frontend logs
docker logs taskboard-frontend

# Rebuild node_modules
cd frontend && rm -rf node_modules && pnpm install
```

### Database issues

```bash
# Reset database
docker-compose -f docker/docker-compose.dev.yml down -v
docker-compose -f docker/docker-compose.dev.yml up -d

# Run migrations manually
cd backend/src/TaskBoard.Api
dotnet ef database update
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 🎓 Learning Resources

- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Angular Documentation](https://angular.io/docs)
- [SignalR Documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Angular Signals](https://angular.io/guide/signals)
- [Docker Documentation](https://docs.docker.com/)

## 👨‍💻 Author

Built with ❤️ using the latest .NET 9 and Angular 18+ features for a modern, scalable, real-time task management POC.

---

**Happy Coding! 🚀**
