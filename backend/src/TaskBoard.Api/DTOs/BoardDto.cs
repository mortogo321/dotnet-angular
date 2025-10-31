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
    string Title,
    string Description
);

public record UpdateBoardDto(
    string Title,
    string Description
);
