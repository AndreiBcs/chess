using chess.Board;

namespace chess.Pieces.Types;

public class Bishop : Piece
{
    public Bishop(Color color) : base(color)
    {
    }

    public override PieceType Type => PieceType.Bishop;

    public override IEnumerable<Position> GetPossiblePositions(IReadOnlyBoard board, Position from)
    {
        var possiblePositions = new List<Position>();
        
        var directions = new (int row, int column)[]
        {
            (-1, -1), // up left
            (-1, 1), // up right
            (1, -1), // down left
            (1, 1) // down right
        };

        foreach (var (rowDir, colDir) in directions)
        {
            var row = from.Row + rowDir;
            var col = from.Column + colDir;

            while (row is >= 0 and < 8 && col is >= 0 and < 8)
            {
                var pos = new Position(row, col);
                var piece = board.GetPiece(pos);

                if (piece is not null)
                {
                    if (piece.Color != Color)
                    {
                        possiblePositions.Add(pos);
                    }

                    break;
                }
                possiblePositions.Add(pos);
                
                row += rowDir;
                col += colDir;
            }
        }
        
        return possiblePositions;
    }

    public override IEnumerable<Position> GetAttackPositions(IReadOnlyBoard board, Position from)
    {
        return GetPossiblePositions(board, from);
    }

    public override (int Row, int Column)[] GetMoveDirections()
    {
        return 
        [
            (-1, -1),
            (-1, 1),
            (1, -1),
            (1, 1)
        ];
    }

    public override (int Rox, int Column)[] GetAttackDirections()
    {
        return GetMoveDirections();
    }

    public override Piece Copy()
    {
        return new Bishop(Color);
    }
}