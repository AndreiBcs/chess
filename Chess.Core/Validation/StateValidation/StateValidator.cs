using System.Collections.Immutable;
using chess.Board;
using chess.Game;
using chess.Moves;

namespace chess.Validation.StateValidation;

public static class StateValidator
{
    public static GameStatus ValidateState(
        Move previousMove,
        Color currentTurn,
        Board.Board board,
        int halfMoveClock,
        int fullMoveCounter,
        Position? enPassantTarget,
        ImmutableList<CastlingRights> castlingRights,
        ImmutableList<string> positionHistory)
    {
        
    }
}