using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Game.GameState;

public sealed class GameSnapshot
{
    public bool IsOver { get; }
    public Color CurrentTurn { get; }
    public IReadOnlyBoard Board { get; }
    public int FullMoveCounter { get; }
    public int HalfMoveCounter { get; }
    // TODO add en-passant & castling info

    public GameSnapshot(
        bool over, 
        Color currentTurn, 
        Board board, 
        int fullMoveCounter, 
        int halfMoveCounter)
    {
        IsOver = over;
        CurrentTurn = currentTurn;
        Board = board;
        FullMoveCounter = fullMoveCounter;
        HalfMoveCounter = halfMoveCounter;
    }
}