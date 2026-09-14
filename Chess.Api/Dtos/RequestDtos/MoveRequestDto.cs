using chess.Board;
using chess.Moves;
using chess.Pieces;

namespace Chess.Api.Dtos.RequestDtos;

public readonly record struct PositionDto(
    int Row,
    int Column);

public readonly record struct MoveRequestDto
{
    public PositionDto From { get; init; }
    public PositionDto To { get; init; }
    public string? Promotion { get; init; }

    public static Move FromMoveDto(MoveRequestDto requestDto)
    {
        PieceType? promotion = requestDto.Promotion?.Trim().ToLowerInvariant()
            switch
            {
                "bishop" => PieceType.Bishop,
                "knight" => PieceType.Knight,
                "rook" => PieceType.Rook,
                "queen" => PieceType.Queen,
                null => null,
                _ => throw new ArgumentException(
                    "Promotion must be bishop, knight, queen, or rook.",
                    nameof(requestDto))
            };
        
        var from = new Position(requestDto.From.Row, requestDto.From.Column);
        var to = new Position(requestDto.To.Row, requestDto.To.Column);
        
        return new Move(from, to, promotion);
    } 
}