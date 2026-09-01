using chess;
using Chess.Cli.Presentation;
using chess.Game;
using chess.Moves;

namespace Chess.Cli.Player;

public class ConsolePlayer : chess.Player.Player
{
    private readonly ConsoleInteraction _interaction;
    public ConsolePlayer(Color color) : base(color)
    {
        _interaction = new ConsoleInteraction();
    }

    public override async Task<Move> GetMoveAsync(GameSnapshot snapshot, MoveResult? previousResult)
    {
        if (previousResult == MoveResult.Invalid)
            _interaction.ShowMoveError(previousResult);
            
        return await _interaction.ReadMove(snapshot);
    }
}