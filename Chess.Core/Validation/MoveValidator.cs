using chess.Board;
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
        
        if (piece is null) 
            return new MoveStatus(MoveResult.Invalid);
        
        if (target?.Type == PieceType.King) // cannot capture the king
            return new MoveStatus(MoveResult.Invalid);
        
        var isPawnMove = piece.Type == PieceType.Pawn;
        var isCapture = target is not null;
        
        // 2. check for castling
        if (CastleValidator.IsCastlingAttempt(piece, move))
        {
            // validate
            if (!CastleValidator.TryGetCastling(snapshot, move, out var castling)
                || !CastleValidator.CanCastle(snapshot, castling))
            {
                return new MoveStatus(MoveResult.Invalid);
            }

            // simulate
            var castleTestBoard = snapshot.Board.Copy();

            castleTestBoard.MovePiece(castling.KingFrom, castling.KingTo);
            castleTestBoard.MovePiece(castling.RookFrom, castling.RookTo);

            return EvaluateBoard(
                castleTestBoard, 
                piece.Color,
                isCastling: true);
        }

        // 3. check for en passant
        if (piece.Type == PieceType.Pawn && 
            EnPassantValidator.IsEnPassant(snapshot, move))
        {
            // simulate
            var enPassantTestBoard = snapshot.Board.Copy();
            
            enPassantTestBoard.MovePiece(move.From, move.To);
            
            var capturedPawnPosition = new Position(move.From.Row, move.To.Column);
            enPassantTestBoard.RemovePiece(capturedPawnPosition);
            
            return EvaluateBoard( 
                enPassantTestBoard, 
                piece.Color,
                isPawnMove: true,
                isCapture: true,
                isEnPassant: true);
        }
        
        // 4. simulate move
        var testBoard = snapshot.Board.Copy();

        testBoard.MovePiece(move.From, move.To);

        // 5. promotion
        if (piece.Type == PieceType.Pawn 
            && move.To.Row is 0 or 7 
            && move.Promotion is not null)
        {
            testBoard.ReplacePromotion(move.To, move.Promotion, piece.Color);
            
            return EvaluateBoard( 
                testBoard, 
                piece.Color,
                isPawnMove: true, 
                isCapture: isCapture,
                isPromotion: true); 
        }
        
        return EvaluateBoard( 
            testBoard, 
            piece.Color,
            isPawnMove: isPawnMove,
            isCapture: isCapture);
    }

    private static MoveStatus EvaluateBoard(
        Board.Board board,
        Color movingColor,
        bool isCapture = false,
        bool isPawnMove = false,
        bool isCastling = false, 
        bool isEnPassant = false,
        bool isPromotion = false)
    {
        // the player cannot make a move that leaves
        // their own king in check.
        if (CheckValidator.IsKingInCheck(board, movingColor))
            return new MoveStatus(MoveResult.Invalid);

        var opponentColor = movingColor == Color.White ?  Color.Black : Color.White;

        var opponentInCheck = CheckValidator.IsKingInCheck(board, opponentColor);
        var opponentHasLegalMove = HasLegalMove(board, opponentColor);

        var result =  opponentInCheck switch
        {
            true when !opponentHasLegalMove => MoveResult.Checkmate,
            
            false when !opponentHasLegalMove => MoveResult.Stalemate,
            
            _ => MoveResult.Valid
        };

        return new MoveStatus(
            result,
            isCapture,
            isPawnMove,
            isCastling,
            isEnPassant,
            isPromotion);
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
                // TODO en-passant and castle are not considered here
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
}