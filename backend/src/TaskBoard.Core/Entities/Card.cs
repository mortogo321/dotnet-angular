namespace TaskBoard.Core.Entities;

public class Card
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Position { get; set; }
    public Guid ListId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; } // Low, Medium, High
    public string? Status { get; set; } // Todo, InProgress, Done

    // Navigation properties
    public BoardList List { get; set; } = null!;
}
