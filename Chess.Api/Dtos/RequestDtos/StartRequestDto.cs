namespace Chess.Api.Dtos.RequestDtos;

public readonly record struct StartRequestDto(
    string PlayerColor,
    string EngineType,
    int Elo,
    string? GameId = null);