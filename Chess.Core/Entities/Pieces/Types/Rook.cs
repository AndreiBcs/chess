using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Entities.Pieces.Types;

public class Rook : Piece, IMoveTracker
{
    public Rook(Color color, bool hasMoved = false) : base(color)
    {
        HasMoved = hasMoved;
    }

    public Rook(Color color) : this(color, false)
    {
    }

    public override PieceType Type => PieceType.Rook;
    public bool HasMoved { get; private set; }
    public void MarkAsMoved()
    {
        HasMoved = true;
    }

    public override IEnumerable<Position> GetPossiblePositions(IReadOnlyBoard board, Position from)
    {
        var possiblePositions = new List<Position>();
        
        var directions = new (int row, int column)[]
        {
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

    public override Piece Copy()
    {
        return new Rook(Color, HasMoved);
    }
}