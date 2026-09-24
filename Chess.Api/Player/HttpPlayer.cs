using chess;
using chess.Game;
using chess.Moves;
using System.Threading.Channels;

namespace Chess.Api.Player;

public class HttpPlayer : chess.Player.Player
{
    private readonly Channel<Move> _moves = Channel.CreateUnbounded<Move>();

    public event Action<MoveStatus>? MoveStatusReceived;

    public HttpPlayer(Color color) : base(color)
    {
    }

    public override Task<Move> GetMoveAsync(
        GameSnapshot snapshot,
        MoveStatus? moveStatus,
        CancellationToken cancellationToken = default)
    {
        // wait asynchronously for the client move, or stop when the session is cancelled
        if (moveStatus is not null)
        {
            MoveStatusReceived?.Invoke(moveStatus.Value);
        }

        return _moves.Reader.ReadAsync(cancellationToken).AsTask();
    }

    public void ProvideMoveFromClient(Move move)
    {
        // queue the move so the game loop can validate it
        _moves.Writer.TryWrite(move);
    }
}