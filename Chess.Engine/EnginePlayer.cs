using chess.Entities.Board;
using chess.Entities.Pieces;
using chess.Entities.Player;
using chess.Game;
using chess.Game.GameState;

namespace Chess.Engine;

public class EnginePlayer : Player
{
    private readonly Engine _engine;

    public EnginePlayer(ChessEngineDifficulty difficulty)
    {
        _engine = new Engine();
        _engine.Elo = difficulty.GetDifficultyElo();
    }
    
    public override Task<Move> GetMoveAsync(GameSnapshot snapshot)
    {
        try
        {
            var move = _engine.GetMove(snapshot.ToFen());

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
            
            return new Move(from, to, promotion);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
}