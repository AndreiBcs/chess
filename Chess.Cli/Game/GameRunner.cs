using chess;
using Chess.Cli.Arguments;
using Chess.Cli.Player;
using Chess.Cli.Presentation;
using Chess.Cli.Services;
using Chess.Engine;
using chess.Pgn;

namespace Chess.Cli.Game;

internal sealed class GameRunner : IAsyncDisposable
{
    private chess.Game.Game _game;
    private readonly EnginePlayer _enginePlayer;
    private readonly ConsolePlayer _consolePlayer;
    private readonly Color _playerColor;
    private readonly int _elo;
    private readonly bool _textRender;

    public GameRunner(Options options)
    {
        _playerColor = options.PlayerColor;
        
        var engineColor = _playerColor == Color.White?
            Color.Black : 
            Color.White;

        var engineType = options.Engine;
        
        _elo = options.Elo;
        
        _consolePlayer = new ConsolePlayer(_playerColor);
        _enginePlayer = new EnginePlayer(engineColor, engineType);
        
        _textRender = options.TextRender;
        
        _game = new chess.Game.Game(_consolePlayer, _enginePlayer);
    }

    public async Task Run()
    {
        await _enginePlayer.Uci.StartEngine();
        await _enginePlayer.Uci.SetElo(_elo);

        while (true)
        {
            await _enginePlayer.Uci.NewGame();
            
            await foreach (var snapshot in _game.GameLoop())
            {
                BoardRenderer.Render(snapshot, _playerColor, _textRender);
            }

            if (ConsoleInteraction.SaveGameToFile())
            {
                var pgn = PgnWriter.Write(_game.Snapshots);

                try
                {
                    await GameSaver.SaveGameAsync(pgn);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not save game...");
                }
                
                Console.WriteLine($"Game saved in: {GameSaver.GamesDirectory}");
            }
            
            Console.WriteLine("Press R to restart or Q to quit.");
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                
                if (key == ConsoleKey.R)
                {
                    _game = new chess.Game.Game(_consolePlayer, _enginePlayer);
                    break;
                }
                
                if (key == ConsoleKey.Q)
                    return;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _enginePlayer.DisposeAsync();
    }
}