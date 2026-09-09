using chess.Moves;

namespace Chess.Api.Dtos.ResponseDtos;

public readonly record struct MoveResultDto
{
    public ResponseDtoType Type { get; init; }
    public string Result { get; init; }

    public static MoveResultDto ToMoveResultDto(MoveResult result)
    {
        return new MoveResultDto
        {
            Type = ResponseDtoType.MoveResult,
            Result = result.ToString()
        };
    }
}