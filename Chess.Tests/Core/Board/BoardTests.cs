using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Entities.Pieces.Types;
using chess.Game.GameState;
using chess.Game.Validators;

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
    
    [Fact]
    public void PieceCanCaptureEnemyPiece()
    {
        var board = new chess.Entities.Board.Board();
        var pos1 = new Position(7, 0);
        var pos2 = new Position(2, 0);
        var friendlyRook = new Rook(Color.White);
        var enemyBishop = new Bishop(Color.Black);
        
        board.CreateEmpty()
            .PlacePiece(friendlyRook, pos1)
            .PlacePiece(enemyBishop, pos2);
        
        var move = new Move(pos1, pos2);

        var snapshot = new GameSnapshot(
            false,
            Color.White, 
            board,
            1, 
            0);
        
        var result = MoveValidator.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Valid, result);
    }
    
    [Theory]
    [InlineData(2, 0)]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    public void PieceCannotGoThoughFriendlyPiece(int toRow, int toCol)
    {
        var board = new chess.Entities.Board.Board();
        var pos1 = new Position(7, 0);
        var pos2 = new Position(2, 0);
        var friendlyRook = new Rook(Color.White);
        var friendlyBishop = new Bishop(Color.White);
        
        board.CreateEmpty()
            .PlacePiece(friendlyRook, pos1)
            .PlacePiece(friendlyBishop, pos2);
        
        var move = new Move(pos1, new Position(toRow, toCol));

        var snapshot = new GameSnapshot(
            false,
            Color.White, 
            board,
            1, 
            0);
        
        var result = MoveValidator.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
    
    [Fact]
    public void KingCannotBeNextToEnemyKing()
    {
        var board = new chess.Entities.Board.Board();
        var pos1 = new Position(4, 4);
        var pos2 = new Position(2, 4);
        var friendlyKing = new King(Color.White);
        var enemyKing = new King(Color.Black);
        
        board.CreateEmpty()
            .PlacePiece(friendlyKing, pos1)
            .PlacePiece(enemyKing, pos2);
        
        var move = new Move(pos1, pos2 with{Row = pos2.Row - 1});

        var snapshot = new GameSnapshot(
            false,
            Color.White, 
            board,
            1, 
            0);
        
        var result = MoveValidator.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
}