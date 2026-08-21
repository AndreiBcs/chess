using chess.Entities.Board;

namespace chess.Entities.Pieces.Types;

public class King : Piece
{
    public override PieceType Type => PieceType.King;

    public override IEnumerable<Position> GetPossiblePositions(Board.Board board, Position from)
    {
        var possiblePositions = new List<Position>();
        
        var moves = new (int row, int column)[]
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

        foreach (var (rowOffset, colOffset) in moves)
        {
            var row = from.Row + rowOffset;
            var col = from.Column + colOffset;

            if (row is < 0 or >= 8 || col is < 0 or >= 8)
                continue;
            
            var piece = board.Squares[row, col].Piece;

            if (piece?.Owner == Owner)
                continue;
            
            possiblePositions.Add(new Position(row, col));
        }
        
        return possiblePositions;
    }

    public bool HasMoved { get; set; } = false;
}