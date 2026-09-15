using System.Net.WebSockets;
using chess.Game;
using chess.Moves;

namespace Chess.Api.Game;

public sealed class GameSession
{
    private readonly Lock _sync = new();
    private readonly GameRunner _runner;
    private WebSocket? _socket;
    private GameSnapshot? _latestSnapshot;
    private bool _gameStarted;
    
    public event Func<GameSnapshot,CancellationToken, Task>? SnapshotPublished;
    public event Func<MoveStatus, CancellationToken, Task>? MoveStatusReceived;

    public GameSession(GameRunner runner)
    {
        _runner = runner;
        
        // subscribe to http player events
        _runner.HttpPlayer.MoveStatusReceived += async moveStatus =>
        {
            if (moveStatus.MoveResult == MoveResult.Invalid)
            {
                await OnMoveStatusReceivedAsync(moveStatus, CancellationToken.None);
            }
        };
    }

    public async Task AttachAsync(WebSocket socket, CancellationToken ct)
    {
        GameSnapshot? latest;
        bool shouldStartGame;
        
        lock (_sync)
        {
            _socket = socket;
            latest = _latestSnapshot;
            shouldStartGame = !_gameStarted;
            _gameStarted = true;
        }

        // send latest snapshot if available
        if (latest is not null)
        {
            await OnSnapshotPublishedAsync(latest, ct);
        }
        
        // start game loop on first client attach
        if (shouldStartGame)
        {
            _ = RunGameLoopAsync(ct);
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

    private async Task RunGameLoopAsync(CancellationToken ct)
    {
        try
        {
            await foreach (var snapshot in _runner.Run(ct))
            {
                lock (_sync)
                {
                    _latestSnapshot = snapshot;
                }

                await OnSnapshotPublishedAsync(snapshot, ct);
            }
        }
        catch (OperationCanceledException)
        {
            // game was canceled
        }
    }
    
    private async Task OnSnapshotPublishedAsync(GameSnapshot snapshot, CancellationToken ct)
    {
        if (SnapshotPublished is not null)
        {
            await SnapshotPublished.Invoke(snapshot, ct);
        }
    }

    private async Task OnMoveStatusReceivedAsync(MoveStatus moveStatus, CancellationToken ct)
    {
        if (MoveStatusReceived is not null)
        {
            await MoveStatusReceived.Invoke(moveStatus, ct);
        }
    }

    public void ProvideMoveFromClient(Move move)
    {
        _runner.HttpPlayer.ProvideMoveFromClient(move);
    }
}