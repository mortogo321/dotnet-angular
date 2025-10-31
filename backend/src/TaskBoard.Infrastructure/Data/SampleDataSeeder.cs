using TaskBoard.Core.Entities;

namespace TaskBoard.Infrastructure.Data;

public static class SampleDataSeeder
{
    public static async Task SeedAsync(TaskBoardDbContext context)
    {
        // Check if data already exists
        if (context.Boards.Any())
        {
            return;
        }

        // Create sample board
        var board = new Board
        {
            Id = Guid.NewGuid(),
            Title = "🚀 Product Launch Project",
            Description = "Planning and execution for Q1 2025 product launch",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create lists
        var todoList = new BoardList
        {
            Id = Guid.NewGuid(),
            Title = "📋 To Do",
            Position = 0,
            BoardId = board.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var inProgressList = new BoardList
        {
            Id = Guid.NewGuid(),
            Title = "🔄 In Progress",
            Position = 1,
            BoardId = board.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var doneList = new BoardList
        {
            Id = Guid.NewGuid(),
            Title = "✅ Done",
            Position = 2,
            BoardId = board.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create sample cards
        var cards = new List<Card>
        {
            new Card
            {
                Id = Guid.NewGuid(),
                Title = "Design new landing page",
                Description = "Create mockups and wireframes for the new product landing page",
                Position = 0,
                ListId = todoList.Id,
                Priority = "High",
                Status = "Todo",
                DueDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Card
            {
                Id = Guid.NewGuid(),
                Title = "Setup marketing campaign",
                Description = "Configure email marketing and social media campaigns",
                Position = 1,
                ListId = todoList.Id,
                Priority = "Medium",
                Status = "Todo",
                DueDate = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Card
            {
                Id = Guid.NewGuid(),
                Title = "Implement authentication",
                Description = "Add JWT authentication and authorization to the API",
                Position = 0,
                ListId = inProgressList.Id,
                Priority = "High",
                Status = "InProgress",
                DueDate = DateTime.UtcNow.AddDays(3),
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow
            },
            new Card
            {
                Id = Guid.NewGuid(),
                Title = "Write API documentation",
                Description = "Document all API endpoints with Swagger/OpenAPI",
                Position = 1,
                ListId = inProgressList.Id,
                Priority = "Medium",
                Status = "InProgress",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            },
            new Card
            {
                Id = Guid.NewGuid(),
                Title = "Setup Docker environment",
                Description = "Create Docker containers for dev, staging, and production",
                Position = 0,
                ListId = doneList.Id,
                Priority = "High",
                Status = "Done",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Card
            {
                Id = Guid.NewGuid(),
                Title = "Configure CI/CD pipeline",
                Description = "Setup GitHub Actions for automated testing and deployment",
                Position = 1,
                ListId = doneList.Id,
                Priority = "Medium",
                Status = "Done",
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        // Add to context
        context.Boards.Add(board);
        context.Lists.AddRange(new[] { todoList, inProgressList, doneList });
        context.Cards.AddRange(cards);

        await context.SaveChangesAsync();

        Console.WriteLine("✅ Sample data seeded successfully!");
    }
}
