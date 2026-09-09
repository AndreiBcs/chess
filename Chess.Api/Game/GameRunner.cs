using System.Runtime.CompilerServices;
using chess;
using Chess.Api.Dtos.RequestDtos;
using Chess.Api.Player;
using Chess.Engine;
using chess.Game;

namespace Chess.Api.Game;

public sealed class GameRunner : IAsyncDisposable
{
    private readonly chess.Game.Game _game;
    private readonly EnginePlayer _enginePlayer;
    public readonly HttpPlayer HttpPlayer;
    private readonly int _elo;

    public GameRunner(StartOptionsDto options)
    {
        var playerColor = options.PlayerColor switch
        {
            "white" => Color.White,
            "black" => Color.Black,
            _ => Color.White
        };
        
        var engineColor = playerColor == Color.White ?
            Color.Black : 
            Color.White;

        var engineType = options.EngineType switch
        {
            _ => ChessEngine.Stockfish
        };

        _elo = options.Elo;
        HttpPlayer = new HttpPlayer(playerColor);
        _enginePlayer = new EnginePlayer(engineColor, engineType);
        
        _game = new chess.Game.Game(HttpPlayer, _enginePlayer);
    }

    public async IAsyncEnumerable<GameSnapshot> Run(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await _enginePlayer.Uci.StartEngine();
        await _enginePlayer.Uci.SetElo(_elo);
        await _enginePlayer.Uci.NewGame();

        await foreach (var snapshot in _game.GameLoop().WithCancellation(ct))
        {
            yield return snapshot;
            
            HttpPlayer.ResetForNextMove();
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _enginePlayer.DisposeAsync();
    }
}