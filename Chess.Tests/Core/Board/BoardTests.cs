using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Entities.Pieces.Types;

namespace Chess.Tests.Core.Board;

public class BoardTests
{
    public static IEnumerable<object[]> PieceTypes =>
    [
        [typeof(Rook)],
        [typeof(Bishop)],
        [typeof(Knight)],
        [typeof(Queen)],
        [typeof(King)],
        [typeof(Pawn)]
    ];
    
    [Theory]
    [MemberData(nameof(PieceTypes))]
    public void PieceIdentification(Type pieceType)
    {
        var board = new chess.Entities.Board.Board();
        var pos = new Position(2, 2);
        var piece = (Piece)Activator.CreateInstance(pieceType, Color.White)!;
        
        board.CreateEmpty().PlacePiece(piece, pos);
        
        var pieceFromBoard = board.GetPiece(pos);
        
        Assert.Equal(piece, pieceFromBoard);
    }
}