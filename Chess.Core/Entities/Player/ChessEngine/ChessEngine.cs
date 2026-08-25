using Chess.Engine;
using chess.Entities.Board;
using chess.Entities.Pieces;

namespace chess.Entities.Player.ChessEngine;

public class ChessEngine : IPlayerTypeActions
{
    private readonly Engine _engine;

    public ChessEngine(ChessEngineDifficulty difficulty)
    {
        _engine = new Engine();
        _engine.Elo = difficulty.GetDifficultyElo();
    }
    public Move GetMove()
    {
        try
        {
            var move = _engine.GetMove(ToFen());

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
    
}
