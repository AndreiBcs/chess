namespace chess.Entities.Pieces;

public class Knight : Piece
{
    public override PieceType Type { get; init; } = PieceType.Knight;
    public override char LetterId { get; init; }
    public override char Icon { get; init; }
    public override int Points { get; init; } = 3;
    public override bool[][] GetPossibleMoves(Board board)
    {
        throw new NotImplementedException();
    }
}