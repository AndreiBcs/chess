namespace chess.Entities.Pieces;

public class Queen : Piece
{
    public override PieceType Type { get; init; } = PieceType.Queen;
    public override char LetterId { get; init; }
    public override char Icon { get; init; }
    public override int Points { get; init; } = 9;
    public override bool[][] GetPossibleMoves(Board board)
    {
        throw new NotImplementedException();
    }
}