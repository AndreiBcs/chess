using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Game.GameState;

namespace chess.Game.Validators;

public static class MoveValidator
{
    public static MoveResult ValidateMove(GameSnapshot snapshot, Move move)
    {
        // 1. test if the piece can be moved there
        var board = snapshot.Board;
        var piece = board.GetPiece(move.From);

        if (piece is null || piece.Color != snapshot.CurrentTurn)
            return MoveResult.Invalid;

        var possibleMoves = piece
            .GetPossiblePositions(board, move.From);

        if (!possibleMoves.Contains(move.To))
            return MoveResult.Invalid;
        
        var target = board.GetPiece(move.To);
        if (target?.Type == PieceType.King) // cannot capture the king
            return MoveResult.Invalid;
        
        // TODO check for castling
        
        // 2. test the move's side effects
        var testBoard = snapshot.Board.Copy();
        testBoard.MovePiece(move.From, move.To);
        
        if(IsKingInCheck(testBoard, piece.Color))
            return MoveResult.Invalid;
        
        var opponentColor = piece.Color == Color.White ? Color.Black : Color.White;

        if (IsKingInCheck(testBoard, opponentColor))
        {
            if (!HasLegalMove(testBoard, opponentColor))
                return MoveResult.Checkmate;
        }
        else
        {
            if (!HasLegalMove(testBoard, opponentColor))
                return MoveResult.Stalemate;
        }
        
        return MoveResult.Valid;
    }

    private static bool IsKingInCheck(Board board, Color kingColor)
    {
        var kingPosition = board.GetKingPosition(kingColor);

        var enemyColor = kingColor == Color.White ? Color.Black : Color.White;

        foreach (var square in board.GetSquares())
        {
            var piece = square.Piece;

            if (piece is null || piece.Color != enemyColor)
                continue;

            if (piece
                .GetAttackPositions(board, square.Position)
                .Contains(kingPosition))
            {
                return true;
            }
        }

        return false;
    }
    
    private static bool HasLegalMove(Board board, Color color)
    {
        foreach (var square in board.GetSquares())
        {
            var piece = square.Piece;

            if (piece is null || piece.Color != color)
                continue;

            var possiblePositions =
                piece.GetPossiblePositions(board, square.Position);

            foreach (var destination in possiblePositions)
            {
                var target = board.GetPiece(destination);

                if (target?.Type == PieceType.King)
                    continue;

                var testBoard = board.Copy();
                testBoard.MovePiece(square.Position, destination);

                if (!IsKingInCheck(testBoard, color))
                    return true;
            }
        }

        return false;
    }
}