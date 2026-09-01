namespace chess.Moves;

public record MoveStatus(
    MoveResult Result, 
    bool IsCapture = false,
    bool IsPawnMove = false,
    bool IsCastling = false,
    bool IsEnPassant = false,
    bool IsPromotion = false);