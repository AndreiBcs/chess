namespace chess.Entities;

public class Board
{
    public List<Piece> pieces { get; set; }
    public List<Square> squares { get; set; }
}

public record Square
{
    public SquareColor Color { get; set; }
    public string Position { get; set; }
}

public enum SquareColor
{
    White,
    Black
}

