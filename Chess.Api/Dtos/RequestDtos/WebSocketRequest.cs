using System.Text.Json;

namespace Chess.Api.Dtos.RequestDtos;

public record WebSocketRequest(
    RequestDtoType Type,
    JsonElement Data);