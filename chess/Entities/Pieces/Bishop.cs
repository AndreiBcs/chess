namespace chess.Entities.Pieces;

public class Bishop : Piece
{
    public override PieceType Type { get; set; } = PieceType.Bishop;
    public override char LetterId { get; set; }
    public override char Icon { get; set; }
    public override byte Points { get; set; } = 3;
    public override bool[][] GetPossibleMoves(Board board)
    {
        throw new NotImplementedException();
    }
}