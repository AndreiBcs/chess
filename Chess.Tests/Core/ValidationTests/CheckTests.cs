using chess;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces.Types;
using chess.Validation.MoveValidation;

namespace Chess.Tests.Core.ValidationTests;

public class CheckTests
{
    [Fact]
    public void KingCannotMoveIntoCheck()
    {
        var board = Board.CreateEmptyBoard()
            .WithPiece(new King(Color.White), new Position(7, 7))
            .WithPiece(new King(Color.Black), new Position(0, 7))
            .WithPiece(new Rook(Color.White), new Position(7, 6));
        
        var snapshot = GameSnapshot.CreateCustomSnapshot(
            GameStatus.InProgress,
            board,
            Color.Black,
            null,
            null,
            null,
            null,
            new Move(new Position(7, 5), new Position(7, 6)),
            null
        );
        
        var moveStatus = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(0, 7), new Position(0, 6)));
        
        Assert.Equal(
            new MoveStatus(MoveResult.Invalid, InvalidMoveReason: "Move leaves the king in check."), 
            moveStatus);
    }
    
    [Fact]
    public void CannotLeaveOwnKingInCheck()
    {
        var board = Board.CreateEmptyBoard()
            .WithPiece(new King(Color.White), new Position(7, 7))
            .WithPiece(new King(Color.Black), new Position(0, 7))
            .WithPiece(new Rook(Color.White), new Position(0, 0))
            .WithPiece(new Bishop(Color.Black), new Position(3, 3));
        
        var snapshot = GameSnapshot.CreateCustomSnapshot(
            GameStatus.InProgress,
            board,
            Color.Black,
            null,
            null,
            null,
            null,
            new Move(new Position(1, 0), new Position(0, 0)),
            null
        );
        
        var moveStatus = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(3, 3), new Position(4, 2)));
        
        Assert.Equal(
            new MoveStatus(MoveResult.Invalid, InvalidMoveReason: "Move leaves the king in check."), 
            moveStatus);
    }
    
    [Fact]
    public void HideKingInCheckBehindOtherPiece()
    {
        var board = Board.CreateEmptyBoard()
            .WithPiece(new King(Color.White), new Position(7, 7))
            .WithPiece(new King(Color.Black), new Position(0, 7))
            .WithPiece(new Rook(Color.White), new Position(0, 0))
            .WithPiece(new Bishop(Color.Black), new Position(3, 3));
        
        var snapshot = GameSnapshot.CreateCustomSnapshot(
            GameStatus.InProgress,
            board,
            Color.Black,
            null,
            null,
            null,
            null,
            new Move(new Position(1, 0), new Position(0, 0)),
            null
        );
        
        var moveStatus = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(3, 3), new Position(0, 6)));
        
        Assert.Equal(
            new MoveStatus(MoveResult.Valid), 
            moveStatus);
    }
}