namespace chess.Pieces.Types;

public sealed record King(Color Color) : Piece(Color)
{
    public override PieceType Type => PieceType.King;

    public override (int Row, int Column)[] GetMoveDirections()
    {
        return 
        [
            (-1, -1),
            (-1, 1),
            (1, -1),
            (1, 1),
            (-1, 0),
            (1, 0),
            (0, -1),
            (0, 1)
        ];
    }

    public override (int Rox, int Column)[] GetAttackDirections()
    {
        return GetMoveDirections();
    }
}