using chess.Entities.Pieces;

namespace chess.Entities;

public class Player
{
    public PlayerColor Color { get; set; }
    public byte Score { get; set; }
    public List<Pawn> Pawns { get; set; }
    public List<Rook> Rooks { get; set; }
    public List<Knight> Knights { get; set; }
    public List<Bishop> Bishops { get; set; }
    public Queen Queen { get; set; }
    public King King { get; set; }
}

public enum PlayerColor
{
    // unrelated to piece color or square color
    PlayerWhite,
    PlayerBlack
}