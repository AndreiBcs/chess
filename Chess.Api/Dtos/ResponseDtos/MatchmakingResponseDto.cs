using chess;

namespace Chess.Api.Dtos.ResponseDtos;

public readonly record struct MatchmakingResponseDto(
    bool Matched,
    string? SessionId,
    Color? PlayerColor);