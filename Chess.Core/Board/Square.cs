using chess.Pieces;

namespace chess.Board;

public record struct Square
{
    public Color Color { get; init; }  
    public Position Position { get; init; }
    public Piece? Piece { get; set; }
};