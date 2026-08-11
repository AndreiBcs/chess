using chess.Entities.Board;

namespace chess.Entities.Pieces.Types;

public class Knight : Piece
{
    public override PieceType Type => PieceType.Knight;

    public override IEnumerable<Position> GetLegalMoves(Board.Board board, Position from)
    {
        throw new NotImplementedException();
    }
}