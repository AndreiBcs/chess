using Chess.Cli.Visuals.Interactions;
using chess.Entities.Board;
using chess.Entities.Common;
using chess.Game.GameState;

namespace Chess.Cli.Player;

public class HumanPlayer : chess.Entities.Player.Player
{
    private readonly UserInteraction _interaction;
    public HumanPlayer(Color color) : base(color)
    {
        _interaction = new UserInteraction();
    }

    public override async Task<Move> GetMoveAsync(GameSnapshot snapshot, MoveResult? previousResult)
    {
        if (previousResult is not null && previousResult != MoveResult.Valid)
            _interaction.ShowMoveError(previousResult);
            
        return await _interaction.ReadMove(snapshot);
    }
}