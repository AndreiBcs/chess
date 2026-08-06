namespace chess.Entities;

public abstract class Piece
{
    public Color Color { get; init; }
    
    public abstract PieceType Type { get; init; }
    public abstract char LetterId { get; init; }
    public abstract char Icon { get; init; } // nu merge
    public abstract int Points { get; init; }

    public abstract bool[][] GetPossibleMoves(Board board);
}

public enum  PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}