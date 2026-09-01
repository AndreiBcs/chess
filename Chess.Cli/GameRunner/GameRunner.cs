using Chess.Cli.CliArgs;
using Chess.Cli.Player;
using Chess.Cli.Visuals.Board;
using Chess.Engine;
using chess.Entities.Common;
using chess.Game;
using UserInteraction = Chess.Cli.Visuals.Interactions.UserInteraction;

namespace Chess.Cli.GameRunner;

public class GameRunner
{
    private readonly BoardRender _render;
    private readonly UserInteraction _interaction;
    private readonly Game _game;

    public GameRunner(CliArguments.ParsedOptions options)
    {
        _render = new BoardRender();
        _interaction = new UserInteraction();

        var userColor = options.PlayerColor;
        var engineColor = userColor == Color.White?
            Color.Black : 
            Color.White;

        var engineType = options.Engine;
        
        var player1 = new HumanPlayer(userColor);
        var player2 = new EnginePlayer(engineColor, engineType);
        
        _game = new Game(player1, player2);
    }

    public async Task Run()
    {
        await foreach (var snapshot in _game.GameLoop())
        {
            _render.Render(snapshot);
        }
    }
}