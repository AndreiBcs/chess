using chess.Board;

namespace chess.Game;

public readonly record struct CastlingRights(
    char LetterId,
    Color Color,
    Position KingFrom,
    Position KingTo,
    Position RookFrom,
    Position RookTo,
    IEnumerable<Position> KingSafePositions)
{
    public override string ToString()
    {
        return LetterId.ToString();
    }
}