using Chess.Engine;
using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Player;

namespace chess.Game;

public class Game
{
    public bool IsOver { get; set; } = false;
    public Color UserColor { get; init; } = Color.White;
    public Color CurrentTurn { get; set; } = Color.White;
    public Difficulty Difficulty { get; set; } = Difficulty.Normal;
    private Player PlayerWhite { get; } = new() { Color = Color.White };
    private Player PlayerBlack { get; } = new() { Color = Color.Black };
    public Board Board { get; } = new();
    private Engine ChessEngine { get; } = new();

    public void StartGame()
    {
        PlayerWhite.InitializePlayer();
        PlayerBlack.InitializePlayer();
        Board.InitializeBoard(PlayerWhite, PlayerBlack);
        GameLoop();
    }

    private void GameLoop() // TODO
    {
        // while (!IsOver)
        // {
        //     if (CurrentTurn == Color.White)
        //     {
        //         
        //     }
        //     else
        //     {
        //         return;
        //     }
        // }
    }
    
    public void MakeMove(Move move)
    {
        // TODO
    }
    
    public void SetDifficulty(Difficulty difficulty)
    {
        var elo = difficulty switch
        {
            Difficulty.Easy => 1000,
            Difficulty.Normal => 1500,
            Difficulty.Hard => 2000,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}