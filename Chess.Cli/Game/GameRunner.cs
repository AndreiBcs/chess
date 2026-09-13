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
    private readonly Color _playerColor;
    private readonly int _elo;

    public GameRunner(CliArguments.ParsedOptions options)
    {
        if (!Equals(options.PlayerColor.ToString(), "White") || 
            !Equals(options.PlayerColor.ToString(), "Black"))
            throw new ArgumentException("You can only play as White or Black.", nameof(options));
            
        var userColor = options.PlayerColor;
        _playerColor = userColor;
        
        var engineColor = userColor == Color.White?
            Color.Black : 
            Color.White;

        var engineType = options.Engine switch
        {
            ChessEngine.Stockfish => ChessEngine.Stockfish,
            ChessEngine.Deakfish => ChessEngine.Deakfish,
            _ => throw new ArgumentException("Engine not supported.", nameof(options))
        };
        
        if (engineType is ChessEngine.Stockfish && 
            options.Elo is < 1320 or > 3190)
        {
            throw new ArgumentOutOfRangeException(nameof(options),
                "Stockfish elo can be above 1320 and below 3190");
        }
        _elo = options.Elo;
        
        var consolePlayer = new ConsolePlayer(userColor);
        _enginePlayer = new EnginePlayer(engineColor, engineType);
        
        _game = new chess.Game.Game(consolePlayer, _enginePlayer);
    }

    public async Task Run()
    {
        await _enginePlayer.Uci.StartEngine();
        await _enginePlayer.Uci.SetElo(_elo);
        await _enginePlayer.Uci.NewGame();

        await foreach (var snapshot in _game.GameLoop())
        {
            BoardRenderer.Render(snapshot, _playerColor);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _enginePlayer.DisposeAsync();
    }
}