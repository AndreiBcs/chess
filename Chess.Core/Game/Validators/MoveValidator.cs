using chess.Entities.Board;
using chess.Game.GameState;

namespace chess.Game.Validators;

public static class MoveValidator
{
    public static MoveResult ValidateMove(GameSnapshot snapshot, Move move)
    {
        var piece = snapshot.Board.GetPiece(move.From);

        if (piece is null || piece.Color != snapshot.CurrentTurn)
            return MoveResult.Invalid;

        var possibleMoves = piece
            .GetPossiblePositions(snapshot.Board, move.From);

        if (!possibleMoves.Contains(move.To))
            return MoveResult.Invalid;
        
        // TODO validate special move
        
        // TODO validate king safety
        
        return MoveResult.Valid;
    }
}