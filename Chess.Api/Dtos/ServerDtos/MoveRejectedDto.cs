namespace Chess.Api.Dtos.ServerDtos;

public readonly record struct MoveRejectedDto(
    string Reason);