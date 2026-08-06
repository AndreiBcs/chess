namespace chess.Entities.Pieces;

public class Pawn : Piece
{
    public override PieceType Type { get; set; } =  PieceType.Pawn;
    public override char Icon { get; set; }
    public override char LetterId { get; set; }
    public override byte Points { get; set; } = 1;
    
    public override bool[][] GetPossibleMoves(Board board)
    {
        throw new NotImplementedException();
    }
}