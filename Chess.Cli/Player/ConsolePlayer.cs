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

    public override async Task<Move> GetMoveAsync(
        GameSnapshot snapshot,
        MoveStatus? moveStatus,
        CancellationToken cancellationToken = default)
    {
        if (moveStatus is not null &&
            moveStatus.Value.MoveResult == MoveResult.Invalid)
        {
            ConsoleInteraction.ShowMoveError(moveStatus.Value.InvalidMoveReason!);
        }
            
        return await ConsoleInteraction.ReadMove(snapshot);
    }
}