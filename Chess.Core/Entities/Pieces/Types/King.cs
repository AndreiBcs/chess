using chess.Entities.Board;

namespace chess.Entities.Pieces.Types;

public class King : Piece
{
    public override PieceType Type => PieceType.King;

    public override IEnumerable<Position> GetLegalMoves(Board.Board board, Position from)
    {
        throw new NotImplementedException();
    }

    public bool HasMoved { get; set; } = false;
}