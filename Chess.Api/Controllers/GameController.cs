using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Chess.Api.Dtos.RequestDtos;
using Chess.Api.Dtos.ResponseDtos;
using Chess.Api.Game;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    
    [Route("/ws")]
    public async Task WebSocket(
        HttpContext context,
        CancellationToken ct = default)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }
        
        using var socket = await context.WebSockets.AcceptWebSocketAsync();

        GameRunner? runner = null;
        Task? gameTask = null;

        while (socket.State == WebSocketState.Open)
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            var result = await socket.ReceiveAsync(buffer, ct);
            var message = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);
            var request = JsonSerializer.Deserialize<WebSocketRequest>(message);

            switch (request!.Type)
            {
                case RequestDtoType.StartOptions:
                {
                    var options = request.Data.Deserialize<StartOptionsDto>();
                        
                    runner = new GameRunner(options);

                    gameTask = Task.Run(async () =>
                    {
                        await foreach (var snapshot in runner.Run(ct))
                        {
                            var dto = SnapshotDto.ToSnapshotDto(snapshot);
                            var json = JsonSerializer.Serialize(dto);
                            var bytes = Encoding.UTF8.GetBytes(json);

                            await socket.SendAsync(
                                bytes,
                                WebSocketMessageType.Text,
                                true,
                                ct);
                        }
                    }, ct);
                        
                    break;
                }
                case RequestDtoType.Move:
                {
                    if (runner is null)
                        break;
                    
                    var moveDto = request.Data.Deserialize<MoveDto>();
                    var move = MoveDto.FromMoveDto(moveDto);
                    
                    runner.HttpPlayer.ProvideMoveFromClient(move);
                    
                    break;
                }
            }
        }

        if (gameTask is not null)
            await gameTask;
    }
}