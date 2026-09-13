namespace Chess.Api.Dtos.ResponseDtos;

public readonly record struct  ErrorDto(
    ResponseDtoType Type,
    string Error);