namespace chess.Game;

public enum GameStatus
{
    InProgress,
    WhiteWonByCheckmate,
    BlackWonByCheckmate,
    WhiteWonByResignation,
    BlackWonByResignation,
    DrawByStalemate,
    DrawByInsufficientMaterial,
    DrawByThreefoldRepetition,
    DrawBy75MoveRule
}