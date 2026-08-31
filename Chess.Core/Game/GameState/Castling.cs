using System.Collections.Immutable;
using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Game.GameState;

public readonly record struct CastlingInfo(
    char LetterId,
    Color Color,
    Move Castle,
    Position RookPosition,
    IEnumerable<Position> KingTravelPositions);


public class Castling
{
    public Castling()
    {
        CastlingPositions = new Dictionary<CastlingInfo, bool>();

        var kingSideBlack = new CastlingInfo(
            'k',
            Color.Black,
            new Move(
                new Position(0, 4), new Position(0, 6)),
            new Position(0, 7),
            [new Position(0, 4), 
                new Position(0, 5), 
                new Position(0, 6)]
            );
        
        var queenSideBlack = new CastlingInfo(
            'q',
            Color.Black,
            new Move(
                new Position(0, 4), new Position(0, 2)),
            new Position(0, 0),
            [new Position(0, 4), 
                new Position(0, 3), 
                new Position(0, 2)]
        );
        
        var kingSideWhite = new CastlingInfo(
            'K',
            Color.White,
            new Move(
                new Position(7, 4), new Position(7, 6)),
            new Position(7, 7),
            [new Position(7, 4), 
                new Position(7, 5), 
                new Position(7, 6)]
        );
        
        var queenSideWhite = new CastlingInfo(
            'Q',
            Color.White,
            new Move(
                new Position(7, 4), new Position(7, 2)),
            new Position(7, 0),
            [new Position(7, 4), 
                new Position(7, 3), 
                new Position(7, 2)]
        );
        
        CastlingPositions.Add(kingSideWhite, true);
        CastlingPositions.Add(queenSideWhite, true);
        CastlingPositions.Add(kingSideBlack, true);
        CastlingPositions.Add(queenSideBlack, true);

        CastlingPositions.ToImmutableDictionary();
    }

    public Dictionary<CastlingInfo, bool> CastlingPositions { get; }

    public bool CanCastle()
    {
        return true;
    }
}    