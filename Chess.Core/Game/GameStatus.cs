namespace chess.Game;

public enum GameStatus
{
    InProgress,
    WhiteWon,
    BlackWon,
    DrawByStalemate,
    DrawByInsufficientMaterial,
    DrawByThreefoldRepetition,
    DrawBy75MoveRule
}