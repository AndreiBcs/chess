using System.Text.Json;
using Chess.Api.Dtos.RequestDtos;

namespace Chess.Api.Dtos;

public sealed record RequestObject(
    RequestType Type,
    JsonElement Data);