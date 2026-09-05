namespace chess.Pieces.Types;

public sealed record Knight(Color Color) : Piece(Color)
{
    public override PieceType Type => PieceType.Knight;

    public override (int Row, int Column)[] GetMoveDirections()
    {
        return 
        [
            (-2, -1),
            (-2, 1),
            (-1, -2),
            (-1, 2),
            (1, -2),
            (1, 2),
            (2, -1),
            (2, 1)
        ];
    }

    public override (int Rox, int Column)[] GetAttackDirections()
    {
        return GetMoveDirections();
    }
}