using chess.Game;
using chess.Moves;
using chess.Pieces;

namespace chess.Validation.MoveValidation;

public static class EnPassantValidator
{
    public static bool IsEnPassant(GameSnapshot snapshot, Move move)
    {
        var currentPiece = snapshot.Board.GetPiece(move.From);

        if (currentPiece?.Type != PieceType.Pawn)
            return false;

        if (currentPiece.Color == snapshot.CurrentTurn)
            return false;
        
        if(snapshot.MoveHistory.Count == 0)
            return false;
        
        var previousMove = snapshot.MoveHistory[^1];
        var lastMovedPiece = snapshot.Board.GetPiece(previousMove.To);
        
        if(lastMovedPiece?.Type != PieceType.Pawn)
            return false;
        
        if (Math.Abs(previousMove.To.Row - previousMove.From.Row) != 2)
            return false;

        if (previousMove.To.Row != move.From.Row)
            return false;

        if (Math.Abs(previousMove.To.Column - move.From.Column) != 1)
            return false;

        if (move.To.Column != previousMove.To.Column)
            return false;

        if (move.To.Row != previousMove.To.Row +
            (move.From.Row < previousMove.To.Row ? 1 : -1))
            return false;

        return true;
    }
}