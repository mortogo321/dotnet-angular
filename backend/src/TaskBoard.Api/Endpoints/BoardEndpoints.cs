using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Hubs;
using TaskBoard.Core.Entities;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories;

namespace TaskBoard.Api.Endpoints;

public static class BoardEndpoints
{
    public static void MapBoardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/boards").WithTags("Boards");

        group.MapGet("/", GetAllBoards)
            .WithName("GetAllBoards");

        group.MapGet("/{id:guid}", GetBoardById)
            .WithName("GetBoardById");

        group.MapPost("/", CreateBoard)
            .WithName("CreateBoard");

        group.MapPut("/{id:guid}", UpdateBoard)
            .WithName("UpdateBoard");

        group.MapDelete("/{id:guid}", DeleteBoard)
            .WithName("DeleteBoard");
    }

    private static async Task<IResult> GetAllBoards(BoardRepository repository)
    {
        var boards = await repository.GetAllAsync();
        var boardDtos = boards.Select(b => new BoardDto(
            b.Id,
            b.Title,
            b.Description,
            b.CreatedAt,
            b.UpdatedAt,
            b.Lists.Select(l => new BoardListDto(
                l.Id,
                l.Title,
                l.Position,
                l.BoardId,
                l.CreatedAt,
                l.UpdatedAt,
                l.Cards.Select(c => new CardDto(
                    c.Id,
                    c.Title,
                    c.Description,
                    c.Position,
                    c.ListId,
                    c.CreatedAt,
                    c.UpdatedAt,
                    c.DueDate,
                    c.Priority,
                    c.Status
                )).ToList()
            )).ToList()
        )).ToList();

        return Results.Ok(boardDtos);
    }

    private static async Task<IResult> GetBoardById(Guid id, BoardRepository repository)
    {
        var board = await repository.GetByIdAsync(id);
        if (board == null)
            return Results.NotFound();

        var boardDto = new BoardDto(
            board.Id,
            board.Title,
            board.Description,
            board.CreatedAt,
            board.UpdatedAt,
            board.Lists.Select(l => new BoardListDto(
                l.Id,
                l.Title,
                l.Position,
                l.BoardId,
                l.CreatedAt,
                l.UpdatedAt,
                l.Cards.Select(c => new CardDto(
                    c.Id,
                    c.Title,
                    c.Description,
                    c.Position,
                    c.ListId,
                    c.CreatedAt,
                    c.UpdatedAt,
                    c.DueDate,
                    c.Priority,
                    c.Status
                )).ToList()
            )).ToList()
        );

        return Results.Ok(boardDto);
    }

    private static async Task<IResult> CreateBoard(
        CreateBoardDto dto,
        BoardRepository repository,
        IHubContext<TaskBoardHub> hubContext)
    {
        var board = new Board
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description
        };

        await repository.AddAsync(board);
        await repository.SaveChangesAsync();

        var boardDto = new BoardDto(
            board.Id,
            board.Title,
            board.Description,
            board.CreatedAt,
            board.UpdatedAt,
            new List<BoardListDto>()
        );

        await hubContext.Clients.All.SendAsync("BoardCreated", boardDto);

        return Results.Created($"/api/boards/{board.Id}", boardDto);
    }

    private static async Task<IResult> UpdateBoard(
        Guid id,
        UpdateBoardDto dto,
        BoardRepository repository,
        IHubContext<TaskBoardHub> hubContext)
    {
        var board = await repository.GetByIdAsync(id);
        if (board == null)
            return Results.NotFound();

        board.Title = dto.Title;
        board.Description = dto.Description;

        await repository.UpdateAsync(board);
        await repository.SaveChangesAsync();

        var boardDto = new BoardDto(
            board.Id,
            board.Title,
            board.Description,
            board.CreatedAt,
            board.UpdatedAt,
            board.Lists.Select(l => new BoardListDto(
                l.Id,
                l.Title,
                l.Position,
                l.BoardId,
                l.CreatedAt,
                l.UpdatedAt,
                l.Cards.Select(c => new CardDto(
                    c.Id,
                    c.Title,
                    c.Description,
                    c.Position,
                    c.ListId,
                    c.CreatedAt,
                    c.UpdatedAt,
                    c.DueDate,
                    c.Priority,
                    c.Status
                )).ToList()
            )).ToList()
        );

        await hubContext.Clients.Group($"board-{id}").SendAsync("BoardUpdated", boardDto);

        return Results.Ok(boardDto);
    }

    private static async Task<IResult> DeleteBoard(
        Guid id,
        BoardRepository repository,
        IHubContext<TaskBoardHub> hubContext)
    {
        var board = await repository.GetByIdAsync(id);
        if (board == null)
            return Results.NotFound();

        await repository.DeleteAsync(board);
        await repository.SaveChangesAsync();

        await hubContext.Clients.Group($"board-{id}").SendAsync("BoardDeleted", id);

        return Results.NoContent();
    }
}
