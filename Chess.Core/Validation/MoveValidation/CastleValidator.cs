using chess.Game;
using chess.Moves;
using chess.Pieces;

namespace chess.Validation.MoveValidation;

public static class CastleValidator
{
    public static bool IsValidCastling(
        GameSnapshot snapshot,
        Move move,
        out CastlingRights? castling)
    {
        var king = snapshot.Board.GetPiece(move.From);

        // check if the move matches a king castling move
        if (king is not { Type: PieceType.King, HasMoved: false } ||
            king.Color != snapshot.CurrentTurn ||
            move.From.Row != move.To.Row ||
            Math.Abs(move.To.Column - move.From.Column) != 2)
        {
            castling = null;
            return false;
        }

        // get the right castling move
        castling = snapshot.CastlingRights
            .SingleOrDefault(c => 
                c.Color == snapshot.CurrentTurn &&
                c.KingFrom == move.From &&
                c.KingTo == move.To);

        if (castling is null)
        {
            return false;
        }
        
        // get the rook
        var rook = snapshot.Board.GetPiece(castling.Value.RookFrom);
        
        if (rook is not { Type: PieceType.Rook, HasMoved: false } ||
           rook.Color != king.Color)
        {
            return false;
        }
        
        // check if the squares are empty between the king and rook
        if (!castling.Value.BetweenPositions
                .ToList()
                .TrueForAll(p => snapshot.Board.GetPiece(p) is null))
        {
            return false;
        }

        // test if the king can castle without being in check
        foreach (var safePosition in castling.Value.KingSafePositions)
        {
            // simulate
            var testBoard = safePosition == castling.Value.KingTo
                ? snapshot.Board
                    .WithMove(move.From, safePosition)
                    .WithMove(castling.Value.RookFrom, castling.Value.RookTo)
                : snapshot.Board.WithMove(move.From, safePosition);

            if (CheckValidator.IsKingInCheck(testBoard, king.Color))
            {
                return false;
            }
        }
        
        return true;
    }
}