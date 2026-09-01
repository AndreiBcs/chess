using chess.Board;
using chess.Pieces;

namespace chess.Moves;

public readonly record struct Move(
    Position From,
    Position To, 
    PieceType? Promotion = null)
{
    public override string ToString()
    {
        return $"{From.ToString()}{To.ToString()}{Promotion?.ToString()}";
    }

    public string ToNotation()
    {
        return $"col: {From.Column} row: {From.Row} " +
               $"-> col: {To.Column} row: {To.Row}";
    }
}