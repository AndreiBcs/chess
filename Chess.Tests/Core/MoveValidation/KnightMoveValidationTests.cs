using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Game.Validators;

namespace Chess.Tests.Core.MoveValidation;

public class KnightMoveValidationTests
{
    /* TODO
     * can capture
     * cannot capture own piece
     */

    [Theory]
    [InlineData(4, 4, 2, 3)]
    [InlineData(4, 4, 2, 5)]
    [InlineData(4, 4, 3, 2)]
    [InlineData(4, 4, 3, 6)]
    [InlineData(4, 4, 5, 2)]
    [InlineData(4, 4, 5, 6)]
    [InlineData(4, 4, 6, 3)]
    [InlineData(4, 4, 6, 5)]
    public void Knight_CanMoveInLShape(int fromRow, int fromCol, int toRow, int toCol)
    {
        var snapshot = Helper.GetSnapshotWithPiece(
            PieceType.Knight,
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
}