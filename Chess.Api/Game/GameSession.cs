using chess.Game;
using chess.Moves;

namespace Chess.Api.Game;

public sealed class GameSession
{
    private readonly Lock _lock = new();
    private readonly GameRunner _runner;
    public string SessionId { get; }
    public CancellationTokenSource? GameCts { get; private set; }
    public event Func<GameSnapshot,CancellationToken, Task>? SnapshotPublished;
    public event Func<MoveStatus, CancellationToken, Task>? MoveStatusReceived;

    public GameSession(string id, GameRunner runner)
    {
        SessionId = id;
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

    private async Task RunGameLoopAsync(CancellationToken ct)
    {
        GameCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        try
        {
            await foreach (var snapshot in _runner.Run(GameCts.Token))
            {
                await OnSnapshotPublishedAsync(snapshot, GameCts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // game was canceled
        }
    }

    public void StopGame()
    {
        GameCts?.Cancel();
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
}