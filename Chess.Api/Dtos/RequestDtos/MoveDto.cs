using chess.Board;
using chess.Moves;
using chess.Pieces;

namespace Chess.Api.Dtos.RequestDtos;

public readonly record struct PositionDto(
    int Row,
    int Column);

public readonly record struct MoveDto
{
    public PositionDto From { get; init; }
    public PositionDto To { get; init; }
    public string? Promotion { get; init; }

    public static Move FromMoveDto(MoveDto dto)
    {
        PieceType? promotion = dto.Promotion?.Trim().ToLowerInvariant()
            switch
            {
                "bishop" => PieceType.Bishop,
                "knight" => PieceType.Knight,
                "rook" => PieceType.Rook,
                "queen" => PieceType.Queen,
                null => null,
                _ => throw new ArgumentException("Promotion must be bishop, knight, queen, or rook.", nameof(dto))
            };
        
        var from = new Position(dto.From.Row, dto.From.Column);
        var to = new Position(dto.To.Row, dto.To.Column);
        
        return new Move(from, to, promotion);
    } 
}