using Microsoft.EntityFrameworkCore;
using TaskBoard.Core.Entities;

namespace TaskBoard.Infrastructure.Data;

public class TaskBoardDbContext : DbContext
{
    public TaskBoardDbContext(DbContextOptions<TaskBoardDbContext> options) : base(options)
    {
    }

    public DbSet<Board> Boards => Set<Board>();
    public DbSet<BoardList> Lists => Set<BoardList>();
    public DbSet<Card> Cards => Set<Card>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskBoardDbContext).Assembly);

        // Global query filters and conventions
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Configure table names to be singular
            if (entityType.ClrType.Name == "BoardList")
            {
                entityType.SetTableName("Lists");
            }
            else
            {
                entityType.SetTableName(entityType.ClrType.Name + "s");
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Automatically set timestamps
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is Board board)
            {
                if (entry.State == EntityState.Added)
                {
                    board.CreatedAt = DateTime.UtcNow;
                }
                board.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is BoardList list)
            {
                if (entry.State == EntityState.Added)
                {
                    list.CreatedAt = DateTime.UtcNow;
                }
                list.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.Entity is Card card)
            {
                if (entry.State == EntityState.Added)
                {
                    card.CreatedAt = DateTime.UtcNow;
                }
                card.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
