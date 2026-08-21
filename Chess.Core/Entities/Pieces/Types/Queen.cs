using chess.Entities.Board;

namespace chess.Entities.Pieces.Types;

public class Queen : Piece
{
    public override PieceType Type => PieceType.Queen;

    public override IEnumerable<Position> GetPossiblePositions(Board.Board board, Position from)
    {
        var legalMoves = new List<Position>();
        
        var directions = new (int row, int column)[]
        {
            (-1, -1), // up left
            (-1, 1), // up right
            (1, -1), // down left
            (1, 1), // down right
            (-1, 0), // up
            (1, 0), // down
            (0, -1), // left
            (0, 1) // right
        };

        foreach (var (rowDir, colDir) in directions)
        {
            var row = from.Row + rowDir;
            var col = from.Column + colDir;

            while (row is >= 0 and < 8 && col is >= 0 and < 8)
            {
                var pos = new Position(row, col);
                var piece = board.Squares[pos.Row, pos.Column].Piece;

                if (piece is not null)
                {
                    if (piece.Owner != Owner)
                    {
                        legalMoves.Add(pos);
                    }

                    break;
                }
                legalMoves.Add(pos);
                
                row += rowDir;
                col += colDir;
            }
        }
        
        return legalMoves;
    }
}