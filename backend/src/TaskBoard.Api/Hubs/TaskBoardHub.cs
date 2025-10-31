using Microsoft.AspNetCore.SignalR;

namespace TaskBoard.Api.Hubs;

public class TaskBoardHub : Hub
{
    public async Task JoinBoard(string boardId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"board-{boardId}");
        await Clients.Group($"board-{boardId}").SendAsync("UserJoined", Context.ConnectionId);
    }

    public async Task LeaveBoard(string boardId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"board-{boardId}");
        await Clients.Group($"board-{boardId}").SendAsync("UserLeft", Context.ConnectionId);
    }

    public async Task BoardCreated(object board)
    {
        await Clients.All.SendAsync("BoardCreated", board);
    }

    public async Task BoardUpdated(string boardId, object board)
    {
        await Clients.Group($"board-{boardId}").SendAsync("BoardUpdated", board);
    }

    public async Task BoardDeleted(string boardId)
    {
        await Clients.Group($"board-{boardId}").SendAsync("BoardDeleted", boardId);
    }

    public async Task ListCreated(string boardId, object list)
    {
        await Clients.Group($"board-{boardId}").SendAsync("ListCreated", list);
    }

    public async Task ListUpdated(string boardId, object list)
    {
        await Clients.Group($"board-{boardId}").SendAsync("ListUpdated", list);
    }

    public async Task ListDeleted(string boardId, string listId)
    {
        await Clients.Group($"board-{boardId}").SendAsync("ListDeleted", listId);
    }

    public async Task ListMoved(string boardId, string listId, int newPosition)
    {
        await Clients.Group($"board-{boardId}").SendAsync("ListMoved", new { listId, newPosition });
    }

    public async Task CardCreated(string boardId, object card)
    {
        await Clients.Group($"board-{boardId}").SendAsync("CardCreated", card);
    }

    public async Task CardUpdated(string boardId, object card)
    {
        await Clients.Group($"board-{boardId}").SendAsync("CardUpdated", card);
    }

    public async Task CardDeleted(string boardId, string cardId)
    {
        await Clients.Group($"board-{boardId}").SendAsync("CardDeleted", cardId);
    }

    public async Task CardMoved(string boardId, string cardId, string targetListId, int newPosition)
    {
        await Clients.Group($"board-{boardId}").SendAsync("CardMoved", new { cardId, targetListId, newPosition });
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
    }
}
