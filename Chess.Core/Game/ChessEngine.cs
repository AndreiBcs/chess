using Chess.Engine;
using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;

namespace chess.Game;

public static class ChessEngine
{
    
    public static void StartEngine(this Engine engine, Difficulty difficulty)
    {
        try
        {
            engine.NewGame();
            engine.SetElo(GetDifficultyElo(difficulty));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static Move GetEngineMove(this Engine engine, Game game)
    {
        try
        {
            var move = engine.GetMove(ToFen(game));

            var from = Position.ParsePosition(move[..2]);
            var to = Position.ParsePosition(move[2..4]);

            PieceType? promotion = move.Length switch
            {
                5 => move[4] switch
                {
                    'q' => PieceType.Queen,
                    'r' => PieceType.Rook,
                    'b' => PieceType.Bishop,
                    'n' => PieceType.King,
                    _ => null
                },
                _ => null
            };
            
            return new Move(from, to,null, promotion);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public static void StopEngine(this Engine engine)
    {
        engine.Dispose();
    }
    
    private static string ToFen(Game game)
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
    
    private static int GetDifficultyElo(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => 1000,
            Difficulty.Normal => 1500,
            Difficulty.Hard => 2000,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };
    }
    
}
