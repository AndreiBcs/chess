using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Player;

namespace chess.Game;

public class Game
{
    public bool IsOver { get; set; } = false;
    public Color CurrentTurn { get; set; } = Color.White;
    public Player PlayerWhite { get; } = new() { Color = Color.White };
    public Player PlayerBlack { get; } = new() { Color = Color.Black };
    public Board Board { get; } = new();
    public int FullMoveCounter { get; set; } = 1; // increase after black's turn
    public int HalfMoveCounter { get; set; } = 0; // back at 0 after a capture or pawn advance
    public Move CurrentMove { get; set; }
    public Move PreviousMove { get; set; }

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
    
    public static string ToFen(Game game)
    {
        var fen = "";

        for (var i = 0; i < 8; i++)
        {
            var empty = 0;
            var col = 0;

            while (col < 8)
            {
                if (game.Board.Squares[i, col].Piece is not null)
                {
                    fen += game.Board.Squares[i, col].Piece!.LetterId;
                    col++;
                }
                else
                {
                    while (game.Board.Squares[i, col++].Piece is null)
                    {
                        empty++;
                    }

                    fen += empty.ToString();
                }
            }

            switch (i)
            {
                case < 7:
                    fen += "/";
                    break;
                case 7:
                    fen += " ";
                    break;
            }
        }

        fen += game.CurrentTurn == Color.White ? "w" : "b";
        
        // TODO castling rights
        fen += "-";
        
        // TODO en-passant valid square
        fen += "-";
        
        fen += game.HalfMoveCounter.ToString();
        fen += game.FullMoveCounter.ToString();
        
        return fen;
    }
    
}
