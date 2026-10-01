using chess;

namespace Chess.Api.Messages.Dtos.ServerDtos;

public readonly record struct MatchmakingResponseDto(
    bool Matched,
    string? SessionId,
    Color? PlayerColor);