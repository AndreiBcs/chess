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
    private int FullMoveCounter { get; set; } = 1; // increase after black's turn
    private int HalfMoveCounter { get; set; } = 0; // back at 0 after a capture or pawn advance
    public Move CurrentMove { get; set; }
    public Move PreviousMove { get; set; }
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

    public string ToFen()
    {
        var fen = "";

        for (var i = 0; i < 8; i++)
        {
            var empty = 0;
            var col = 0;

            while (col < 8)
            {
                if (Board.Squares[i, col].Piece is not null)
                {
                    fen += Board.Squares[i, col].Piece!.LetterId;
                    col++;
                }
                else
                {
                    while (Board.Squares[i, col++].Piece is null)
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

        fen += CurrentTurn == Color.White ? "w" : "b";
        
        // TODO castling rights
        fen += "-";
        
        // TODO en-passant valid square
        fen += "-";
        
        fen += HalfMoveCounter.ToString();
        fen += FullMoveCounter.ToString();
        
        return fen;
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}