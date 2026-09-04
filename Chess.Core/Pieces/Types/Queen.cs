namespace chess.Pieces.Types;

public sealed record Queen(Color Color) : Piece(Color)
{
    public override PieceType Type => PieceType.Queen;

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