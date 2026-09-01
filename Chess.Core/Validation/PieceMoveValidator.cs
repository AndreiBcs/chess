using chess.Game;
using chess.Moves;

namespace chess.Validation;

public static class PieceMoveValidator
{
    public static bool IsValid(GameSnapshot snapshot, Move move)
    {
        var board = snapshot.Board;
        var piece = board.GetPiece(move.From);
        
        if (piece is null || piece.Color != snapshot.CurrentTurn)
            return false;

        var possibleMoves = piece
            .GetPossiblePositions(board, move.From);

        return possibleMoves.Contains(move.To);
    }
}