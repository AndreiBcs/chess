using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces;

namespace chess.Validation;

public static class MoveValidator
{
    public static MoveResult ValidateMove(GameSnapshot snapshot, Move move)
    {
        var board = snapshot.Board;
        var piece = board.GetPiece(move.From);

        if (piece is null || piece.Color != snapshot.CurrentTurn)
            return MoveResult.Invalid;

        var possibleMoves = piece
            .GetPossiblePositions(board, move.From);

        if (!possibleMoves.Contains(move.To))
            return MoveResult.Invalid;
        
        var target = board.GetPiece(move.To);
        if (target?.Type == PieceType.King)
            return MoveResult.Invalid;

        var isCastlingAttempt =
            piece.Type == PieceType.King &&
            move.From.Row == move.To.Row &&
            move.To.Column - move.From.Column is 2 or -2;

        if (isCastlingAttempt)
        {
            if (!IsCastlingMove(snapshot, move, piece.Color, out var castling))
                return MoveResult.Invalid;
            if (!CanCastle(snapshot, move))
                return MoveResult.Invalid;

            return ValidateBoardState(snapshot, piece.Color, testBoard =>
            {
                testBoard.MovePiece(castling.KingFrom, castling.KingTo);
                testBoard.MovePiece(castling.RookFrom, castling.RookTo);
            });
        }

        return ValidateBoardState(snapshot, piece.Color, testBoard =>
        {
            testBoard.MovePiece(move.From, move.To);
        });
    }

    public static bool IsKingInCheck(Board.Board board, Color kingColor)
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
    
    private static bool HasLegalMove(Board.Board board, Color color)
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

    private static bool CanCastle(GameSnapshot snapshot, Move move)
    {
        var piece = snapshot.Board.GetPiece(move.From);

        if (piece is null ||
            piece.Color != snapshot.CurrentTurn ||
            piece.Type != PieceType.King)
            return false;

        foreach (var castlingPosition in snapshot.CastlingRights.CastlingPositions)
        {
            if (castlingPosition.Color == piece.Color &&
                castlingPosition.KingFrom == move.From &&
                castlingPosition.KingTo == move.To)
            {
                foreach (var position in castlingPosition.KingSafePositions)
                {
                    var board = snapshot.Board.Copy();

                    if (position != castlingPosition.KingFrom)
                    {
                        board.MovePiece(castlingPosition.KingFrom, position);
                    }

                    if (IsKingInCheck(board, piece.Color))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return false;
    }

    public static bool IsCastlingMove(
        GameSnapshot snapshot,
        Move move,
        Color pieceColor,
        out CastlingInfo castling)
    {
        foreach (var castlingPosition in snapshot.CastlingRights.CastlingPositions)
        {
            if (castlingPosition.Color == pieceColor &&
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

    private static MoveResult ValidateBoardState(
        GameSnapshot snapshot,
        Color movingColor,
        Action<Board.Board> applyMove)
    {
        var testBoard = snapshot.Board.Copy();
        applyMove(testBoard);

        if (IsKingInCheck(testBoard, movingColor))
            return MoveResult.Invalid;

        var opponentColor = movingColor == Color.White ? Color.Black : Color.White;

        if (IsKingInCheck(testBoard, opponentColor))
        {
            if (!HasLegalMove(testBoard, opponentColor))
                return MoveResult.Checkmate;
        }
        else if (!HasLegalMove(testBoard, opponentColor))
        {
            return MoveResult.Stalemate;
        }

        return MoveResult.Valid;
    }
}