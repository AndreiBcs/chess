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

    public GameRunner(StartRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerColor))
            throw new ArgumentException("PlayerColor is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.EngineType))
            throw new ArgumentException("EngineType is required.", nameof(request));

        var playerColor = request.PlayerColor.Trim().ToLowerInvariant() switch
        {
            "white" => Color.White,
            "black" => Color.Black,
            _ => throw new ArgumentException("PlayerColor must be 'white' or 'black'.", nameof(request))
        };
        
        var engineColor = playerColor == Color.White ?
            Color.Black : 
            Color.White;

        var engineType = request.EngineType.Trim().ToLowerInvariant() switch
        {
            "stockfish" => ChessEngine.Stockfish,
            _ => throw new ArgumentException("EngineType must be 'stockfish'.", nameof(request))
        };

        if (request.Elo <= 0)
            throw new ArgumentOutOfRangeException(nameof(request), "Elo must be greater than zero.");

        _elo = request.Elo;
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
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _enginePlayer.DisposeAsync();
    }
}