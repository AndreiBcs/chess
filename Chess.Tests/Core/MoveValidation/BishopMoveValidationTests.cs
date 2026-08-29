using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Game.Validators;

namespace Chess.Tests.Core.MoveValidation;

public class BishopMoveValidationTests
{
    /* TODO
     * can capture
     * cannot capture own piece
     */
    
    [Theory]
    [InlineData(4, 4, 3, 3)]
    [InlineData(4, 4, 2, 2)]
    [InlineData(4, 4, 1, 1)]
    [InlineData(4, 4, 0, 0)]
    [InlineData(4, 4, 5, 5)]
    [InlineData(4, 4, 6, 6)]
    [InlineData(4, 4, 7, 7)]
    [InlineData(4, 4, 3, 5)]
    [InlineData(4, 4, 2, 6)]
    [InlineData(4, 4, 1, 7)]
    [InlineData(4, 4, 5, 3)]
    [InlineData(4, 4, 6, 2)]
    [InlineData(4, 4, 7, 1)]
    public void Bishop_CanMoveDiagonally(int fromRow, int fromCol, int toRow, int toCol)
    {
        var snapshot = Helper.GetSnapshotWithPiece(
            PieceType.Bishop,
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