using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    string Title,

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    string? Description,

    [Required(ErrorMessage = "ListId is required")]
    Guid ListId,

    [Range(0, int.MaxValue, ErrorMessage = "Position must be non-negative")]
    int Position,

    DateTime? DueDate,

    [MaxLength(20, ErrorMessage = "Priority cannot exceed 20 characters")]
    string? Priority,

    [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    string? Status
);

public record UpdateCardDto(
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    string Title,

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    string? Description,

    [Range(0, int.MaxValue, ErrorMessage = "Position must be non-negative")]
    int Position,

    DateTime? DueDate,

    [MaxLength(20, ErrorMessage = "Priority cannot exceed 20 characters")]
    string? Priority,

    [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    string? Status
);

public record MoveCardDto(
    [Required(ErrorMessage = "TargetListId is required")]
    Guid TargetListId,

    [Range(0, int.MaxValue, ErrorMessage = "Position must be non-negative")]
    int NewPosition
);
