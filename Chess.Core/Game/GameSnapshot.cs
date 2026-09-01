using chess.Board;

namespace chess.Game;

public sealed class GameSnapshot
{
    public bool IsOver { get; }
    public Color CurrentTurn { get; }
    public IReadOnlyBoard Board { get; }
    public int FullMoveCounter { get; }
    public int HalfMoveCounter { get; }
    public CastlingRights CastlingRights { get; }

    public GameSnapshot(
        bool over, 
        Color currentTurn, 
        Board.Board board, 
        int fullMoveCounter, 
        int halfMoveCounter,
        CastlingRights castlingRights)
    {
        IsOver = over;
        CurrentTurn = currentTurn;
        Board = board;
        FullMoveCounter = fullMoveCounter;
        HalfMoveCounter = halfMoveCounter;
        CastlingRights = castlingRights;
    }
}