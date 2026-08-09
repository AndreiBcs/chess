using chess.Entities;

namespace chess.Game;

public class Game
{
    public bool IsOver { get; set; } = false;
    public bool IsUserWhite { get; init; } = true;
    public Difficulty Difficulty { get; set; } = Difficulty.Normal;
    private Player PlayerWhite { get; } = new() { Color = Color.White };
    private Player PlayerBlack { get; } = new() { Color = Color.Black };
    public Board Board { get; } = new();

    public void StartGame()
    {
        PlayerWhite.InitializePlayer();
        PlayerBlack.InitializePlayer();
        Board.InitializeStartingBoard(PlayerWhite, PlayerBlack);
        GameLoop();
    }

    private void GameLoop()
    {
        while (IsOver)
        {
            if (IsUserWhite)
            {
                StartAsWhite();
            }
            else
            {
                StartAsBlack();
            }
        }
    }

    private void StartAsWhite()
    {
        
    }

    private void StartAsBlack()
    {
        
    }

    public void ApplyMove(((int fromRow, int fromCol), (int toRow, int toCol)) move)
    {
        var (fromRow, fromCol) = move;
        // TODO
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}