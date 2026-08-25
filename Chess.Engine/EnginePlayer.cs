using Chess.Engine.Configs;
using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Entities.Player;
using chess.Game.GameState;

namespace Chess.Engine;

public class EnginePlayer : Player
{
    private readonly Configs.Engine _engine;

    public EnginePlayer(ChessEngineDifficulty difficulty)
    {
        _engine = new Configs.Engine();
        _engine.Elo = difficulty.GetDifficultyElo();
    }
    
    public override Task<Move> GetMoveAsync(GameSnapshot snapshot)
    {
        try
        {
            try
            {
                var move = _engine.GetMove(ToFen(snapshot));

                var from = Position.ParsePosition(move[..2]);
                var to = Position.ParsePosition(move[2..4]);

                PieceType? promotion = move.Length switch
                {
                    5 => move[4] switch
                    {
                        'q' => PieceType.Queen,
                        'r' => PieceType.Rook,
                        'b' => PieceType.Bishop,
                        'n' => PieceType.Knight,
                        _ => null
                    },
                    _ => null
                };
            
                return Task.FromResult(new Move(from, to, promotion));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        catch (Exception exception)
        {
            return Task.FromException<Move>(exception);
        }
    }
    
    private static string ToFen(GameSnapshot snapshot)
    {
        var fen = "";

        for (var i = 0; i < 8; i++)
        {
            var empty = 0;
            var col = 0;

            while (col < 8)
            {
                if (snapshot.Board.GetPiece(new Position(i, col)) is not null)
                {
                    fen += snapshot.Board.GetPiece(new Position(i, col))!.LetterId;
                    col++;
                }
                else
                {
                    while (snapshot.Board.GetPiece(new Position(i, col++)) is null)
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

        fen += snapshot.CurrentTurn == Color.White ? "w" : "b";
        
        // TODO castling rights
        fen += "-";
        
        // TODO en-passant valid square
        fen += "-";
        
        fen += snapshot.HalfMoveCounter.ToString();
        fen += " ";
        fen += snapshot.FullMoveCounter.ToString();
        
        return fen;
    }
}