using System.Text.Json;

namespace Chess.Api.Messages;

public readonly record struct ServerMessage(
    string? SessionId,
    ServerMessageType MessageType,
    JsonElement Data);

public enum ServerMessageType
{
    GameSnapshotDto,
    MatchmakingDto,
    MoveRejectedDto,
    ErrorDto
}