using chess.Entities;

namespace chess.Game;

public class Game
{
    private Player PlayerWhite { get; set; } = new Player();
    private Player PlayerBlack { get; set; } = new Player();
    private Board Board { get; set; } = new Board();

    public void StartGame()
    {
        PlayerWhite = new Player();
        PlayerBlack = new Player();
        Board = new Board();

        PlayerWhite.InitializePlayer(PlayerColor.PlayerWhite);
        PlayerBlack.InitializePlayer(PlayerColor.PlayerBlack);
        Board.InitializeStartingBoard(PlayerWhite, PlayerBlack);
        Board.PrintBoard();
    }
}