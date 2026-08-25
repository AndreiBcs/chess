using chess.Entities.Board;
using chess.Game.GameState;

namespace chess.Game.Validators;

public static class MoveValidator
{
    public static bool ValidateMove(GameSnapshot snapshot, Move move)
    {
        var piece = snapshot.Board.GetPiece(move.From);
        
        if (piece is null) return false;

        var possibleMoves = piece
            .GetPossiblePositions(snapshot.Board, move.From);

        return possibleMoves.Contains(move.To) &&
               snapshot.Board.IsValidMove(move);
    }
}