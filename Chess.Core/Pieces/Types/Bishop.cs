namespace chess.Pieces.Types;

public sealed record Bishop(Color Color) : Piece(Color)
{
    public override PieceType Type => PieceType.Bishop;

    public override (int Row, int Column)[] GetMoveDirections()
    {
        return 
        [
            (-1, -1),
            (-1, 1),
            (1, -1),
            (1, 1)
        ];
    }

    public override (int Rox, int Column)[] GetAttackDirections()
    {
        return GetMoveDirections();
    }
}