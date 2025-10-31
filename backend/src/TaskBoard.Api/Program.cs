using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Endpoints;
using TaskBoard.Api.Hubs;
using TaskBoard.Api.Middleware;
using TaskBoard.Core.Entities;
using TaskBoard.Core.Interfaces;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["http://localhost:4200"])
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TaskBoardDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString ?? throw new InvalidOperationException("Connection string not found"));

// Configure SignalR
builder.Services.AddSignalR();

// Register repositories
builder.Services.AddScoped<BoardRepository>();
builder.Services.AddScoped<IRepository<BoardList>, Repository<BoardList>>();
builder.Services.AddScoped<IRepository<Card>, Repository<Card>>();

var app = builder.Build();

// Configure the HTTP request pipeline
// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// Map endpoints
app.MapBoardEndpoints();
app.MapListEndpoints();
app.MapCardEndpoints();

// Map SignalR hub
app.MapHub<TaskBoardHub>("/hubs/taskboard");

app.MapGet("/", () => Results.Ok(new { message = "TaskBoard API is running", version = "1.0" }));

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/api/health");

// Auto-migrate database and seed sample data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskBoardDbContext>();
    await dbContext.Database.MigrateAsync();

    // Seed sample data in development
    if (app.Environment.IsDevelopment())
    {
        await SampleDataSeeder.SeedAsync(dbContext);
    }
}

app.Run();
