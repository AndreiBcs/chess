using chess;
using chess.Game;
using chess.Moves;

namespace Chess.Api.Player;

public class HttpPlayer : chess.Player.Player
{
    public HttpPlayer(Color color) : base(color)
    {
    }

    public override Task<Move> GetMoveAsync(GameSnapshot snapshot, MoveResult? previousResult)
    {
        throw new NotImplementedException();
    }
}