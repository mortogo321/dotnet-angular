using Microsoft.EntityFrameworkCore;
using TaskBoard.Core.Entities;
using TaskBoard.Core.Interfaces;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.Infrastructure.Repositories;

public class BoardRepository : Repository<Board>
{
    public BoardRepository(TaskBoardDbContext context) : base(context)
    {
    }

    public override async Task<Board?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Lists.OrderBy(l => l.Position))
            .ThenInclude(l => l.Cards.OrderBy(c => c.Position))
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Board>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Lists.OrderBy(l => l.Position))
            .ThenInclude(l => l.Cards.OrderBy(c => c.Position))
            .OrderByDescending(b => b.UpdatedAt)
            .ToListAsync(cancellationToken);
    }
}
