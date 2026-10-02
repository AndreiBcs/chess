using chess;
using Chess.Api.Hubs;
using Chess.Api.Messages.Dtos.ClientDtos;
using Chess.Api.Player;
using Microsoft.AspNetCore.SignalR;

namespace Chess.Api.Game;

public sealed partial class GameSession
{
    public static HttpPlayer CreatePlayer(Color color, StartGameDto dto)
    {
        var p = new HttpPlayer(color);
        
        if (dto is ClashPvPStartDto c)
        {
            p.ClashPieces = c.PlayerPieces.ToList();
        }
        
        return p;
    }

    public static GameSession CreatePvP(
        string id,
        ChessGameMode mode,
        HttpPlayer white,
        HttpPlayer black,
        IHubContext<GameHub> hub,
        Action<string> onFinished)
    {
        var game = new chess.Game.Game(white, black, mode);
        
        return new GameSession(
            id,
            game,
            new Dictionary<Color, HttpPlayer> { [Color.White] = white, [Color.Black] = black },
            null,
            null,
            hub,
            onFinished);
    }
}