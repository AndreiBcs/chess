using chess.Pieces;

namespace chess;

public enum ChessMode
{
    Normal,
    Clash
}

public readonly record struct PieceCustomPosition(
    PieceType PieceType,
    int PositionIndex)
{
    public static bool Validate(List<PieceCustomPosition> pieces)
    {
        return true;
    }
}