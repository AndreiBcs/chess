using chess.Entities.Board;

namespace chess.Entities.Pieces.Types;

public class Queen : Piece
{
    public override required PieceType Type { get; init; } = PieceType.Queen;
    public override required char LetterId { get; init; }
    public override IEnumerable<Position> GetLegalMoves(Board.Board board, Position from)
    {
        throw new NotImplementedException();
    }
}