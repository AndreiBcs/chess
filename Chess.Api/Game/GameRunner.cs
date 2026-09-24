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
    private readonly EnginePlayer? _enginePlayer;
    //public readonly HttpPlayer HttpPlayer;
    public readonly IReadOnlyDictionary<Color, HttpPlayer> HttpPlayers;
    private readonly int _elo;

    // player vs chess engine
    public GameRunner(StartRequestDto request)
    {
        var normalizedPlayerColor = request.PlayerColor.Trim().ToLowerInvariant();

        var playerColor = normalizedPlayerColor == "white" ? Color.White : Color.Black;
        var engineColor = playerColor == Color.White ? Color.Black : Color.White;

        _elo = request.Elo;
        var httpPlayer = new HttpPlayer(playerColor);
        _enginePlayer = new EnginePlayer(engineColor, ChessEngine.Stockfish);
        HttpPlayers = new Dictionary<Color, HttpPlayer> { [playerColor] = httpPlayer };

        _game = new chess.Game.Game(httpPlayer, _enginePlayer);
    }

    // player vs player
    public GameRunner(HttpPlayer white, HttpPlayer black)
    {
        _enginePlayer = null;
        
        HttpPlayers = new Dictionary<Color, HttpPlayer>
        {
            [Color.White] = white,
            [Color.Black] = black
        };

        _game = new chess.Game.Game(white, black);
    }

    public async IAsyncEnumerable<GameSnapshot> Run(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        if (_enginePlayer is not null)
        {
            await _enginePlayer.Uci.StartEngine();
            await _enginePlayer.Uci.SetElo(_elo);
            await _enginePlayer.Uci.NewGame();   
        }

        await foreach (var snapshot in _game.GameLoop().WithCancellation(ct))
        {
            yield return snapshot;
        }
    }
}