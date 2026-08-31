using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Game.GameState;

public enum CastleSide
{
    KingSide,
    QueenSide
}

public readonly record struct CastlingInfo(
    char LetterId,
    Color Color,
    CastleSide Side,
    Position KingFrom,
    Position KingTo,
    Position RookFrom,
    Position RookTo,
    IEnumerable<Position> KingSafePositions);


public class Castling
{
    public Castling()
    {
        CastlingPositions = new List<CastlingInfo>();

        var kingSideBlack = new CastlingInfo(
            'k',
            Color.Black,
            CastleSide.KingSide,
            new Position(0, 4), new Position(0, 6),
            new Position(0, 7), new Position(0, 5),
            [new Position(0, 4), new Position(0, 5), new Position(0, 6)]
        );

        var queenSideBlack = new CastlingInfo(
            'q',
            Color.Black,
            CastleSide.QueenSide,
            new Position(0, 4), new Position(0, 2),
            new Position(0, 0), new Position(0, 3),
            [new Position(0, 4), new Position(0, 3), new Position(0, 2)]
        );

        var kingSideWhite = new CastlingInfo(
            'K',
            Color.White,
            CastleSide.KingSide,
            new Position(7, 4), new Position(7, 6),
            new Position(7, 7), new Position(7, 5),
            [new Position(7, 4), new Position(7, 5), new Position(7, 6)]
        );

        var queenSideWhite = new CastlingInfo(
            'Q',
            Color.White,
            CastleSide.QueenSide,
            new Position(7, 4), new Position(7, 2),
            new Position(7, 0), new Position(7, 3),
            [new Position(7, 4), new Position(7, 3), new Position(7, 2)]
        );

        CastlingPositions.Add(kingSideWhite);
        CastlingPositions.Add(queenSideWhite);
        CastlingPositions.Add(kingSideBlack);
        CastlingPositions.Add(queenSideBlack);
    }

    public List<CastlingInfo> CastlingPositions { get; }

    public override string ToString()
    {
        return "";
    }
}