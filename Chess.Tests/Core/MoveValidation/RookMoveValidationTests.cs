using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Game.Validators;

namespace Chess.Tests.Core.MoveValidation;

public class RookMoveValidationTests
{
    
    [Theory]
    [InlineData(4, 4, 3, 4)]
    [InlineData(4, 4, 2, 4)]
    [InlineData(4, 4, 1, 4)]
    [InlineData(4, 4, 0, 4)]
    [InlineData(4, 4, 5, 4)]
    [InlineData(4, 4, 6, 4)]
    [InlineData(4, 4, 7, 4)]
    [InlineData(4, 4, 4, 3)]
    [InlineData(4, 4, 4, 2)]
    [InlineData(4, 4, 4, 1)]
    [InlineData(4, 4, 4, 0)]
    [InlineData(4, 4, 4, 5)]
    [InlineData(4, 4, 4, 6)]
    [InlineData(4, 4, 4, 7)]
    public void Rook_CanMoveUpDownLeftRight(int fromRow, int fromCol, int toRow, int toCol)
    {
        var snapshot = Helpers.GetSnapshotWithPiece(
            PieceType.Rook,
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
    [InlineData(4, 4, -1, 4)]
    [InlineData(4, 4, 8, 4)]
    [InlineData(6, 6, 6, 8)]
    [InlineData(6, 6, 6, -1)]
    public void Rook_CannotMoveOutsideTheBoard(int fromRow, int fromCol, int toRow, int toCol)
    {
        var snapshot = Helpers.GetSnapshotWithPiece(
            PieceType.Rook,
            Color.White,
            fromRow,
            fromCol);

        var move = new Move
        {
            From = new Position(fromRow, fromCol),
            To = new Position(toRow, toCol)
        };
        
        var result = MoveValidator.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
}