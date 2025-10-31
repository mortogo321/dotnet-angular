using System.ComponentModel.DataAnnotations;

namespace TaskBoard.Api.DTOs;

public record BoardListDto(
    Guid Id,
    string Title,
    int Position,
    Guid BoardId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<CardDto> Cards
);

public record CreateBoardListDto(
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    string Title,

    [Required(ErrorMessage = "BoardId is required")]
    Guid BoardId,

    [Range(0, int.MaxValue, ErrorMessage = "Position must be non-negative")]
    int Position
);

public record UpdateBoardListDto(
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    string Title,

    [Range(0, int.MaxValue, ErrorMessage = "Position must be non-negative")]
    int Position
);
