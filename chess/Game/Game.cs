using chess.Entities;

namespace chess.Game;

public class Game
{
    Player PlayerWhite { get; set; }
    Player PlayerBlack { get; set; }
    Board Board { get; set; }

    public void StartGame()
    {
        PlayerWhite = new Player();
        PlayerBlack = new Player();
        Board = new Board();

        PlayerWhite.InitializePlayer(PlayerColor.PlayerWhite);
        PlayerBlack.InitializePlayer(PlayerColor.PlayerBlack);
        Board.InitializeStartingBoard(PlayerWhite, PlayerBlack);
    }
}