namespace chess.Entities.Pieces;

public class King : Piece
{
    public override PieceType Type { get; set; } = PieceType.King;
    public override char LetterId { get; set; }
    public override char Icon { get; set; }
    public override byte Points { get; set; } = 100; // change this
    public override bool[][] GetPossibleMoves(Board board)
    {
        throw new NotImplementedException();
    }
}