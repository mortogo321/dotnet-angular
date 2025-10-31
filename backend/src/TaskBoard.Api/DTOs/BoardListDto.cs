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
    string Title,
    Guid BoardId,
    int Position
);

public record UpdateBoardListDto(
    string Title,
    int Position
);
