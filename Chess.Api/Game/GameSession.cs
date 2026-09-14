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
                await SendResponseAsync(
                    MoveStatusDto.ToMoveStatusDto(moveStatus),
                    ResponseType.MoveStatus,
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
            await SendResponseAsync(SnapshotDto.ToSnapshotDto(latest), ResponseType.Snapshot, ct);
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

        await SendResponseAsync(SnapshotDto.ToSnapshotDto(snapshot), ResponseType.Snapshot, ct);
    }

    private async Task SendResponseAsync(
        object message,
        ResponseType responseType,
        CancellationToken ct)
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
            await GameController.SendMessageAsync(socket, message, responseType, ct);
        }
        catch (WebSocketException)
        {
            Detach(socket);
        }
    }
}