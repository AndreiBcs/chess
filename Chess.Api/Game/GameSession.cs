using System.Net.WebSockets;
using Chess.Api.Controllers;
using Chess.Api.Dtos.ResponseDtos;
using chess.Game;

namespace Chess.Api.Game;

public sealed class GameSession
{
    private readonly Lock _sync = new();
    private WebSocket? _socket;

    public GameRunner Runner { get; }
    private GameSnapshot? LatestSnapshot { get; set; }
    public Task? GameTask { get; set; }

    public GameSession(GameRunner runner)
    {
        Runner = runner;
        Runner.HttpPlayer.MoveStatusReceived += async moveStatus =>
        {
            if (moveStatus.MoveResult == chess.Moves.MoveResult.Invalid)
            {
                await SendAsync(
                    MoveStatusDto.ToMoveStatusDto(moveStatus),
                    CancellationToken.None);
            }
        };
    }

    public async Task AttachAsync(WebSocket socket, CancellationToken ct)
    {
        GameSnapshot? latest;
        lock (_sync)
        {
            _socket = socket;
            latest = LatestSnapshot;
        }

        if (latest is not null)
        {
            await SendAsync(SnapshotDto.ToSnapshotDto(latest), ct);
        }
    }

    public void Detach(WebSocket socket)
    {
        lock (_sync)
        {
            if (ReferenceEquals(_socket, socket))
            {
                _socket = null;
            }
        }
    }

    public async Task PublishAsync(GameSnapshot snapshot, CancellationToken ct)
    {
        lock (_sync)
        {
            LatestSnapshot = snapshot;
        }

        await SendAsync(SnapshotDto.ToSnapshotDto(snapshot), ct);
    }

    private async Task SendAsync(object message, CancellationToken ct)
    {
        WebSocket? socket;
        lock (_sync)
        {
            socket = _socket;
        }

        if (socket is null || socket.State != WebSocketState.Open)
        {
            return;
        }

        try
        {
            await GameController.SendMessageAsync(socket, message, ct);
        }
        catch (WebSocketException)
        {
            Detach(socket);
        }
    }
}