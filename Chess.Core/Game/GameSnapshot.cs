using System.Collections.Immutable;
using chess.Board;
using chess.Moves;

namespace chess.Game;

public sealed class GameSnapshot
{
    public GameStatus GameStatus { get; }
    public Color CurrentTurn { get; }
    public IReadOnlyBoard Board { get; }
    public int FullMoveCounter { get; }
    public int HalfMoveCounter { get; }
    public CastlingRights CastlingRights { get; }
    public ImmutableList<Move> MoveHistory { get; }
    public Position? EnPassantTargetSquare { get; }

    public GameSnapshot(
        GameStatus gameStatus,
        Color currentTurn, 
        Board.Board board, 
        int fullMoveCounter, 
        int halfMoveCounter,
        CastlingRights castlingRights,
        List<Move> moveHistory, 
        Position? enPassantTarget)
    {
        CurrentTurn = currentTurn;
        Board = board;
        FullMoveCounter = fullMoveCounter;
        HalfMoveCounter = halfMoveCounter;
        CastlingRights = castlingRights;
        EnPassantTargetSquare = enPassantTarget;
        GameStatus = gameStatus;
        MoveHistory = moveHistory.ToImmutableList();
    }
}