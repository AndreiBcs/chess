using chess;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces.Types;
using chess.Validation.MoveValidation;

namespace Chess.Tests.Core.ValidationTests;

public class EnPassantTests
{
    [Fact]
    public void EnPassantIsValid()
    {
        var board = Board.CreateEmptyBoard()
            .WithPiece(new King(Color.White), new Position(7, 7))
            .WithPiece(new King(Color.Black), new Position(0, 7))
            .WithPiece(new Pawn(Color.White), new Position(3, 2))
            .WithPiece(new Pawn(Color.Black), new Position(1, 3));
        
        var snapshot = GameSnapshot.CreateCustomSnapshot(
            GameStatus.InProgress,
            board,
            Color.Black,
            null,
            null,
            null,
            null,
            new Move(new Position(7, 6), new Position(7, 7)),
            null
        );
        
        snapshot = GameSnapshot.GetUpdatedGameSnapshot(
            snapshot,
            new Move(new Position(1, 3), new Position(3, 3)),
            new MoveStatus(MoveResult.Valid));

        var moveStatus = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(3, 2 ), new Position(2, 3)));
        
        Assert.Equal(
            new MoveStatus(
                MoveResult.Valid, 
                IsCapture: true, 
                IsPawnMove: true, 
                IsEnPassant: true), 
            moveStatus);
    }
}