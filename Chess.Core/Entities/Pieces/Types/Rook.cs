using chess.Entities.Board;

namespace chess.Entities.Pieces.Types;

public class Rook : Piece
{
    public override required PieceType Type { get; init; } = PieceType.Rook;
    public override required char LetterId { get; init; }
    public override IEnumerable<Position> GetLegalMoves(Board.Board board, Position from)
    {
        throw new NotImplementedException();
    }

    public bool HasMoved { get; set; } = false;
}