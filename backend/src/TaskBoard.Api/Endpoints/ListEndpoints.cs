using Microsoft.AspNetCore.SignalR;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Hubs;
using TaskBoard.Core.Entities;
using TaskBoard.Core.Interfaces;
using TaskBoard.Infrastructure.Repositories;

namespace TaskBoard.Api.Endpoints;

public static class ListEndpoints
{
    public static void MapListEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/lists").WithTags("Lists");

        group.MapPost("/", CreateList)
            .WithName("CreateList");

        group.MapPut("/{id:guid}", UpdateList)
            .WithName("UpdateList");

        group.MapDelete("/{id:guid}", DeleteList)
            .WithName("DeleteList");

        group.MapPatch("/{id:guid}/move", MoveList)
            .WithName("MoveList");
    }

    private static async Task<IResult> CreateList(
        CreateBoardListDto dto,
        IRepository<BoardList> repository,
        BoardRepository boardRepository,
        IHubContext<TaskBoardHub> hubContext)
    {
        var list = new BoardList
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            BoardId = dto.BoardId,
            Position = dto.Position
        };

        await repository.AddAsync(list);
        await repository.SaveChangesAsync();

        var listDto = new BoardListDto(
            list.Id,
            list.Title,
            list.Position,
            list.BoardId,
            list.CreatedAt,
            list.UpdatedAt,
            new List<CardDto>()
        );

        await hubContext.Clients.Group($"board-{dto.BoardId}").SendAsync("ListCreated", listDto);

        return Results.Created($"/api/lists/{list.Id}", listDto);
    }

    private static async Task<IResult> UpdateList(
        Guid id,
        UpdateBoardListDto dto,
        IRepository<BoardList> repository,
        IHubContext<TaskBoardHub> hubContext)
    {
        var list = await repository.GetByIdAsync(id);
        if (list == null)
            return Results.NotFound();

        list.Title = dto.Title;
        list.Position = dto.Position;

        await repository.UpdateAsync(list);
        await repository.SaveChangesAsync();

        var listDto = new BoardListDto(
            list.Id,
            list.Title,
            list.Position,
            list.BoardId,
            list.CreatedAt,
            list.UpdatedAt,
            new List<CardDto>()
        );

        await hubContext.Clients.Group($"board-{list.BoardId}").SendAsync("ListUpdated", listDto);

        return Results.Ok(listDto);
    }

    private static async Task<IResult> DeleteList(
        Guid id,
        IRepository<BoardList> repository,
        IHubContext<TaskBoardHub> hubContext)
    {
        var list = await repository.GetByIdAsync(id);
        if (list == null)
            return Results.NotFound();

        var boardId = list.BoardId;

        await repository.DeleteAsync(list);
        await repository.SaveChangesAsync();

        await hubContext.Clients.Group($"board-{boardId}").SendAsync("ListDeleted", id);

        return Results.NoContent();
    }

    private static async Task<IResult> MoveList(
        Guid id,
        int newPosition,
        IRepository<BoardList> repository,
        IHubContext<TaskBoardHub> hubContext)
    {
        var list = await repository.GetByIdAsync(id);
        if (list == null)
            return Results.NotFound();

        list.Position = newPosition;

        await repository.UpdateAsync(list);
        await repository.SaveChangesAsync();

        await hubContext.Clients.Group($"board-{list.BoardId}")
            .SendAsync("ListMoved", new { listId = id, newPosition });

        return Results.Ok();
    }
}
