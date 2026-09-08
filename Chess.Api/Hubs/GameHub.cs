using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Chess.Api.Dtos;
using Chess.Api.Game;
using chess.Game;

namespace Chess.Api.Hubs;

public class GameHub
{
    private readonly ConcurrentDictionary<string, List<WebSocket>> _connection = new();
    private readonly ConcurrentDictionary<string, GameRunner> _games = new();

    public async Task HandleConnectionAsync(
        string gameId,
        WebSocket socket,
        CancellationToken ct)
    {
        var sockets = _connection.GetOrAdd(gameId, _ => new List<WebSocket>());
        lock (sockets) 
        {
            sockets.Add(socket);
        }

        var buffer = new byte[4096];
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, ct);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }

                // parse received moves
                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var moveDto = JsonSerializer.Deserialize<MoveDto>(json);

                if (moveDto.Type == DtoType.Move &&
                    _games.TryGetValue(gameId, out var game))
                {
                    var move = MoveDto.FromMoveDto(moveDto);
                    game.HttpPlayer.ProvideMoveFromClient(move);
                }
            }
        }
        finally
        {
            lock (sockets)
            {
                sockets.Remove(socket);
            }
        }
    }

    public async Task BroadcastGameSnapshotAsync(
        string gameId,
        GameSnapshot snapshot,
        CancellationToken ct = default)
    {
        if (!_connection.TryGetValue(gameId, out var sockets))
        {
            return;
        }

        var dto = SnapshotDto.ToSnapshotDto(snapshot);
        var json = JsonSerializer.Serialize(dto);
        var bytes = Encoding.UTF8.GetBytes(json);
        
        List<WebSocket> clients;
        lock (sockets)
        {
            clients = sockets
                .Where(s => s.State == WebSocketState.Open)
                .ToList();
        }

        foreach (var client in clients)
        {
            try
            {
                await client.SendAsync(
                    bytes,
                    WebSocketMessageType.Text, 
                    true,
                    ct);    
            }
            catch
            {
                // connection lost
            }
        }
    }
    
    public void RegisterGame(string gameId, GameRunner game)
    {
        _games.TryAdd(gameId, game);        
    }

    public GameRunner? GetGame(string gameId)
    {
        _games.TryGetValue(gameId, out var game);
        return game;
    }
}