using chess;
using chess.Board;
using chess.Pieces.Types;
using chess.Validation.MoveValidation;

namespace Chess.Tests.Core.MoveValidation;

public class PieceMovementTests
{
    
    [Theory]
    [InlineData(4, 4)]
    [InlineData(4, 5)]
    public void BishopMovementTest(int fromRow, int fromCol)
    {
        var bishop = new Bishop(Color.White);
        var board = Board.CreateEmptyBoard()
            .WithPiece(bishop, new Position(fromRow, fromCol));

        var piecePossiblePosition = bishop.GetPiecePositions(board);
        
        
    }
}