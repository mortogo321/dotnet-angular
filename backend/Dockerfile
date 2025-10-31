# Multi-stage build for .NET 9 API
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
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
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TaskBoard.Api.dll"]
