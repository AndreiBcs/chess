namespace chess.Moves;

public readonly record struct MoveStatus(
    MoveResult Result, 
    bool IsCapture = false,
    bool IsPawnMove = false,
    bool IsCastling = false,
    bool IsEnPassant = false,
    bool IsPromotion = false);