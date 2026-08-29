using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Game.Validators;

namespace Chess.Tests.Core.MoveValidation;

public class KingMoveValidationTests
{
    
    [Theory]
    [InlineData(4, 4, 3, 3)]
    [InlineData(4, 4, 5, 5)]
    [InlineData(4, 4, 3, 5)]
    [InlineData(4, 4, 5, 3)]
    [InlineData(4, 4, 3, 4)]
    [InlineData(4, 4, 5, 4)]
    [InlineData(4, 4, 4, 3)]
    [InlineData(4, 4, 4, 5)]
    public void King_CanMoveUpDownLeftRightDiagonally(int fromRow, int fromCol, int toRow, int toCol)
    {
        var snapshot = Helpers.GetSnapshotWithPiece(
            PieceType.King,
            Color.White,
            fromRow,
            fromCol);

        var move = new Move
        {
            From = new Position(fromRow, fromCol),
            To = new Position(toRow, toCol)
        };
        
        var result = MoveValidator.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Valid, result);
    }
    
    [Theory]
    [InlineData(0, 0, -1, -1)]
    [InlineData(7, 7, 8, 8)]
    public void King_CannotMoveOutsideTheBoard(int fromRow, int fromCol, int toRow, int toCol)
    {
        var snapshot = Helpers.GetSnapshotWithPiece(
            PieceType.King,
            Color.White,
            fromRow,
            fromCol,
            true);

        var move = new Move
        {
            From = new Position(fromRow, fromCol),
            To = new Position(toRow, toCol)
        };
        
        var result = MoveValidator.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
}