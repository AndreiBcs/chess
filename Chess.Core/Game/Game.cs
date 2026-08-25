using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Player;
using chess.Game.GameState;

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
    // TODO add en-passant & castling info

    public void StartGame()
    {
        Board.InitializeBoard();
        GameLoop();
    }

    private void GameLoop()
    {
        while (!IsOver)
        {
            var currentPlayer = CurrentTurn == Color.White
                ? PlayerWhite
                : PlayerBlack;

            var snapshot = CreateSnapshot();

            var move = currentPlayer.GetMoveAsync(snapshot);

            // TODO validate move + game state update
        }
    }

    private GameSnapshot CreateSnapshot()
    {
        return new GameSnapshot(CurrentTurn, Board, FullMoveCounter, HalfMoveCounter);
    }
    
}
