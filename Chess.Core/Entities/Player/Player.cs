using chess.Entities.Board;
using chess.Entities.Common;
using chess.Game.GameState;

namespace chess.Entities.Player;

public abstract class Player
{
    public Color Color { get; init; }
    public abstract Task<Move> GetMoveAsync(GameSnapshot snapshot);
}