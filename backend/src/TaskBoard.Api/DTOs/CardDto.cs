namespace TaskBoard.Api.DTOs;

public record CardDto(
    Guid Id,
    string Title,
    string? Description,
    int Position,
    Guid ListId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? DueDate,
    string? Priority,
    string? Status
);

public record CreateCardDto(
    string Title,
    string? Description,
    Guid ListId,
    int Position,
    DateTime? DueDate,
    string? Priority,
    string? Status
);

public record UpdateCardDto(
    string Title,
    string? Description,
    int Position,
    DateTime? DueDate,
    string? Priority,
    string? Status
);

public record MoveCardDto(
    Guid TargetListId,
    int NewPosition
);
