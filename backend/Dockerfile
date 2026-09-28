# Multi-stage build for .NET 10 API
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY src/TaskBoard.Core/TaskBoard.Core.csproj TaskBoard.Core/
COPY src/TaskBoard.Infrastructure/TaskBoard.Infrastructure.csproj TaskBoard.Infrastructure/
COPY src/TaskBoard.Api/TaskBoard.Api.csproj TaskBoard.Api/

RUN dotnet restore TaskBoard.Api/TaskBoard.Api.csproj

# Copy source files and build
COPY src/TaskBoard.Core/ TaskBoard.Core/
COPY src/TaskBoard.Infrastructure/ TaskBoard.Infrastructure/
COPY src/TaskBoard.Api/ TaskBoard.Api/

WORKDIR /src/TaskBoard.Api
RUN dotnet build -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN useradd -m appuser

COPY --from=publish --chown=appuser:appuser /app/publish .
USER appuser

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "TaskBoard.Api.dll"]
