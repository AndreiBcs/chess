using chess.Entities.Board;

namespace chess.Entities.Pieces.Types;

public class Queen : Piece
{
    public override PieceType Type => PieceType.Queen;

    public override IEnumerable<Position> GetLegalMoves(Board.Board board, Position from)
    {
        throw new NotImplementedException();
    }
}