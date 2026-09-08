namespace Chess.Api.Dtos;

public readonly record struct PositionDto(
    int Row,
    int Column);