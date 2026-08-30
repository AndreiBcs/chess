using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces.Types;
using chess.Game.GameState;
using chess.Game.Validators;
using Chess.Tests.Core.Board;

namespace Chess.Tests.Core.Game;

public class CheckTests
{
    /* TODO
     * cannot move if king in check
     * king cannot move into check
     * king cannot capture a protected piece
     * check detection
     * checkmate
     * stalemate
     */

    [Fact]
    public void CannotMoveIfTheKingIsInCheck()
    {
        var board = new chess.Entities.Board.Board();
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
            false,
            Color.White, 
            board,
            1, 
            0);
        
        var result = MoveValidator.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
}