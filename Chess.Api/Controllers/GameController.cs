using Chess.Api.Hubs;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Controllers;

public class GameController : ControllerBase
{
    [Route("/ws/game/{gameId}")]
    public async Task GetGameSnapshot(
        HttpContext context,
        [FromRoute] string gameId,
        GameHub hub)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }
        
        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        await hub.HandleConnectionAsync(gameId, socket, context.RequestAborted);
    }
}