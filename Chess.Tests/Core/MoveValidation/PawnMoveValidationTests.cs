using chess.Entities.Board;
using chess.Entities.Common;
using chess.Game.GameState;
using chess.Game.Validators;

namespace Chess.Tests.Core.MoveValidation;

public class PawnMoveValidationTests
{
    /* TODO
     * move 1 vertically
     * move 2 vertically if first move
     * cannot move more then 2 squares
     * can capture diagonally
     * cannot move through own piece
     */
    
    [Fact]
    public void Pawn_CannotMoveThreeSquares()
    {
        var snapshot = new GameSnapshot(
            false,
            Color.White,
            new chess.Entities.Board.Board(),
            0,
            1);

        var move = new Move(
            new Position(6, 0),
            new Position(3, 0));
        
        var result = MoveValidator.ValidateMove(snapshot, move);
        
        Assert.NotEqual(MoveResult.Valid, result);
    }
}