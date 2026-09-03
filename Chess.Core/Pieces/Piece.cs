using chess.Board;

namespace chess.Pieces;

public abstract class Piece
{
    protected Piece(Color color)
    {
        Color = color;
    }

    public Color Color { get; init; }
    public bool IsCaptured { get; private set; }
    public bool HasMoved { get; init; }
    public void MarkAsCaptured()
    {
        IsCaptured = true;
    }
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
    public abstract IEnumerable<Position> GetPossiblePositions(IReadOnlyBoard board, Position from);
    public abstract IEnumerable<Position> GetAttackPositions(IReadOnlyBoard board, Position from);
    public abstract (int Row, int Column)[] GetMoveDirections();
    public abstract (int Rox, int Column)[] GetAttackDirections();
    public abstract Piece Copy();
}