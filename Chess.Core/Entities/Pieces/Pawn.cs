namespace chess.Entities.Pieces;

public class Pawn : Piece
{
    public override PieceType Type { get; init; } =  PieceType.Pawn;
    public override char Icon { get; init; }
    public override char LetterId { get; init; }
    public override int Points { get; init; } = 1;
    
    public override bool[][] GetPossibleMoves(Board board)
    {
        throw new NotImplementedException();
    }
}