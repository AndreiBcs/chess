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
                _ => '?'
            };
            
            return Color == Color.White ? letter : char.ToLower(letter);
        }
    }
    public abstract (int Row, int Column)[] GetMoveDirections();
    public abstract (int Row, int Column)[] GetAttackDirections();
    // public abstract IEnumerable<Position> GetPossiblePositions(Board.Board board, Position from);
    // public abstract IEnumerable<Position> GetAttackPositions(Board.Board board, Position from);
}