using chess.Entities.Pieces;

namespace chess.Entities.Board;

public readonly record struct Move(
    Position From,
    Position To, 
    PieceType? Promotion = null);