using chess.Pieces;

namespace chess.Board;

public readonly record struct Square
{
    public Color Color { get; init; }  
    public Position Position { get; init; }
    public Piece? Piece { get; init; }
};