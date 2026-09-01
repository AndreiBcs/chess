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
    public abstract Task<Move> GetMoveAsync(GameSnapshot snapshot, MoveResult? previousResult);
}