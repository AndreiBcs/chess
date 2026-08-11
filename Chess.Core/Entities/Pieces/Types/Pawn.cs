using chess.Entities.Board;

namespace chess.Entities.Pieces.Types;

public class Pawn : Piece
{
    public override PieceType Type => PieceType.Pawn;

    public override IEnumerable<Position> GetLegalMoves(Board.Board board, Position from)
    {
        throw new NotImplementedException();
    }

    public bool HasMoved { get; set; } = false;
}