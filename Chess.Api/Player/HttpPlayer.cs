using chess;
using chess.Game;
using chess.Moves;

namespace Chess.Api.Player;

public class HttpPlayer : chess.Player.Player
{
    private TaskCompletionSource<Move> _moveSource = new();
    public HttpPlayer(Color color) : base(color)
    {
    }

    public override Task<Move> GetMoveAsync(GameSnapshot snapshot, MoveResult? previousResult)
    {
        return _moveSource.Task;
    }

    public void ProvideMoveFromClient(Move move)
    {
        if (!_moveSource.Task.IsCompleted)
        {
            _moveSource.SetResult(move);
        }
    }

    public void ResetForNextMove()
    {
        _moveSource = new TaskCompletionSource<Move>();
    }
}