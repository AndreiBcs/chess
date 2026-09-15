using System.Runtime.CompilerServices;
using chess;
using Chess.Api.Dtos.RequestDtos;
using Chess.Api.Player;
using Chess.Engine;
using chess.Game;

namespace Chess.Api.Game;

public sealed class GameRunner
{
    private readonly chess.Game.Game _game;
    private readonly EnginePlayer _enginePlayer;
    public readonly HttpPlayer HttpPlayer;
    private readonly int _elo;

    public GameRunner(StartRequestDto request)
    {
        var normalizedPlayerColor = request.PlayerColor.Trim().ToLowerInvariant();
        var normalizedEngineType = request.EngineType.Trim().ToLowerInvariant();

        var playerColor = normalizedPlayerColor == "white" ? Color.White : Color.Black;
        var engineColor = playerColor == Color.White ? Color.Black : Color.White;
        var engineType = ChessEngine.Stockfish;

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
}