namespace Chess.Api.Dtos.RequestDtos;

public readonly record struct StartOptionsDto(
    string PlayerColor,
    string EngineType,
    int Elo);