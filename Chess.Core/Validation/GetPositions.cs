using chess.Board;
using chess.Pieces;

namespace chess.Validation;

public static class GetPositions
{
    public static IEnumerable<Position> GetPossiblePositions(this Piece piece)
    {
        return new List<Position>();
    }
}