namespace chess.Moves;

public record MoveStatus(
    MoveResult Result, 
    MoveType MoveType = MoveType.Normal);