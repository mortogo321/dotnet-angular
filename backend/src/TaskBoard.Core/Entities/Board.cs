namespace TaskBoard.Core.Entities;

public class Board
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<BoardList> Lists { get; set; } = new List<BoardList>();
}
