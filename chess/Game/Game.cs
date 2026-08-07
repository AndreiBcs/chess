using chess.Cli;
using chess.Entities;

namespace chess.Game;

public class Game
{
    private Player PlayerWhite { get; } = new() { Color = Color.White };
    private Player PlayerBlack { get; } = new() { Color = Color.Black };
    private Board Board { get; } = new();

    public void StartGame()
    {
        PlayerWhite.InitializePlayer();
        PlayerBlack.InitializePlayer();
        Board.InitializeStartingBoard(PlayerWhite, PlayerBlack);
        //Board.DrawBoard();
        Board.Run();
    }
}