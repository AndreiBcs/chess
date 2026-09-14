using chess.Moves;

namespace Chess.Api.Dtos.ResponseDtos;

public readonly record struct MoveStatusDto
{
    public string ResultReason { get; init; }

    public static MoveStatusDto ToMoveStatusDto(MoveStatus moveStatus)
    {
        return new MoveStatusDto
        {
            ResultReason = moveStatus.InvalidMoveReason!
        };
    }
}