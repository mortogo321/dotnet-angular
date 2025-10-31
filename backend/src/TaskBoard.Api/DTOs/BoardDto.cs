using System.ComponentModel.DataAnnotations;

namespace TaskBoard.Api.DTOs;

public record BoardDto(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<BoardListDto> Lists
);

public record CreateBoardDto(
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    string Title,

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    string Description
);

public record UpdateBoardDto(
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    string Title,

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    string Description
);
