using chess;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces.Types;
using Chess.Tests.Core.Board;
using Chess.Tests.Core.MoveValidation;
using chess.Validation;

namespace Chess.Tests.Core.Game;

public class CheckTests
{
    
    [Fact]
    public void CannotMovePinnedPiece()
    {
        var board = new chess.Board.Board();
        var king = new King(Color.White);
        var knight = new Knight(Color.White);
        var enemyRook = new Rook(Color.Black);

        board.CreateEmpty()
            .PlacePiece(king, new Position(6, 3))
            .PlacePiece(knight, new Position(5, 3))
            .PlacePiece(enemyRook, new Position(2, 3));
        
        /*
         * r    r
         * .    .
         * .    . . N
         * N    . .
         * K    K
         */
        
        var move = new Move(new Position(5, 3),
            new Position(4, 5));

        var snapshot = new GameSnapshot(
            GameStatus.InProgress, 
            Color.White,
            board, 
            1, 
            0,
            new CastlingRights(), 
            new List<Move>(),
            null);
        
        var result = MoveValidationTestsExtensions.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
    
    [Fact]
    public void CannotMoveIfTheKingIsInCheck()
    {
        var board = new chess.Board.Board();
        var king = new King(Color.White);
        var knight = new Knight(Color.White);
        var enemyRook = new Rook(Color.Black);

        board.CreateEmpty()
            .PlacePiece(king, new Position(6, 3))
            .PlacePiece(knight, new Position(6, 5))
            .PlacePiece(enemyRook, new Position(2, 3));
        
        /*
         * r        r
         * .        .
         * . .      . N
         * . .      . .
         * K . N    K .
         */
        
        var move = new Move(new Position(6, 5),
            new Position(4, 4));

        var snapshot = new GameSnapshot(
            GameStatus.InProgress, 
            Color.White,
            board, 
            1, 
            0, 
            new CastlingRights(), 
            new List<Move>(), 
            null);
        
        var result = MoveValidationTestsExtensions.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
    
    [Fact]
    public void CanCoverKingInCheck()
    {
        var board = new chess.Board.Board();
        var king = new King(Color.White);
        var knight = new Knight(Color.White);
        var enemyRook = new Rook(Color.Black);

        board.CreateEmpty()
            .PlacePiece(king, new Position(6, 3))
            .PlacePiece(knight, new Position(6, 5))
            .PlacePiece(enemyRook, new Position(2, 3));
        
        /*
         * r        r
         * .        .
         * . .      . 
         * . .      N .
         * K . N    K .
         */
        
        var move = new Move(new Position(6, 5),
            new Position(5, 3));

        var snapshot = new GameSnapshot(
            GameStatus.InProgress, 
            Color.White,
            board, 
            1, 
            0,
            new CastlingRights(), 
            new List<Move>(), 
            null);
        
        var result = MoveValidationTestsExtensions.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Valid, result);
    }
    
    [Fact]
    public void KingCannotMoveIntoCheck()
    {
        var board = new chess.Board.Board();
        var king = new King(Color.White);
        var enemyRook = new Rook(Color.Black);

        board.CreateEmpty()
            .PlacePiece(king, new Position(6, 4))
            .PlacePiece(enemyRook, new Position(2, 3));
        
        var move = new Move(new Position(6, 4),
            new Position(6, 3));

        var snapshot = new GameSnapshot(
            GameStatus.InProgress, 
            Color.White, 
            board, 
            1, 
            0,
            new CastlingRights(), 
            new List<Move>(), 
            null);
        
        var result = MoveValidationTestsExtensions.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
    
    [Fact]
    public void KingCannotCaptureProtectedPiece()
    {
        var board = new chess.Board.Board();
        var king = new King(Color.White);
        var enemyKnight = new Knight(Color.Black);
        var enemyRook = new Rook(Color.Black);

        board.CreateEmpty()
            .PlacePiece(king, new Position(6, 4))
            .PlacePiece(enemyKnight, new Position(5, 3))
            .PlacePiece(enemyRook, new Position(2, 3));
        
        var move = new Move(new Position(6, 4),
            new Position(5, 3));

        var snapshot = new GameSnapshot(
            GameStatus.InProgress, 
            Color.White, 
            board, 
            1, 
            0, 
            new CastlingRights(), 
            new List<Move>(), 
            null);
        
        var result = MoveValidationTestsExtensions.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
    
    [Fact]
    public void TestCheckmate()
    {
        var board = new chess.Board.Board();
        var king = new King(Color.White);
        var enemyQueen = new Queen(Color.Black);
        var enemyRook = new Rook(Color.Black);

        board.CreateEmpty()
            .PlacePiece(king, new Position(7, 0))
            .PlacePiece(enemyQueen, new Position(0, 3))
            .PlacePiece(enemyRook, new Position(6, 4));
        
        var move = new Move(new Position(0, 3),
            new Position(7, 3));

        var snapshot = new GameSnapshot(
            GameStatus.InProgress, 
            Color.Black,
            board, 
            1, 
            0,
            new CastlingRights(), 
            new List<Move>(), 
            null);;
        
        var result = MoveValidator.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Checkmate, result.Result);
    }
    
    [Fact]
    public void TestStalemate()
    {
        var board = new chess.Board.Board();
        var king = new King(Color.White);
        var enemyQueen = new Queen(Color.Black);

        board.CreateEmpty()
            .PlacePiece(king, new Position(7, 0))
            .PlacePiece(enemyQueen, new Position(4, 4));
        
        var move = new Move(new Position(4, 4),
            new Position(6, 2));

        var snapshot = new GameSnapshot(
            GameStatus.InProgress, 
            Color.Black, 
            board, 1, 
            0, 
            new CastlingRights(), 
            new List<Move>(), 
            null);
        
        var result = MoveValidator.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Stalemate, result.Result);
    }
}