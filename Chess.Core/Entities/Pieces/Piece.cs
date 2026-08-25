using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Entities.Pieces;

public abstract class Piece
{
    protected Piece(Color color)
    {
        Color = color;
    }

    public Color Color { get; init; }
    public bool IsCaptured { get; set; } = false;
    public abstract PieceType Type { get; }
    public char LetterId
    {
        get
        {
            var letter = Type switch
            {
                PieceType.Pawn => 'P',
                PieceType.Rook => 'R',
                PieceType.Knight => 'N',
                PieceType.Bishop => 'B',
                PieceType.Queen => 'Q',
                PieceType.King => 'K',
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return Color == Color.White ? letter : char.ToLower(letter);
        }
    }
    // TODO change to IReadOnlyBoard
    public abstract IEnumerable<Position> GetPossiblePositions(Board.Board board, Position from);
}