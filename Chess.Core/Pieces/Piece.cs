namespace chess.Pieces;

public abstract record Piece(Color Color)
{
    public bool HasMoved { get; init; }
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
    public abstract (int Row, int Column)[] GetMoveDirections();
    public abstract (int Rox, int Column)[] GetAttackDirections();
}