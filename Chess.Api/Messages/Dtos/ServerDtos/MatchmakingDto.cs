using chess;

namespace Chess.Api.Messages.Dtos.ServerDtos;

public readonly record struct MatchmakingDto(
    bool Matched,
    Color PlayerColor);