using chess.Board;

namespace chess.Pieces.Types;

public class Knight : Piece
{
    public Knight(Color color) : base(color)
    {
    }

    public override PieceType Type => PieceType.Knight;

    public override IEnumerable<Position> GetPossiblePositions(IReadOnlyBoard board, Position from)
    {
        var possiblePositions = new List<Position>();
        
        var moves = new (int row, int column)[]
        {
            (-2, -1),
            (-2, 1),
            (-1, -2),
            (-1, 2),
            (1, -2),
            (1, 2),
            (2, -1),
            (2, 1)
        };

        foreach (var (rowOffset, colOffset) in moves)
        {
            var row = from.Row + rowOffset;
            var col = from.Column + colOffset;

            if (row is < 0 or >= 8 || col is < 0 or >= 8)
                continue;
            
            var pos = new Position(row, col);
            var piece = board.GetPiece(pos);

            if (piece?.Color == Color)
                continue;
            
            possiblePositions.Add(pos);
        }
        
        return possiblePositions;
    }

    public override IEnumerable<Position> GetAttackPositions(IReadOnlyBoard board, Position from)
    {
        return GetPossiblePositions(board, from);
    }

    public override Piece Copy()
    {
        return new Knight(Color);
    }
}