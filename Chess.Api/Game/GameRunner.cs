using chess;
using Chess.Api.Player;
using Chess.Engine;

namespace Chess.Api.Game;

public sealed class GameRunner : IAsyncDisposable
{
    private readonly chess.Game.Game _game;
    private readonly EnginePlayer _enginePlayer;
    private readonly int _elo;

    public GameRunner(Color playerColor, int elo, ChessEngine engineType)
    {
        var engineColor = playerColor == Color.White?
            Color.Black : 
            Color.White;

        _elo = elo;
        var player1 = new HttpPlayer(playerColor);
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
            
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _enginePlayer.DisposeAsync();
    }
}