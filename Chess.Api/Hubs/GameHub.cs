using Chess.Api.Game;
using chess.Moves;
using Microsoft.AspNetCore.SignalR;

namespace Chess.Api.Hubs;

public class GameHub : Hub
{
    private readonly GameSessionManager _sessionManager;

    public GameHub(GameSessionManager sessionManager)
    {
        _sessionManager = sessionManager;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGame(string id)
    {
        var session = _sessionManager.GetSession(id);
        if (session is null)
        {
            await Clients.Caller.SendAsync("Error", "Game session not found");
            return;
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, id);
        await Clients.Group(id).SendAsync("PlayerJoined", Context.ConnectionId);
    }

    public async Task SendMove(string sessionId, Move move)
    {
        var session = _sessionManager.GetSession(sessionId);

        session?.ProvideMoveFromClient(move);
    }

    public async Task LeaveGame(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        await Clients.Group(sessionId).SendAsync("PlayerLeft", Context.ConnectionId);
    }
}