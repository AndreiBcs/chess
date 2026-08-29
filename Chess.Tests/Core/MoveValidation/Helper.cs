using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Entities.Pieces.Types;
using chess.Game.GameState;
using Chess.Tests.Core.Board;

namespace Chess.Tests.Core.MoveValidation;

public static class Helper
{
    public static GameSnapshot GetSnapshotWithPiece(
        PieceType type, 
        Color color,
        int fromRow, 
        int fromCol,
        bool hasMoved = false)
    {
        var board = new chess.Entities.Board.Board();
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
            1);
    }
}