using chess.Game;
using chess.Moves;
using chess.Pieces;
using chess.Validation.MoveValidation;

namespace chess.Validation.StateValidation;

public static class StateValidator
{
    public static GameStatus ValidateState(GameSnapshot snapshot)
    {
        // 1. check for DrawBy75MoveRule
        if (snapshot.HalfMoveClock >= 150)
        {
            return GameStatus.DrawBy75MoveRule;
        }

        // 2. check for DrawByThreefoldRepetition
        if (snapshot.PositionHistory.Count >= 3)
        {
            var parts = snapshot.PositionHistory[^1].Split(' ');
            var currentPosition = string.Join(" ", parts, 0, parts.Length - 2);
        
            if (snapshot.PositionHistory.Count(p =>
                {
                    var part = p.Split(' ');
                    var pos = string.Join(" ", part, 0, part.Length - 2);
                    return pos == currentPosition;
                }) >= 3)
            {
                return GameStatus.DrawByThreefoldRepetition;
            }
        }
        
        // 3. check for DrawByInsufficientMaterial
        var pieces = new List<Piece>();

        foreach (var square in snapshot.Board.CopySquares())
        {
            if (square.Piece is not null)
            {
                pieces.Add(square.Piece);
            }
        }

        if (pieces.All(p => p.Type == PieceType.King))
        {
            return GameStatus.DrawByInsufficientMaterial;
        }

        if (!pieces.Any(p => p.Type is PieceType.Pawn or PieceType.Rook or PieceType.Queen))
        {
            var nonKings = pieces.Where(p => p.Type != PieceType.King).ToList();
            
            // king + knight vs king
            if (nonKings.Count == 1 && nonKings[0].Type == PieceType.Knight)
            {
                return GameStatus.DrawByInsufficientMaterial;
            }
            
            // king + bishop vs king
            if (nonKings.Count == 1 && nonKings[0].Type == PieceType.Bishop)
            {
                return GameStatus.DrawByInsufficientMaterial;
            }
            
            // king + bishop vs king + bishop
            if (nonKings.Count == 2 && nonKings.All(p => p.Type == PieceType.Bishop))
            {
                // if bishops are on the same color it's a draw
                if (snapshot.Board.BishopsAreSameColor())
                {
                    return GameStatus.DrawByInsufficientMaterial;
                }
            }
        }
        
        // 4. start evaluating for checkmate or stalemate
        var isInCheck = CheckValidator.IsKingInCheck(snapshot.Board, snapshot.CurrentTurn);
        var hasLegalMoves = false;

        foreach (var square in snapshot.Board.CopySquares())
        {
            var piece = square.Piece;
            if (piece is null || piece.Color != snapshot.CurrentTurn)
            { 
                continue;
            }

            var possiblePositions = piece.GetPiecePositions(snapshot.Board);

            foreach (var pos in possiblePositions)
            {
                if (MoveValidator.ValidateMove(snapshot, new Move(square.Position, pos))
                        .MoveResult == MoveResult.Valid)
                {
                    // at least one move has to be valid
                    hasLegalMoves = true;
                    break;
                }
            }
        }

        return isInCheck switch
        {
            // in check and no legal moves => checkmate
            true when !hasLegalMoves => snapshot.CurrentTurn == Color.White 
                ? GameStatus.BlackWon 
                : GameStatus.WhiteWon,
            
            // not in check and no legal moves => stalemate 
            false when !hasLegalMoves => GameStatus.DrawByStalemate,
            
            _ => GameStatus.InProgress
        };
    }
}