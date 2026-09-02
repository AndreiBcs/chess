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


// public readonly record struct CastlingRights
// {
//     public CastlingRights()
//     {
//         CastlingPositions = new List<CastlingInfo>();
//
//         var kingSideBlack = new CastlingInfo(
//             'k',
//             Color.Black,
//             new Position(0, 4), new Position(0, 6),
//             new Position(0, 7), new Position(0, 5),
//             [new Position(0, 4), new Position(0, 5), new Position(0, 6)]
//         );
//
//         var queenSideBlack = new CastlingInfo(
//             'q',
//             Color.Black,
//             new Position(0, 4), new Position(0, 2),
//             new Position(0, 0), new Position(0, 3),
//             [new Position(0, 4), new Position(0, 3), new Position(0, 2)]
//         );
//
//         var kingSideWhite = new CastlingInfo(
//             'K',
//             Color.White,
//             new Position(7, 4), new Position(7, 6),
//             new Position(7, 7), new Position(7, 5),
//             [new Position(7, 4), new Position(7, 5), new Position(7, 6)]
//         );
//
//         var queenSideWhite = new CastlingInfo(
//             'Q',
//             Color.White,
//             new Position(7, 4), new Position(7, 2),
//             new Position(7, 0), new Position(7, 3),
//             [new Position(7, 4), new Position(7, 3), new Position(7, 2)]
//         );
//
//         CastlingPositions.Add(kingSideWhite);
//         CastlingPositions.Add(queenSideWhite);
//         CastlingPositions.Add(kingSideBlack);
//         CastlingPositions.Add(queenSideBlack);
//     }
//
//     public List<CastlingInfo> CastlingPositions { get; }
//
//     public override string ToString()
//     {
//         var text = "";
//         
//         foreach (var castling in CastlingPositions)
//             text += castling.LetterId;
//         
//         return text.Length == 0 ? "-" : text;
//     }
// }