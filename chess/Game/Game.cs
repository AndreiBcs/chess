using chess.Entities;

namespace chess.Game;

public class Game
{
    Player Player { get; set; }
    Player Robot { get; set; }
    Board Board { get; set; }
}