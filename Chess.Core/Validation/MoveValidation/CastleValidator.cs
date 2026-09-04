using chess.Game;
using chess.Moves;
using chess.Pieces;

namespace chess.Validation.MoveValidation;

public static class CastleValidator
{
    public static bool IsCastlingAttempt(Piece piece, Move move)
    {
        return piece.Type == PieceType.King &&
               move.From.Row == move.To.Row &&
               move.To.Column - move.From.Column is 2 or -2;
    }
    
    public static bool TryGetCastling(
        GameSnapshot snapshot,
        Move move,
        out CastlingRights castling)
    {
        foreach (var castlingPosition in snapshot.CastlingRights.CastlingPositions)
        {
            if (castlingPosition.Color == snapshot.CurrentTurn &&
                castlingPosition.KingFrom == move.From &&
                castlingPosition.KingTo == move.To)
            {
                castling = castlingPosition;
                return true;
            }
        }

        castling = default;
        return false;
    }
    
    public static bool CanCastle(GameSnapshot snapshot, CastlingRights castling)
    {
        var piece = snapshot.Board.GetPiece(castling.KingFrom);

        if (piece is null ||
            piece.Type != PieceType.King ||
            piece.Color != snapshot.CurrentTurn)
        {
            return false;
        }
        
        foreach (var position in castling.KingSafePositions)
        {
            var board = snapshot.Board.Copy();
            
            if (castling.KingFrom == position) // check the initial king position
            {
                if (CheckValidator.IsKingInCheck(board, piece.Color))
                {
                    return false;
                }
            }

            board.MovePiece(castling.KingFrom, position);

            if (position == castling.KingTo)
            {
                board.MovePiece(castling.RookFrom, castling.RookTo);
            }

            if (CheckValidator.IsKingInCheck(board, piece.Color))
            {
                return false;
            }
        }

        return true;
    }
}