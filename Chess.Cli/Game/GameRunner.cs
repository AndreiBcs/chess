using chess;
using Chess.Cli.Arguments;
using Chess.Cli.Player;
using Chess.Cli.Presentation;
using Chess.Engine;

namespace Chess.Cli.Game;

public class GameRunner
{
    private readonly BoardRenderer _renderer;
    private readonly ConsoleInteraction _interaction;
    private readonly chess.Game.OldGame _oldGame;

    public GameRunner(CliArguments.ParsedOptions options)
    {
        _renderer = new BoardRenderer();
        _interaction = new ConsoleInteraction();

        var userColor = options.PlayerColor;
        var engineColor = userColor == Color.White?
            Color.Black : 
            Color.White;

        var engineType = options.Engine;
        var elo = options.Elo;
        
        var player1 = new ConsolePlayer(userColor);
        var player2 = new EnginePlayer(engineColor, engineType);
        _ = player2.Uci.StartEngine();
        _ = player2.Uci.SetElo(elo);
        _ = player2.Uci.NewGame();
        
        _oldGame = new chess.Game.OldGame(player1, player2);
    }

    public async Task Run()
    {
        await foreach (var snapshot in _oldGame.GameLoop())
        {
            _renderer.Render(snapshot);
        }
    }
}