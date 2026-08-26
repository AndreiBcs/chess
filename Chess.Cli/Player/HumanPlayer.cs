using chess.Entities.Board;
using chess.Entities.Common;
using chess.Game.GameState;

namespace Chess.Cli.Player;

public class HumanPlayer : chess.Entities.Player.Player
{
    public HumanPlayer(Color color) : base(color)
    {
    }

    public override Task<Move> GetMoveAsync(GameSnapshot snapshot)
    {
        throw new NotImplementedException();
    }
}