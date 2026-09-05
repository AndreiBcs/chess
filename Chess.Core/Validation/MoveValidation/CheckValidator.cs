namespace chess.Validation.MoveValidation;

public static class CheckValidator
{
    public static bool IsKingInCheck(Board.Board board, Color kingColor)
    {
        var kingPosition = board.GetKingPosition(kingColor);

        var enemyColor = kingColor == Color.White ? Color.Black : Color.White;

        foreach (var square in board.CopySquares())
        {
            var piece = square.Piece;

            if (piece is null || piece.Color != enemyColor)
                continue;

            if (piece.GetPiecePositions(board, true).Contains(kingPosition))
            {
                return true;
            }
        }

        return false;
    }
}