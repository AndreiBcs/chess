using chess.Game;
using chess.Moves;
using chess.Pieces;

namespace chess.Validation.MoveValidation;

public static class EnPassantValidator
{
    public static bool IsValidEnPassant(GameSnapshot snapshot, Move move)
    {
        var currentPiece = snapshot.Board.GetPiece(move.From);
        var previousMove = snapshot.PreviousMove;
        var lastMovedPiece = snapshot.Board.GetPiece(previousMove.To);

        // basic en passant piece & position validation
        if (currentPiece?.Type != PieceType.Pawn ||
            currentPiece.Color != snapshot.CurrentTurn ||
            lastMovedPiece?.Type != PieceType.Pawn ||
            move.To != snapshot.EnPassantTarget)
            return false;
        
        // check if the last moved pawn moved 2 squares
        // and is now on the same row as the current moved pawn
        if (Math.Abs(previousMove.To.Row - previousMove.From.Row) != 2 ||
            previousMove.To.Row != move.From.Row)
            return false;

        // check if the last moved pawn moved exactly 1 column
        // next to the current moved pawn
        if (Math.Abs(previousMove.To.Column - move.From.Column) != 1 ||
            move.To.Column != previousMove.To.Column)
            return false;
        
        // simulate
        var testBoard = snapshot.Board
            .WithMove(move.From, move.To)
            .WithoutPiece(previousMove.To);

        return !CheckValidator.IsKingInCheck(testBoard, currentPiece.Color);
    }
}