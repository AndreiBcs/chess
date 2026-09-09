namespace Chess.Api.Dtos.RequestDtos;

public readonly record struct StartOptionsDto(
    string PlayerColor = "white",
    string EngineType = "stockfish",
    int Elo = 1400);