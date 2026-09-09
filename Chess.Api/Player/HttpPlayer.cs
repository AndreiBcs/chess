using chess;
using chess.Game;
using chess.Moves;
using System.Threading.Channels;

namespace Chess.Api.Player;

public class HttpPlayer : chess.Player.Player
{
    private readonly Channel<Move> _moves = Channel.CreateUnbounded<Move>();

    public event Action<MoveResult>? MoveResultReceived;

    public HttpPlayer(Color color) : base(color)
    {
    }

    public override Task<Move> GetMoveAsync(GameSnapshot snapshot, MoveResult? previousResult)
    {
        if (previousResult is not null)
        {
            MoveResultReceived?.Invoke(previousResult.Value);
        }

        return _moves.Reader.ReadAsync().AsTask();
    }

    public void ProvideMoveFromClient(Move move)
    {
        _moves.Writer.TryWrite(move);
    }
}