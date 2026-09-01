using chess.Game;
using chess.Moves;
using chess.Pieces;

namespace chess.Validation;

public static class MoveValidator
{
    public static MoveStatus ValidateMove(GameSnapshot snapshot, Move move)
    {
        // 1. basic move validation
        if (!PieceMoveValidator.IsValid(snapshot, move))
            return new MoveStatus(MoveResult.Invalid);
        
        var board = snapshot.Board;
        var piece = board.GetPiece(move.From);
        var target = board.GetPiece(move.To);
        
        if (target?.Type == PieceType.King) // cannot capture the king
            return new MoveStatus(MoveResult.Invalid);
        
        // 2. check for special move - castling
        if (CastleValidator.IsCastlingAttempt(piece!, move))
        {
            if (!CastleValidator.TryGetCastling(snapshot, move, out var castling)
                || !CastleValidator.CanCastle(snapshot, castling))
            {
                return new MoveStatus(MoveResult.Invalid);
            }

            var castleTestBoard = snapshot.Board.Copy();

            castleTestBoard.MovePiece(castling.KingFrom, castling.KingTo);
            castleTestBoard.MovePiece(castling.RookFrom, castling.RookTo);

            return EvaluateBoard(castleTestBoard, piece!.Color, MoveType.Castle);
        }

        // 3. normal move
        var testBoard = snapshot.Board.Copy();

        testBoard.MovePiece(move.From, move.To);

        var moveType = snapshot.Board.GetPiece(move.To) is null
            ? MoveType.Normal
            : MoveType.Capture;
        
        moveType = snapshot.Board.GetPiece(move.From)!.Type == PieceType.Pawn
            ? MoveType.PawnAdvance
            : moveType;

        return EvaluateBoard(testBoard, piece!.Color, moveType);
    }

    private static MoveStatus EvaluateBoard(
        Board.Board board,
        Color movingColor,
        MoveType moveType)
    {
        // the player cannot make a move that leaves
        // their own king in check.
        if (CheckValidator.IsKingInCheck(board, movingColor))
            return new MoveStatus(MoveResult.Invalid, moveType);

        var opponentColor = GetOpponent(movingColor);

        var opponentInCheck = CheckValidator.IsKingInCheck(board, opponentColor);
        var opponentHasLegalMove = HasLegalMove(board, opponentColor);

        return opponentInCheck switch
        {
            true when !opponentHasLegalMove => 
                new MoveStatus(MoveResult.Checkmate, moveType),
            
            false when !opponentHasLegalMove => 
                new MoveStatus(MoveResult.Stalemate, moveType),
            
            _ => new MoveStatus(MoveResult.Valid, moveType)
        };
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
                // never consider capturing the king a legal move.
                if (board.GetPiece(destination)?.Type == PieceType.King)
                    continue;

                var testBoard = board.Copy();

                testBoard.MovePiece(square.Position, destination);

                if (!CheckValidator.IsKingInCheck(testBoard, color))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static Color GetOpponent(Color color)
    {
        return color == Color.White
            ? Color.Black
            : Color.White;
    }
}