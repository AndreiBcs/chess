namespace chess.Entities.Pieces;

public class Rook : Piece
{
    public override PieceType Type { get; set; } = PieceType.Rook;
    public override char LetterId { get; set; }
    public override char Icon { get; set; }
    public override byte Points { get; set; } = 5;
    public override bool[][] GetPossibleMoves(Board board)
    {
        throw new NotImplementedException();
    }
}