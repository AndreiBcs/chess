namespace Chess.Api.Messages.Dtos.ServerDtos;

public readonly record struct MoveRejectedDto(
    string Reason);