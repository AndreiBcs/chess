using Chess.Cli.Presentation;
using chess.Entities.Board;
using chess.Entities.Common;
using chess.Game.GameState;

namespace Chess.Cli.Player;

public class ConsolePlayer : chess.Entities.Player.Player
{
    private readonly ConsoleInteraction _interaction;
    public ConsolePlayer(Color color) : base(color)
    {
        _interaction = new ConsoleInteraction();
    }

    public override async Task<Move> GetMoveAsync(GameSnapshot snapshot, MoveResult? previousResult)
    {
        if (previousResult is not null && previousResult != MoveResult.Valid)
            _interaction.ShowMoveError(previousResult);
            
        return await _interaction.ReadMove(snapshot);
    }
}