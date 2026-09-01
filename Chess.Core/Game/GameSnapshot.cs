using System.Collections.Immutable;
using chess.Board;
using chess.Moves;

namespace chess.Game;

public sealed class GameSnapshot
{
    public bool IsOver { get; }
    public bool IsDraw { get; }
    public Color CurrentTurn { get; }
    public IReadOnlyBoard Board { get; }
    public int FullMoveCounter { get; }
    public int HalfMoveCounter { get; }
    public CastlingRights CastlingRights { get; }
    public ImmutableList<Move> MoveHistory { get; }
    public Position? EnPassantTargetSquare { get; }

    public GameSnapshot(
        bool over, 
        Color currentTurn, 
        Board.Board board, 
        int fullMoveCounter, 
        int halfMoveCounter,
        CastlingRights castlingRights,
        List<Move> moveHistory, 
        Position? enPassantTarget, 
        bool isDraw)
    {
        IsOver = over;
        CurrentTurn = currentTurn;
        Board = board;
        FullMoveCounter = fullMoveCounter;
        HalfMoveCounter = halfMoveCounter;
        CastlingRights = castlingRights;
        EnPassantTargetSquare = enPassantTarget;
        IsDraw = isDraw;
        MoveHistory = moveHistory.ToImmutableList();
    }
}