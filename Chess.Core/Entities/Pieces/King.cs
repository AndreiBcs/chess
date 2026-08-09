namespace chess.Entities.Pieces;

public class King : Piece
{
    public override PieceType Type { get; init; } = PieceType.King;
    public override char LetterId { get; init; }
    public override char Icon { get; init; }
    public override int Points { get; init; } = 100; // change this
    public override bool[][] GetPossibleMoves(Board board)
    {
        throw new NotImplementedException();
    }
}