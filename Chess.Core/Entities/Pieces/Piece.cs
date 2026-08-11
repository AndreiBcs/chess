using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Entities.Pieces;

public abstract class Piece
{
    public required Player.Player Owner { get; init; }
    public Color Color => Owner.Color;
    public bool IsCaptured { get; set; } = false;
    //public Position Position { get; set; }
    public abstract required PieceType Type { get; init; }
    public abstract required char LetterId { get; init; }
    public abstract IEnumerable<Position> GetLegalMoves(Board.Board board, Position from);
}