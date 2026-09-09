using chess.Board;
using chess.Moves;
using chess.Pieces;

namespace Chess.Api.Dtos.RequestDtos;

public readonly record struct PositionDto(
    int Row,
    int Column);

public readonly record struct MoveDto
{
    public RequestDtoType Type { get; init; }
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