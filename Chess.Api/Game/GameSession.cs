using System.Collections.Concurrent;
using chess;
using Chess.Api.Hubs;
using Chess.Api.Messages.Dtos.ServerDtos;
using Chess.Api.Player;
using Chess.Engine;
using chess.Game;
using chess.Moves;
using Microsoft.AspNetCore.SignalR;

namespace Chess.Api.Game;

public sealed partial class GameSession
{
    private readonly chess.Game.Game _game;
    private readonly Dictionary<Color, HttpPlayer> _clients;
    private readonly ConcurrentDictionary<string, Color> _connections = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly IHubContext<GameHub> _hub;
    private readonly Action<string> _onFinished;
    private Task? _loop;

    private string SessionId { get; }
    public GameSnapshot? LastSnapshot { get; private set; }
    public bool HasConnections => !_connections.IsEmpty;
    public bool HasConnection(string connId) => _connections.ContainsKey(connId);

    private GameSession(
        string sessionId, 
        chess.Game.Game game,
        Dictionary<Color, HttpPlayer> clients,
        EnginePlayer? engine,
        int? elo,
        IHubContext<GameHub> hub,
        Action<string> onFinished)
    {
        SessionId = sessionId; 
        _game = game;
        _clients = clients;
        _engine = engine;
        _elo = elo;
        _hub = hub;
        _onFinished = onFinished;

        foreach (var (color, player) in clients)
        {
            player.MoveStatusReceived += status =>
            {
                if (status.MoveResult == MoveResult.Invalid)
                    _ = NotifyRejectedMoveAsync(color, status);
            };
        }
    }

    public void Join(string connId, Color color)
    {
        if (!_clients.ContainsKey(color))
        {
            throw new InvalidOperationException("Invalid color.");
        }
        
        _connections[connId] = color;
    }

    public void Leave(string connId) => _connections.TryRemove(connId, out _);

    public void SubmitMove(string connId, Move move)
    {
        if (LastSnapshot is { Status: not GameStatus.InProgress })
            throw new InvalidOperationException("The game is already over.");
        
        if (!_connections.TryGetValue(connId, out var color))
            throw new InvalidOperationException("Not a player in this game.");
        
        _clients[color].ProvideMoveFromClient(move);
    }

    public void Start()
    {
        lock (new Lock())
        {
            _loop ??= RunAsync();
        }
    }

    private async Task RunAsync()
    {
        var ct = _cts.Token;
        try
        {
            if (_engine is not null)
            {
                await _engine.Uci.StartEngine();
                await _engine.Uci.SetElo(_elo!.Value);
                await _engine.Uci.NewGame();
            }

            await foreach (var snap in _game.GameLoop(ct))
            {
                LastSnapshot = snap;
                await _hub.Clients
                    .Group(SessionId)
                    .SendAsync("Snapshot", GameSnapshotDto.ToSnapshotDto(snap), ct);
                
                if (snap.Status != GameStatus.InProgress)
                    break;
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            await _hub.Clients
                .Group(SessionId)
                .SendAsync("Error", new ErrorDto(ex.Message), cancellationToken: ct);
        }
        finally
        {
            _onFinished(SessionId);
            if (_engine is not null)
                await _engine.DisposeAsync();
        }
    }

    public void Cancel() => _cts.Cancel();

    private async Task NotifyRejectedMoveAsync(Color color, MoveStatus status)
    {
        try
        {
            var conns = _connections
                .Where(c => c.Value == color)
                .Select(c => c.Key)
                .ToList();
            
            await _hub.Clients
                .Clients(conns)
                .SendAsync(
                    "MoveRejected",
                    new MoveRejectedDto(status.InvalidMoveReason ?? "Invalid move."));
        }
        catch { /* client gone; nothing to do */ }
    }
}