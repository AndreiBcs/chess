using chess.Entities.Board;
using chess.Entities.Common;
using chess.Game.GameState;

namespace chess.Entities.Player;

public abstract class Player
{
    protected Player(Color color)
    {
        Color = color;
    }

    public Color Color { get; }
    public abstract Task<Move> GetMoveAsync(GameSnapshot snapshot);
}