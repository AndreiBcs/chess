namespace chess.Entities.Pieces;

public class Rook : Piece
{
    public override PieceType Type { get; init; } = PieceType.Rook;
    public override char LetterId { get; init; }
    public override char Icon { get; init; }
    public override int Points { get; init; } = 5;

    public override bool[][] GetPossibleMoves(Board board)
    {
        throw new NotImplementedException();
    }
}