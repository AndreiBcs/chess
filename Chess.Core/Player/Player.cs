using chess.Game;
using chess.Moves;

namespace chess.Player;

public abstract class Player
{
    protected Player(Color color)
    {
        Color = color;
    }

    public Color Color { get; }
    
    // game passes the last move status only if the move was invalid
    public abstract Task<Move> GetMoveAsync(
        GameSnapshot snapshot,
        MoveStatus? moveStatus,
        CancellationToken cancellationToken = default);
}