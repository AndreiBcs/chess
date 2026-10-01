using chess.Moves;

namespace Chess.Api.Messages.Dtos.ClientDtos;

public sealed record MakeMoveDto(
    Move Move);