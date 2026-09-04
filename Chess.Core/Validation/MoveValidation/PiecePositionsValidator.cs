using System.Collections.Immutable;
using chess.Board;
using chess.Pieces;

namespace chess.Validation.MoveValidation;

public static class PiecePositionsValidator
{
    public static IEnumerable<Position> GetPiecePositions(
        this Piece piece, 
        Board.Board board,
        bool getAttackPositions = false)
    {
        var piecePosition = board.GetPiecePosition(piece);
        
        if (piecePosition is null) return ImmutableList<Position>.Empty;
        
        var positions = new List<Position>();
        
        var directions = getAttackPositions 
            ? piece.GetAttackDirections() 
            : piece.GetMoveDirections();
        
        switch (piece.Type)
        {
            case PieceType.Pawn:
            {
                foreach (var (rowDir, colDir) in directions)
                {
                    var row = piecePosition.Value.Row + rowDir;
                    var col = piecePosition.Value.Column + colDir;

                    if (row is < 0 or >= 8 || col is < 0 or >= 8)
                        continue;

                    var pos = new Position(row, col);
                    var localPiece = board.GetPiece(pos);

                    if (getAttackPositions)
                    {
                        if (colDir != 0) positions.Add(pos);

                        continue;
                    }

                    if (colDir == 0)
                    {
                        if (localPiece is not null)
                            continue;

                        if (Math.Abs(rowDir) == 2)
                        {
                            var oneForward = piecePosition.Value with
                            {
                                Row = piecePosition.Value.Row + rowDir / 2
                            };

                            if (board.GetPiece(oneForward) is not null)
                                continue;
                        }

                        positions.Add(pos);
                        continue;
                    }

                    if (localPiece is not null && localPiece.Color != piece.Color)
                    {
                        positions.Add(pos);
                    }
                }
                break;
            }
            case PieceType.King or PieceType.Knight:
            {
                foreach (var (rowDir, colDir) in directions)
                {
                    var row = piecePosition.Value.Row + rowDir;
                    var col = piecePosition.Value.Column + colDir;

                    if (row is < 0 or >= 8 || col is < 0 or >= 8)
                        continue;
            
                    var pos = new Position(row, col);
                    var localPiece = board.GetPiece(pos);

                    if (localPiece?.Color == piece.Color)
                        continue;
            
                    positions.Add(pos);
                }
                break;
            }
            case PieceType.Rook or PieceType.Queen or PieceType.Bishop:
            {
                foreach (var (rowDir, colDir) in directions)
                {
                    var row = piecePosition.Value.Row + rowDir;
                    var col = piecePosition.Value.Column + colDir;

                    while (row is >= 0 and < 8 && col is >= 0 and < 8)
                    {
                        var pos = new Position(row, col);
                        var localPiece = board.GetPiece(pos);

                        if (localPiece is not null && localPiece.Color != piece.Color)
                        {
                            positions.Add(pos);
                            break;
                        }
                
                        positions.Add(pos);
                        row += rowDir;
                        col += colDir;
                    }
                }
                break;
            }
        }
        
        return positions;
    }
}