using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using chess.Game;

namespace Chess.Api.Hubs;

public class GameHub
{
    private readonly ConcurrentDictionary<string, List<WebSocket>> _connection = new();

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

                // TODO handle moves
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

    public async Task BroadcastGameStateAsync(
        string gameId,
        GameSnapshot snapshot)
    {
        if (!_connection.TryGetValue(gameId, out var sockets))
        {
            return;
        }

        var json = JsonSerializer.Serialize(snapshot);
        var bytes = Encoding.UTF8.GetBytes(json);
        
        List<WebSocket> clients;
        lock (sockets)
        {
            clients = sockets.ToList();
        }

        foreach (var client in clients.Where(s => s.State == WebSocketState.Open))
        {
            await client.SendAsync(
                bytes,
                WebSocketMessageType.Text, 
                true,
                CancellationToken.None);
        }
    }
}