# TaskBoard

A Trello-style task management application demonstrating a full-stack .NET + Angular setup with real-time collaboration, clean architecture on the backend, and a signals-based reactive frontend.

## What it demonstrates

- Clean Architecture on the backend (Core / Infrastructure / API layered projects)
- ASP.NET Core Minimal APIs with EF Core code-first migrations against PostgreSQL
- Real-time updates across clients via SignalR
- Angular standalone components with Signals for state management, and CDK drag-and-drop
- Multi-environment Docker setup (dev/staging/prod) behind an Nginx reverse proxy
- CI pipeline that builds and tests both backend and frontend on every push/PR

## What's inside

- **Boards / Lists / Cards** domain model with position-based ordering (drag-and-drop reordering)
- **SignalR hub** (`/hubs/taskboard`) broadcasting board, list, and card change events to connected clients
- **Repository pattern** over EF Core with a shared generic repository plus a dedicated board repository
- **Health checks** (`/health`, `/api/health`) including a PostgreSQL connectivity check
- **Swagger/OpenAPI** documentation (development environment only)
- Automatic database migration and sample-data seeding on startup (development)

## Tech stack

**Backend:** .NET, ASP.NET Core Minimal APIs, Entity Framework Core, PostgreSQL, SignalR, Swashbuckle (Swagger)

**Frontend:** Angular (standalone components, Signals), Angular CDK (drag-and-drop), Microsoft SignalR client, RxJS, SCSS

**Infrastructure:** Docker multi-stage builds, Docker Compose (per-environment files), Nginx reverse proxy, GitHub Actions CI

## Quickstart

### Full stack with Docker

```bash
./scripts/start-full.sh
```

- Frontend: http://localhost:4200
- Backend API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger

Or directly:

```bash
docker compose -f docker/docker-compose.yml up --build
```

### Local development

Start PostgreSQL only:

```bash
./scripts/start-dev.sh
# or: docker compose -f docker/docker-compose.dev.yml up -d
```

Run the backend:

```bash
cd backend/src/TaskBoard.Api
dotnet restore
dotnet ef database update --project ../TaskBoard.Infrastructure/TaskBoard.Infrastructure.csproj
dotnet run
```

Run the frontend:

```bash
cd frontend
pnpm install
pnpm start
```

## Project structure

```
backend/
  src/
    TaskBoard.Api/             # Minimal API endpoints, SignalR hub, DTOs, Program.cs
    TaskBoard.Core/            # Domain entities and repository interfaces
    TaskBoard.Infrastructure/  # EF Core DbContext, configurations, migrations, repositories

frontend/
  src/app/
    components/                # board, boards-list (standalone components)
    services/                   # api.service.ts, signalr.service.ts, board.store.ts
    models/                     # TypeScript interfaces

docker/                # docker-compose files per environment + Nginx config
scripts/               # start-dev.sh, start-full.sh, stop-all.sh
```

## API endpoints

**Boards**
- `GET /api/boards` / `GET /api/boards/{id}` / `POST /api/boards` / `PUT /api/boards/{id}` / `DELETE /api/boards/{id}`

**Lists**
- `POST /api/lists` / `PUT /api/lists/{id}` / `DELETE /api/lists/{id}` / `PATCH /api/lists/{id}/move`

**Cards**
- `POST /api/cards` / `PUT /api/cards/{id}` / `DELETE /api/cards/{id}` / `PATCH /api/cards/{id}/move`

**SignalR hub:** `/hubs/taskboard` — emits `BoardCreated`, `BoardUpdated`, `ListCreated`, `ListUpdated`, `CardCreated`, `CardUpdated`, `CardMoved`

## License

MIT
