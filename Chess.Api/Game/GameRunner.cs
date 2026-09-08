using chess;
using Chess.Api.Hubs;
using Chess.Api.Player;
using Chess.Engine;

namespace Chess.Api.Game;

public sealed class GameRunner : IAsyncDisposable
{
    private readonly chess.Game.Game _game;
    private readonly EnginePlayer _enginePlayer;
    public readonly HttpPlayer HttpPlayer;
    private readonly GameHub _hub;
    private readonly string _gameId;
    private readonly int _elo;

    public GameRunner(
        Color playerColor, 
        int elo, 
        ChessEngine engineType,
        string gameId,
        GameHub hub)
    {
        var engineColor = playerColor == Color.White?
            Color.Black : 
            Color.White;

        _gameId = gameId;
        _hub = hub;
        _elo = elo;
        HttpPlayer = new HttpPlayer(playerColor);
        _enginePlayer = new EnginePlayer(engineColor, engineType);
        
        _game = new chess.Game.Game(HttpPlayer, _enginePlayer);
    }

    public async Task Run(CancellationToken ct = default)
    {
        await _enginePlayer.Uci.StartEngine();
        await _enginePlayer.Uci.SetElo(_elo);
        await _enginePlayer.Uci.NewGame();

        await foreach (var snapshot in _game.GameLoop().WithCancellation(ct))
        {
            await _hub.BroadcastGameSnapshotAsync(_gameId, snapshot, ct);
            
            HttpPlayer.ResetForNextMove();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _enginePlayer.DisposeAsync();
    }
}