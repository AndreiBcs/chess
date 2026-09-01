using chess;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces;
using chess.Pieces.Types;
using Chess.Tests.Core.Board;
using chess.Validation;

namespace Chess.Tests.Core.MoveValidation;

public static class MoveValidationTestsExtensions
{
    public static GameSnapshot GetSnapshotWithPiece(
        PieceType type, 
        Color color,
        int fromRow, 
        int fromCol,
        bool hasMoved = false)
    {
        var board = new chess.Board.Board();
        Piece piece = type switch
        {
            PieceType.Bishop => new Bishop(color),
            PieceType.King => new King(color, hasMoved),
            PieceType.Knight => new Knight(color),
            PieceType.Pawn => new Pawn(color, hasMoved),
            PieceType.Queen => new Queen(color),
            PieceType.Rook => new Rook(color, hasMoved),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        board.CreateEmpty().PlacePiece(piece, new Position(fromRow, fromCol));
        
        return new GameSnapshot(
            false,
            color,
            board,
            0,
            1,
            new CastlingRights());
    }

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
        if (target?.Type == PieceType.King) // cannot capture the king
            return MoveResult.Invalid;
        
        // TODO check for castling
        
        // 2. test the move's side effects
        var testBoard = snapshot.Board.Copy();
        testBoard.MovePiece(move.From, move.To);
        
        if(CheckValidator.IsKingInCheck(testBoard, piece.Color))
            return MoveResult.Invalid;

        return MoveResult.Valid;
    }
}