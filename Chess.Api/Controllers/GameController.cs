using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Concurrent;
using Chess.Api.Dtos;
using Chess.Api.Dtos.RequestDtos;
using Chess.Api.Dtos.ResponseDtos;
using Chess.Api.Game;
using chess.Moves;
using Microsoft.AspNetCore.Mvc;

namespace Chess.Api.Controllers;

[ApiController]
public class GameController : ControllerBase
{
    private static readonly ConcurrentDictionary<string, GameSession> Sessions = new();
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

    [Route("/ws")]
    public async Task WebSocket(CancellationToken ct = default)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        
        GameSession? session = null;
            
        while (socket.State == WebSocketState.Open)
        {
            // get the request
            var message = await ReceiveMessageAsync(socket, ct);

            if (message is null)
                break;

            var request = JsonSerializer.Deserialize<RequestObject>(
                message,
                JsonOptions);

            if (request is null)
                continue;
            
            //Console.WriteLine(request);
            
            // check the request type
            switch (request.Type)
            {
                case RequestType.StartOptions when session is null:
                {
                    var options = request.Data.Deserialize<StartRequestDto>(JsonOptions);
                    if (string.IsNullOrWhiteSpace(options.GameId))
                        break;

                    session = Sessions.GetOrAdd(options.GameId, _ => new GameSession(new GameRunner(options)));
                    await session.AttachAsync(socket, ct);
                    session.GameTask ??= RunGameAsync(session, CancellationToken.None);

                    break;
                }

                case RequestType.Move when session is not null:
                {
                    var moveDto = request.Data.Deserialize<MoveRequestDto>(JsonOptions);
                    
                    var move = MoveRequestDto.FromMoveDto(moveDto);
                    
                    session.Runner.HttpPlayer.ProvideMoveFromClient(move);
                    
                    break;
                }
                default:
                {
                    var error = new ErrorDto
                    {
                        Error = "Unexpected request."
                    };
                    
                    await SendMessageAsync(socket, error, ResponseType.Error, ct);
                    break;
                }
            }
        }
        
        session?.Detach(socket);
    }

    private static async Task RunGameAsync(GameSession session, CancellationToken ct)
    {
        await foreach (var snapshot in session.Runner.Run(ct))
        {
            await session.PublishAsync(snapshot, ct);
        }
    }

    public static async Task SendMessageAsync(
        WebSocket socket, 
        object message,
        ResponseType responseType,
        CancellationToken ct)
    {
        var response = new ResponseObject(
            responseType,
            message);
        
        var json = JsonSerializer.SerializeToUtf8Bytes(
            response,
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