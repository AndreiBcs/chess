namespace chess.Pieces.Types;

public sealed record Pawn(Color Color) : Piece(Color)
{
    public override PieceType Type => PieceType.Pawn;

    public override (int Row, int Column)[] GetMoveDirections()
    {
        var direction = Color == Color.White ? -1 : 1;
        
        if (!HasMoved)
        {
            return 
            [
                (direction, 0),
                (direction * 2, 0)
            ];
        }

        return [(direction, 0)];
    }

    public override (int Rox, int Column)[] GetAttackDirections()
    {
        var direction = Color == Color.White ? -1 : 1;
        
        return 
        [
            (direction, 1),
            (direction, -1)
        ];
    } 
}