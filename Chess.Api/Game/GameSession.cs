using Chess.Api.Dtos.RequestDtos;
using chess.Game;
using chess.Moves;

namespace Chess.Api.Game;

public sealed class GameSession
{
    private readonly Lock _startLock = new();
    private readonly GameRunner _runner;
    private Task? _gameLoopTask;
    public string SessionId { get; }
    public GameSnapshot? LastSnapshot { get; private set; }
    private CancellationTokenSource? GameCts { get; set; }
    public event Func<GameSnapshot, CancellationToken, Task>? SnapshotPublished;
    public event Func<MoveStatus, CancellationToken, Task>? MoveStatusReceived;
    public event Func<string, CancellationToken, Task>? ErrorOccurred;

    public static GameSession Create(string id, StartRequestDto request)
    {
        StartRequestDto.Validate(request);
        return new GameSession(id, new GameRunner(request));
    }

    private GameSession(string id, GameRunner runner)
    {
        SessionId = id;
        _runner = runner;

        _runner.HttpPlayer.MoveStatusReceived += async moveStatus =>
        {
            if (moveStatus.MoveResult == MoveResult.Invalid)
            {
                await OnMoveStatusReceivedAsync(moveStatus, CancellationToken.None);
            }
        };
    }

    public Task StartAsync()
    {
        lock (_startLock)
        {
            if (_gameLoopTask is not null)
            {
                return _gameLoopTask;
            }

            GameCts = new CancellationTokenSource();
            _gameLoopTask = RunGameLoopAsync(GameCts.Token);
            return _gameLoopTask;
        }
    }

    private async Task RunGameLoopAsync(CancellationToken ct)
    {
        try
        {
            await foreach (var snapshot in _runner.Run(ct))
            {
                LastSnapshot = snapshot;
                await OnSnapshotPublishedAsync(snapshot, ct);

                if (snapshot.Status != GameStatus.InProgress)
                {
                    break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // game was canceled by the server lifecycle
        }
        catch (Exception ex)
        {
            await OnErrorOccurredAsync(ex.Message, ct);
        }
    }

    public void ProvideMoveFromClient(Move move)
    {
        _runner.HttpPlayer.ProvideMoveFromClient(move);
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

    private async Task OnErrorOccurredAsync(string message, CancellationToken ct)
    {
        if (ErrorOccurred is not null)
        {
            await ErrorOccurred.Invoke(message, ct);
        }
    }
    
    public void Cancel()
    {
        GameCts?.Cancel();
    }
}