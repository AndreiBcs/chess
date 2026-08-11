using chess.Entities.Board;

namespace chess.Entities.Pieces.Types;

public class Knight : Piece
{
    public override required PieceType Type { get; init; } = PieceType.Knight;
    public override required char LetterId { get; init; }
    public override IEnumerable<Position> GetLegalMoves(Board.Board board, Position from)
    {
        throw new NotImplementedException();
    }
}