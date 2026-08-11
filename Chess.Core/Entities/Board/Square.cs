using chess.Entities.Common;
using chess.Entities.Pieces;

namespace chess.Entities.Board;

public record struct Square
{
    public Color Color { get; init; }  
    public Position Position { get; init; }
    public Piece? Piece { get; set; }
};