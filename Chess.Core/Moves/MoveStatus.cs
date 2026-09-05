using chess.Game;

namespace chess.Moves;

public readonly record struct MoveStatus(
    MoveResult MoveResult,
    bool IsCapture = false,
    bool IsPawnMove = false,
    bool IsCastling = false,
    bool IsEnPassant = false,
    bool IsPromotion = false,
    CastlingRights? CastlingRights = null);