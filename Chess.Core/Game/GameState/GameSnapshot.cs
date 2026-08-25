using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Game.GameState;

public class GameSnapshot
{
    public readonly Color CurrentTurn;
    public readonly Board Board;
    public readonly int FullMoveCounter;
    public readonly int HalfMoveCounter;
    // TODO add en-passant & castling info

    public GameSnapshot(Color currentTurn, Board board, int fullMoveCounter, int halfMoveCounter)
    {
        CurrentTurn = currentTurn;
        Board = board;
        FullMoveCounter = fullMoveCounter;
        HalfMoveCounter = halfMoveCounter;
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