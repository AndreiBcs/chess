using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Chess.Api.Dtos.RequestDtos;
using Chess.Api.Dtos.ResponseDtos;
using Chess.Api.Game;
using chess.Moves;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    [Route("/ws")]
    public async Task WebSocket(CancellationToken ct = default)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            
        GameRunner? runner = null;
        
        // start the game when the client sends start options
        Task? gameTask = null;
            
        while (socket.State == WebSocketState.Open)
        {
            // get the request
            var message = await ReceiveMessageAsync(socket, ct);

            if (message is null)
                break;

            var request = JsonSerializer.Deserialize<WebSocketRequest>(
                message,
                JsonOptions);

            if (request is null)
                continue;
            
            // check the request type
            switch (request.Type)
            {
                case RequestDtoType.StartOptions when runner is null:
                {
                    var options = request.Data.Deserialize<StartOptionsDto>(JsonOptions);

                    runner = new GameRunner(options);

                    runner.HttpPlayer.MoveResultReceived += async result =>
                    {
                        if (result == MoveResult.Invalid)
                        {
                            await SendMessageAsync(
                                socket,
                                MoveResultDto.ToMoveResultDto(result),
                                ct);
                        }
                    };

                    gameTask = RunGameAsync(runner, socket, ct);

                    break;
                }

                case RequestDtoType.Move when runner is not null:
                {
                    var moveDto = request.Data.Deserialize<MoveDto>(JsonOptions);
                    
                    var move = MoveDto.FromMoveDto(moveDto);
                    
                    runner.HttpPlayer.ProvideMoveFromClient(move);
                    
                    break;
                }
                default:
                {
                    var error = new ErrorDto
                    {
                        Type = ResponseDtoType.Error,
                        Error = "Unknown Request"
                    };
                    
                    await SendMessageAsync(socket, error, ct);
                    break;
                }
            }
        }
        
        if(gameTask is not null)
            await gameTask;

        if (runner is not null)
            await runner.DisposeAsync();
    }

    private static async Task RunGameAsync(
        GameRunner runner,
        WebSocket socket,
        CancellationToken ct)
    {
        await foreach (var snapshot in runner.Run(ct))
        {
            var dto = SnapshotDto.ToSnapshotDto(snapshot);

            await SendMessageAsync(socket, dto, ct);
        }
    }

    private static async Task SendMessageAsync(
        WebSocket socket, 
        object message,
        CancellationToken ct)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(
            message,
            JsonOptions);

        await socket.SendAsync(
            json,
            WebSocketMessageType.Text,
            true,
            ct);
    }

    private static async Task<string?> ReceiveMessageAsync(
        WebSocket socket,
        CancellationToken ct)
    {
        var buffer = new byte[4096];
        using var stream = new MemoryStream();

        while (true)
        {
            var result = await socket.ReceiveAsync(buffer, ct);

            if (result.MessageType == WebSocketMessageType.Close)
                return null;

            stream.Write(buffer, 0, result.Count);
            
            if(result.EndOfMessage)
                return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}