using chess;
using Chess.Api.Game;
using Chess.Api.Hubs;
using Chess.Engine;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly GameHub _hub;
    public GameController(GameHub hub)
    {
        _hub = hub;
    }
    
    [HttpPost("{gameId}/start")]
    public async Task<IActionResult> StartGame(
        [FromRoute] string gameId,
        [FromQuery] int elo = 1600,
        CancellationToken ct = default)
    {
        var runner = new GameRunner(gameId, _hub, Color.White, elo, ChessEngine.Stockfish);
        _hub.RegisterGame(gameId, runner);
        
        _ = runner.Run(ct);
        
        return Accepted(new { gameId, status = "started" });
    }

    [WebSocketRoute("/ws/game/{gameId}")]
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

        if (hub.GetGame(gameId) is var game)
        {
            var snapshot = game.GameSnapshot;
            await hub.BroadcastGameSnapshotAsync(gameId, snapshot, context.RequestAborted);    
        }
        
        await hub.HandleConnectionAsync(gameId, socket, context.RequestAborted);
    }
}