namespace chess.Entities;

public abstract class Piece
{
    public PieceColor Color { get; set; }
    public string Position { get; set; }
    public byte Index { get; set; }
    
    public abstract PieceType Type { get; set; }
    public abstract char Letter { get; set; }
    public abstract char Icon { get; set; }
    public abstract byte Points { get; set; }

    public abstract bool[][] GetPossibleMoves(Board board);
}

public enum PieceColor
{
    White,
    Black
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