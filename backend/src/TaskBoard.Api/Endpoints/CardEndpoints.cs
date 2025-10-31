using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.DTOs;
using TaskBoard.Api.Hubs;
using TaskBoard.Core.Entities;
using TaskBoard.Core.Interfaces;
using TaskBoard.Infrastructure.Data;

namespace TaskBoard.Api.Endpoints;

public static class CardEndpoints
{
    public static void MapCardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cards").WithTags("Cards");

        group.MapPost("/", CreateCard)
            .WithName("CreateCard");

        group.MapPut("/{id:guid}", UpdateCard)
            .WithName("UpdateCard");

        group.MapDelete("/{id:guid}", DeleteCard)
            .WithName("DeleteCard");

        group.MapPatch("/{id:guid}/move", MoveCard)
            .WithName("MoveCard");
    }

    private static async Task<IResult> CreateCard(
        CreateCardDto dto,
        IRepository<Card> repository,
        TaskBoardDbContext context,
        IHubContext<TaskBoardHub> hubContext)
    {
        var list = await context.Lists.Include(l => l.Board).FirstOrDefaultAsync(l => l.Id == dto.ListId);
        if (list == null)
            return Results.NotFound("List not found");

        var card = new Card
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            ListId = dto.ListId,
            Position = dto.Position,
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            Status = dto.Status
        };

        await repository.AddAsync(card);
        await repository.SaveChangesAsync();

        var cardDto = new CardDto(
            card.Id,
            card.Title,
            card.Description,
            card.Position,
            card.ListId,
            card.CreatedAt,
            card.UpdatedAt,
            card.DueDate,
            card.Priority,
            card.Status
        );

        await hubContext.Clients.Group($"board-{list.BoardId}").SendAsync("CardCreated", cardDto);

        return Results.Created($"/api/cards/{card.Id}", cardDto);
    }

    private static async Task<IResult> UpdateCard(
        Guid id,
        UpdateCardDto dto,
        IRepository<Card> repository,
        TaskBoardDbContext context,
        IHubContext<TaskBoardHub> hubContext)
    {
        var card = await repository.GetByIdAsync(id);
        if (card == null)
            return Results.NotFound();

        var list = await context.Lists.Include(l => l.Board).FirstOrDefaultAsync(l => l.Id == card.ListId);
        if (list == null)
            return Results.NotFound("List not found");

        card.Title = dto.Title;
        card.Description = dto.Description;
        card.Position = dto.Position;
        card.DueDate = dto.DueDate;
        card.Priority = dto.Priority;
        card.Status = dto.Status;

        await repository.UpdateAsync(card);
        await repository.SaveChangesAsync();

        var cardDto = new CardDto(
            card.Id,
            card.Title,
            card.Description,
            card.Position,
            card.ListId,
            card.CreatedAt,
            card.UpdatedAt,
            card.DueDate,
            card.Priority,
            card.Status
        );

        await hubContext.Clients.Group($"board-{list.BoardId}").SendAsync("CardUpdated", cardDto);

        return Results.Ok(cardDto);
    }

    private static async Task<IResult> DeleteCard(
        Guid id,
        IRepository<Card> repository,
        TaskBoardDbContext context,
        IHubContext<TaskBoardHub> hubContext)
    {
        var card = await repository.GetByIdAsync(id);
        if (card == null)
            return Results.NotFound();

        var list = await context.Lists.Include(l => l.Board).FirstOrDefaultAsync(l => l.Id == card.ListId);
        if (list == null)
            return Results.NotFound("List not found");

        await repository.DeleteAsync(card);
        await repository.SaveChangesAsync();

        await hubContext.Clients.Group($"board-{list.BoardId}").SendAsync("CardDeleted", id);

        return Results.NoContent();
    }

    private static async Task<IResult> MoveCard(
        Guid id,
        MoveCardDto dto,
        IRepository<Card> repository,
        TaskBoardDbContext context,
        IHubContext<TaskBoardHub> hubContext)
    {
        var card = await repository.GetByIdAsync(id);
        if (card == null)
            return Results.NotFound();

        var oldList = await context.Lists.Include(l => l.Board).FirstOrDefaultAsync(l => l.Id == card.ListId);
        var newList = await context.Lists.Include(l => l.Board).FirstOrDefaultAsync(l => l.Id == dto.TargetListId);

        if (oldList == null || newList == null)
            return Results.NotFound("List not found");

        card.ListId = dto.TargetListId;
        card.Position = dto.NewPosition;

        await repository.UpdateAsync(card);
        await repository.SaveChangesAsync();

        await hubContext.Clients.Group($"board-{oldList.BoardId}")
            .SendAsync("CardMoved", new { cardId = id, targetListId = dto.TargetListId, newPosition = dto.NewPosition });

        return Results.Ok();
    }
}
