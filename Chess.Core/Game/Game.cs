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
    public Player PlayerWhite { get; } = new() { Color = Color.White };
    public Player PlayerBlack { get; } = new() { Color = Color.Black };
    public Board Board { get; } = new();
    public int FullMoveCounter { get; set; } = 1; // increase after black's turn
    public int HalfMoveCounter { get; set; } = 0; // back at 0 after a capture or pawn advance
    public Move CurrentMove { get; set; }
    public Move PreviousMove { get; set; }
    public Engine ChessEngine { get; } = new();

    public void StartGame()
    {
        PlayerWhite.InitializePlayer();
        PlayerBlack.InitializePlayer();
        Board.InitializeBoard(PlayerWhite, PlayerBlack);
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
    
    public void MakeMove(Move move)
    {
        // TODO
    }

    
}
