using System.Text.Json;

namespace Chess.Api.Messages;

public readonly record struct ClientMessage(
    string? SessionId,
    ClientMessageType MessageType,
    JsonElement Data);

public enum ClientMessageType
{
    StartGameDto,
    MakeMoveDto,
    ResignGameDto,
}