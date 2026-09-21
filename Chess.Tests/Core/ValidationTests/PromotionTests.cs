using chess;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces;
using chess.Pieces.Types;
using chess.Validation.MoveValidation;

namespace Chess.Tests.Core.ValidationTests;

public class PromotionTests
{
    [Theory]
    [InlineData(Color.White, 1)]
    [InlineData(Color.White, 2)]
    [InlineData(Color.White, 3)]
    [InlineData(Color.White, 4)]
    [InlineData(Color.White, 5)]
    [InlineData(Color.White, 6)]
    [InlineData(Color.Black, 1)]
    [InlineData(Color.Black, 2)]
    [InlineData(Color.Black, 3)]
    [InlineData(Color.Black, 4)]
    [InlineData(Color.Black, 5)]
    [InlineData(Color.Black, 6)]
    public void PromoteToNewPiece(
        Color color,
        int promotingPieceIndex)
    {
        var promotionRow = color == Color.White ? 0 : 7;
        var initialPawnRow = Math.Abs(promotionRow - 1);
        
        Piece promotingPiece = promotingPieceIndex switch
        {
            1 => new Queen(color),
            2 => new Rook(color),
            3 => new Knight(color),
            4 => new Bishop(color),
            5 => new King(color),
            _ => new Pawn(color)
        };
        
        var board = Board.CreateInitial()
            .WithoutPiece(new Position(promotionRow, 1))
            .WithPiece(new Pawn(color), new Position(initialPawnRow, 1));
        
        var snapshot = GameSnapshot.CreateCustomSnapshot(
            null,
            board,
            color,
            null,
            null,
            null,
            null,
            null,
            null
        );
        
        var moveStatus = MoveValidator.ValidateMove(
            snapshot,
            new Move(
                new Position(initialPawnRow, 1),
                new Position(promotionRow, 1),
                promotingPiece.Type));

        if (promotingPiece is King or Pawn)
        {
            Assert.Equal(
                new MoveStatus(MoveResult.Invalid, InvalidMoveReason: "Cannot promote to a Pawn or a King."),
                moveStatus);
        }
        else
        {
            Assert.Equal(
                new MoveStatus(MoveResult.Valid, IsPromotion: true, IsPawnMove: true),
                moveStatus);
    
            var updatedSnapshot = GameSnapshot.GetUpdatedGameSnapshot(snapshot, 
                new Move(
                    new Position(initialPawnRow, 1),
                    new Position(promotionRow, 1),
                    promotingPiece.Type),
                moveStatus);
    
            Assert.Null(updatedSnapshot.Board.GetPiece(new Position(initialPawnRow, 1)));
            Assert.Equal(promotingPiece, updatedSnapshot.Board.GetPiece(new Position(promotionRow, 1)));
        }
    }
}