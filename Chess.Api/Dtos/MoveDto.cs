using chess.Board;
using chess.Moves;
using chess.Pieces;

namespace Chess.Api.Dtos;

public readonly record struct MoveDto
{
    public DtoType Type { get; init; }
    public PositionDto From { get; init; }
    public PositionDto To { get; init; }
    public string? Promotion { get; init; }

    public static Move FromMoveDto(MoveDto dto)
    {
        PieceType? promotion = dto.Promotion?.Trim().ToLower()
            switch 
            {
                "bishop" => PieceType.Bishop,
                "knight" => PieceType.Knight,
                "rook" => PieceType.Rook,
                _ => PieceType.Queen
            };
        
        var from = new Position(dto.From.Row, dto.From.Column);
        var to = new Position(dto.To.Row, dto.To.Column);
        
        return new Move(from, to, promotion);
    } 
}


public readonly record struct MoveResultDto
{
    public DtoType Type { get; init; }
    public string Result { get; init; }

    public static MoveResultDto ToMoveResultDto(MoveResult result)
    {
        return new MoveResultDto
        {
            Type = DtoType.MoveResult,
            Result = result.ToString()
        };
    }
}