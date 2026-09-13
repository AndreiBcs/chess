using chess.Game;
using chess.Moves;
using chess.Pieces;

namespace chess.Validation.MoveValidation;

public static class MoveValidator
{
    public static MoveStatus ValidateMove(GameSnapshot snapshot, Move move)
    {
        var board = snapshot.Board;
        var piece = board.GetPiece(move.From);

        if (piece is null)
        {
            return new MoveStatus(
                MoveResult.Invalid,
                InvalidMoveReason: "Cannot move from empty square.");
        }

        if (piece.Color != snapshot.CurrentTurn)
        {
            return new MoveStatus(
                MoveResult.Invalid,
                InvalidMoveReason: "Cannot move enemy piece.");
        }
        
        var target = board.GetPiece(move.To);

        // 1. check for special moves before basic movement validation
        if (CastleValidator.IsValidCastling(snapshot, move, out var castlingRights))
        {
            return new MoveStatus(
                MoveResult.Valid,
                IsCastling: true,
                CastlingRights: castlingRights);
        }

        if (EnPassantValidator.IsValidEnPassant(snapshot, move))
        {
            return new MoveStatus(
                MoveResult.Valid,
                IsEnPassant: true,
                IsCapture: true,
                IsPawnMove: true);
        }

        // 2. pseudo move validation
        if (!piece.GetPiecePositions(board).Contains(move.To))
        {
            return new MoveStatus(
                MoveResult.Invalid,
                InvalidMoveReason: "Invalid move for selected piece.");
        }

        // 3. check + simulate non-special move
        var testBoard = board.CopyBoard();
        var isPromotion = false;
        
        if (piece.Type == PieceType.Pawn // check promotion
            && move.To.Row is 0 or 7 
            && move.Promotion is not null)
        {
            isPromotion = true;
            testBoard = testBoard
                .WithMove(move.From, move.To)
                .WithPromotion(move.To, move.Promotion.Value, piece.Color);
        }
        else // not promotion => normal move
        {
            testBoard = testBoard.WithMove(move.From, move.To);
        }

        return CheckValidator.IsKingInCheck(testBoard, piece.Color) 
            ? new MoveStatus(
                MoveResult.Invalid,
                InvalidMoveReason: "Move leaves the king in check.") 
            : new MoveStatus(MoveResult.Valid,
                IsCapture: target is not null,
                IsPawnMove: piece.Type == PieceType.Pawn,
                IsPromotion: isPromotion);
    }
}