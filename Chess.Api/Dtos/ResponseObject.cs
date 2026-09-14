using Chess.Api.Dtos.ResponseDtos;

namespace Chess.Api.Dtos;

public sealed record ResponseObject(
    ResponseType Type,
    object Data);