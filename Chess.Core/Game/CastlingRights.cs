using chess.Board;

namespace chess.Game;

public readonly record struct CastlingRights(
    char LetterId,
    Color Color,
    Position KingFrom,
    Position KingTo,
    Position RookFrom,
    Position RookTo,
    IEnumerable<Position> KingSafePositions,
    IEnumerable<Position> BetweenPositions)
{
    public override string ToString()
    {
        return LetterId.ToString();
    }

    public static IEnumerable<CastlingRights> GetInitialCastlingRights()
    {
        return new List<CastlingRights>
        {
            new(
                'K',
                Color.White,
                new Position(7, 4), new Position(7, 6),
                new Position(7, 7), new Position(7, 5),
                [new Position(7, 4), new Position(7, 5), new Position(7, 6)],
                [new Position(7, 5), new Position(7, 6)]
            ),
            new(
                'Q',
                Color.White,
                new Position(7, 4), new Position(7, 2),
                new Position(7, 0), new Position(7, 3),
                [new Position(7, 4), new Position(7, 3), new Position(7, 2)],
                [new Position(7, 1), new Position(7, 3), new Position(7, 2)]
            ),
            new(
                'k',
                Color.Black,
                new Position(0, 4), new Position(0, 6),
                new Position(0, 7), new Position(0, 5),
                [new Position(0, 4), new Position(0, 5), new Position(0, 6)],
                [new Position(0, 5), new Position(0, 6)]
            ),
            new(
                'q',
                Color.Black,
                new Position(0, 4), new Position(0, 2),
                new Position(0, 0), new Position(0, 3),
                [new Position(0, 4), new Position(0, 3), new Position(0, 2)],
                [new Position(0, 1), new Position(0, 3), new Position(0, 2)]
            )
        };
    }
}