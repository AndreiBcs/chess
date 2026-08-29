using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Game.Validators;

namespace Chess.Tests.Core.MoveValidation;

public class KingMoveValidationTests
{
    /* TODO
     * cannot move more then 1 square
     * cannot capture own piece
     */
    
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
        var snapshot = Helper.GetSnapshotWithPiece(
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
}