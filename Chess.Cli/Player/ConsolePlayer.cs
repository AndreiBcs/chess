using chess;
using Chess.Cli.Presentation;
using chess.Game;
using chess.Moves;

namespace Chess.Cli.Player;

public sealed class ConsolePlayer : chess.Player.Player
{
    public ConsolePlayer(Color color) : base(color)
    {
    }

    public override async Task<Move> GetMoveAsync(GameSnapshot snapshot, MoveResult? previousResult)
    {
        if (previousResult == MoveResult.Invalid)
            ConsoleInteraction.ShowMoveError();
            
        return await ConsoleInteraction.ReadMove();
    }
}