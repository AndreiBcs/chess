using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Entities.Pieces.Types;

public class King : Piece, IMoveTracker
{
    public King(Color color, bool hasMoved) : base(color)
    {
        HasMoved = hasMoved;
    }

    public override PieceType Type => PieceType.King;
    public bool HasMoved { get; private set; }
    public void MarkAsMoved()
    {
        HasMoved = true;
    }

    public override IEnumerable<Position> GetPossiblePositions(IReadOnlyBoard board, Position from)
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
            
            var pos = new Position(row, col);
            var piece = board.GetPiece(pos);

            if (piece?.Color == Color)
                continue;
            
            possiblePositions.Add(pos);
        }
        
        if (!HasMoved && from.Column == 4)
        {
            var pos = new Position(from.Row, from.Column);
            
            // kingside castling
            var kingSideRook =
                board.GetPiece(pos with {Column = from.Column + 3});

            if (kingSideRook is Rook {HasMoved: false} &&
                kingSideRook.Color == Color &&
                board.GetPiece(pos with {Column = from.Column + 1}) is null &&
                board.GetPiece(pos with {Column = from.Column + 2}) is null)
            {
                possiblePositions.Add(pos with {Column = from.Column + 2});
            }

            // queenside castling
            var queenSideRook =
                board.GetPiece(pos with {Column = from.Column - 4});

            if (queenSideRook is Rook {HasMoved: false} &&
                queenSideRook.Color == Color &&
                board.GetPiece(pos with {Column = from.Column - 1}) is null &&
                board.GetPiece(pos with {Column = from.Column - 2}) is null &&
                board.GetPiece(pos with {Column = from.Column - 3}) is null)
            {
                possiblePositions.Add(pos with {Column = from.Column - 2});
            }
        }
        
        return possiblePositions;
    }

    public override Piece Copy()
    {
        return new King(Color, HasMoved);
    }
}