using System.Net.WebSockets;
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
public class GameController(ILogger<GameController> logger) : ControllerBase
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
            
        logger.LogInformation("Chess websocket connected.");
            
        using var lifetime = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var cancellationToken = lifetime.Token;
        var outgoing = Channel.CreateUnbounded<object>();
            
        GameRunner? runner = null;
        Task? gameTask = null;
            
        var sendTask = SendMessagesAsync(socket, outgoing.Reader, cancellationToken);

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var message = await ReceiveMessageAsync(socket, cancellationToken);
                if (message is null)
                    break;

                if (message.Value.MessageType != WebSocketMessageType.Text)
                {
                    await WriteErrorAsync(outgoing.Writer, "Only text messages are supported.");
                    continue;
                }

                WebSocketRequest? request;
                try
                {
                    request = JsonSerializer.Deserialize<WebSocketRequest>(message.Value.Payload, JsonOptions);
                }
                catch (JsonException)
                {
                    await WriteErrorAsync(outgoing.Writer, "The request is not valid JSON.");
                    continue;
                }

                if (request is null)
                {
                    await WriteErrorAsync(outgoing.Writer, "The request cannot be empty.");
                    continue;
                }

                switch (request.Type)
                {
                    case RequestDtoType.StartOptions when runner is null:
                        try
                        {
                            var options = request.Data.Deserialize<StartOptionsDto>(JsonOptions);
                            
                            runner = new GameRunner(options);
                                
                            runner.HttpPlayer.MoveResultReceived += result =>
                            {
                                if (result == MoveResult.Invalid)
                                    outgoing.Writer.TryWrite(MoveResultDto.ToMoveResultDto(result));
                            };
                            
                            logger.LogInformation("Chess websocket game started for {PlayerColor}.",
                                options.PlayerColor);
                            
                            gameTask = RunGameAsync(runner, outgoing.Writer, cancellationToken);
                        }
                        catch (ArgumentException exception)
                        {
                            await WriteErrorAsync(outgoing.Writer, exception.Message);
                        }
                        catch (JsonException exception)
                        {
                            await WriteErrorAsync(outgoing.Writer, exception.Message);
                        }

                        break;

                    case RequestDtoType.StartOptions:
                        await WriteErrorAsync(outgoing.Writer, "A game has already started.");
                        break;

                    case RequestDtoType.Move when runner is not null:
                        try
                        {
                            var moveDto = request.Data.Deserialize<MoveDto>(JsonOptions);
                            
                            runner.HttpPlayer.ProvideMoveFromClient(MoveDto.FromMoveDto(moveDto));
                            
                            logger.LogDebug("Chess move received.");
                        }
                        catch (ArgumentException exception)
                        {
                            await WriteErrorAsync(outgoing.Writer, exception.Message);
                        }
                        catch (JsonException exception)
                        {
                            await WriteErrorAsync(outgoing.Writer, exception.Message);
                        }

                        break;

                    case RequestDtoType.Move:
                        await WriteErrorAsync(outgoing.Writer, "Start a game before sending moves.");
                        break;

                    default:
                        await WriteErrorAsync(outgoing.Writer, "The request type is not supported.");
                        break;
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (WebSocketException exception)
        {
            logger.LogWarning(exception, "Chess websocket failed.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Chess websocket request failed.");
        }
        finally
        {
            await lifetime.CancelAsync();
            outgoing.Writer.TryComplete();

            if (gameTask is not null)
                await IgnoreCancellationAsync(gameTask);

            await IgnoreCancellationAsync(sendTask);

            if (runner is not null)
                await runner.DisposeAsync();

            logger.LogInformation("Chess websocket closed.");
        }
    }

    private async Task RunGameAsync(
        GameRunner runner,
        ChannelWriter<object> outgoing,
        CancellationToken ct)
    {
        try
        {
            await foreach (var snapshot in runner.Run(ct))
            {
                await outgoing.WriteAsync(SnapshotDto.ToSnapshotDto(snapshot), ct);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Chess game failed.");
            await WriteErrorAsync(outgoing, "The game failed.");
        }
    }

    private static async Task SendMessagesAsync(WebSocket socket, ChannelReader<object> messages, CancellationToken ct)
    {
        await foreach (var message in messages.ReadAllAsync(ct))
        {
            var payload = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);
            await socket.SendAsync(payload, WebSocketMessageType.Text, true, ct);
        }
    }

    private static async Task WriteErrorAsync(ChannelWriter<object> outgoing, string error)
    {
        await outgoing.WriteAsync(new ErrorDto(ResponseDtoType.Error, error));
    }

    private static async Task<(byte[] Payload, WebSocketMessageType MessageType)?> ReceiveMessageAsync(
        WebSocket socket,
        CancellationToken ct)
    {
        using var message = new MemoryStream();
        var buffer = new byte[4096];

        while (true)
        {
            var result = await socket.ReceiveAsync(buffer, ct);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                if (socket.State == WebSocketState.CloseReceived)
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", ct);

                return null;
            }

            await message.WriteAsync(buffer.AsMemory(0, result.Count), ct);
            
            if (result.EndOfMessage)
                    return (message.ToArray(), result.MessageType);
        }
    } 
    private static async Task IgnoreCancellationAsync(Task task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException) 
        {
        } 
    }
}