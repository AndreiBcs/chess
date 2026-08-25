using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Player;

namespace chess.Game;

public class Game
{
    public Game(Player playerWhite, Player playerBlack)
    {
        PlayerWhite = playerWhite;
        PlayerBlack = playerBlack;
    }

    public bool IsOver { get; set; } = false;
    public Color CurrentTurn { get; set; } = Color.White;
    public Player PlayerWhite { get; }
    public Player PlayerBlack { get; }
    public Board Board { get; } = new();
    public int FullMoveCounter { get; set; } = 1; // increase after black's turn
    public int HalfMoveCounter { get; set; } = 0; // back at 0 after a capture or pawn advance
    public Move CurrentMove { get; set; }
    public Move PreviousMove { get; set; }

    public void StartGame()
    {
        // PlayerWhite.InitializePlayer();
        // PlayerBlack.InitializePlayer();
        Board.InitializeBoard();
        GameLoop();
    }

    private void GameLoop()
    {
        while (!IsOver)
        {
            if (CurrentTurn == Color.White)
            {
                
            }
            else
            {
                return;
            }
        }
    }
    
}
