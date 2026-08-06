namespace chess.Entities.Pieces;

public class Queen : Piece
{
    public override PieceType Type { get; set; } = PieceType.Queen;
    public override char LetterId { get; set; }
    public override char Icon { get; set; }
    public override byte Points { get; set; } = 9;
    public override bool[][] GetPossibleMoves(Board board)
    {
        throw new NotImplementedException();
    }
}