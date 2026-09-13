using chess.Moves;

namespace Chess.Api.Dtos.ResponseDtos;

public readonly record struct MoveStatusDto
{
    public ResponseDtoType Type { get; init; }
    public string ResultReason { get; init; }

    public static MoveStatusDto ToMoveStatusDto(MoveStatus moveStatus)
    {
        return new MoveStatusDto
        {
            Type = ResponseDtoType.MoveResult,
            ResultReason = moveStatus.InvalidMoveReason!
        };
    }
}