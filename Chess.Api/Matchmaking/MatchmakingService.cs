using chess;
using Chess.Api.Dtos.ResponseDtos;
using Chess.Api.Game;
using Chess.Api.Hubs;
using Chess.Api.Player;
using Microsoft.AspNetCore.SignalR;

namespace Chess.Api.Matchmaking;

public sealed class MatchmakingService
{
    private readonly Lock _lock = new();
    private readonly MatchmakingQueue _queue = new();
    private readonly GameSessionManager _sessionManager;
    private readonly IHubContext<GameHub> _hubContext;

    public MatchmakingService(GameSessionManager sessionManager, IHubContext<GameHub> hubContext)
    {
        _sessionManager = sessionManager;
        _hubContext = hubContext;
    }

    public async Task<MatchResult> Enqueue(string connectionId, string playerId)
    {
        // pair the caller with the waiting user, assign random colors, and start a session
        MatchResult result;
        string sessionId;
        Color firstColor;
        Color secondColor;
        GameSession session;

        lock (_lock)
        {
            // enqueue or pair connections
            result = _queue.Enqueue(connectionId, playerId);

            // if no paired connections just keep waiting
            if (!result.Matched || result.First is null || result.Second is null)
            {
                return result;
            }

            sessionId = Guid.NewGuid().ToString("N");
            
            // assign colors and create players 
            firstColor = Random.Shared.Next(2) == 0 ? Color.White : Color.Black;
            secondColor = firstColor == Color.White ? Color.Black : Color.White;
            var firstPlayer = new HttpPlayer(firstColor);
            var secondPlayer = new HttpPlayer(secondColor);

            session = _sessionManager.CreateMultiplayerSession(
                sessionId,
                firstColor == Color.White ? firstPlayer : secondPlayer,
                firstColor == Color.White ? secondPlayer : firstPlayer);

            // register the connections to the manager and to a session
            _sessionManager.RegisterConnection(result.First.Value.ConnectionId, sessionId);
            _sessionManager.RegisterConnection(result.Second.Value.ConnectionId, sessionId);

            session.RegisterConnection(result.First.Value.ConnectionId, firstColor);
            session.RegisterConnection(result.Second.Value.ConnectionId, secondColor);
        }

        await _hubContext.Groups.AddToGroupAsync(result.First!.Value.ConnectionId, sessionId);
        await _hubContext.Groups.AddToGroupAsync(result.Second!.Value.ConnectionId, sessionId);

        // start the game
        _ = session.StartAsync();
        
        // send the session ID and assigned color to the connections 
        var firstResult = result.ForPlayer(sessionId, result.First.Value.ConnectionId, firstColor);
        var secondResult = result.ForPlayer(sessionId, result.Second.Value.ConnectionId, secondColor);
        
        await _hubContext.Clients.Client(result.First.Value.ConnectionId)
                .SendAsync("MatchFound", ToResponse(firstResult));
        await _hubContext.Clients.Client(result.Second.Value.ConnectionId)
                .SendAsync("MatchFound", ToResponse(secondResult));

        return result.ForPlayer(sessionId, connectionId, connectionId == result.First.Value.ConnectionId
            ? firstColor
            : secondColor);
    }

    public bool Cancel(string connectionId)
    {
        // remove only a waiting connection; active sessions are managed separately
        lock (_lock)
        {
            return _queue.TryCancel(connectionId);
        }
    }

    public static MatchmakingResponseDto ToResponse(MatchResult result) =>
        new(result.Matched, result.SessionId, result.PlayerColor);
}