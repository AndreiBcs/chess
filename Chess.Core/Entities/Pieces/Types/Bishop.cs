using chess.Entities.Board;

namespace chess.Entities.Pieces.Types;

public class Bishop : Piece
{
    public override PieceType Type => PieceType.Bishop;

    public override IEnumerable<Position> GetLegalMoves(Board.Board board, Position from)
    {
        throw new NotImplementedException();
    }
}