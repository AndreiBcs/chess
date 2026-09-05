using chess;
using Chess.Cli.Arguments;
using Chess.Cli.Player;
using Chess.Cli.Presentation;
using Chess.Engine;

namespace Chess.Cli.Game;

internal sealed class GameRunner : IAsyncDisposable
{
    private readonly chess.Game.Game _game;
    private readonly EnginePlayer _enginePlayer;
    private readonly int _elo;

    public GameRunner(CliArguments.ParsedOptions options)
    {
        var userColor = options.PlayerColor;
        var engineColor = userColor == Color.White?
            Color.Black : 
            Color.White;

        var engineType = options.Engine;
        _elo = options.Elo;
        
        var player1 = new ConsolePlayer(userColor);
        _enginePlayer = new EnginePlayer(engineColor, engineType);
        
        _game = new chess.Game.Game(player1, _enginePlayer);
    }

    public async Task Run()
    {
        await _enginePlayer.Uci.StartEngine();
        await _enginePlayer.Uci.SetElo(_elo);
        await _enginePlayer.Uci.NewGame();

        await foreach (var snapshot in _game.GameLoop())
        {
            BoardRenderer.Render(snapshot);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _enginePlayer.DisposeAsync();
    }
}