using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Endpoints;
using TaskBoard.Api.Hubs;
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

// Configure SignalR
builder.Services.AddSignalR();

// Register repositories
builder.Services.AddScoped<BoardRepository>();
builder.Services.AddScoped<IRepository<BoardList>, Repository<BoardList>>();
builder.Services.AddScoped<IRepository<Card>, Repository<Card>>();

var app = builder.Build();

// Configure the HTTP request pipeline
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

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskBoardDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();
