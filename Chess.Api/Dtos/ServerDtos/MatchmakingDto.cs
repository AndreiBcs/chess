using chess;

namespace Chess.Api.Dtos.ServerDtos;

public readonly record struct MatchmakingDto(
    bool Matched,
    string? Token,
    Color PlayerColor);