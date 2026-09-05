using System.Collections.Immutable;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces;
using chess.Validation.MoveValidation;

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
        // 1. check for DrawBy75MoveRule
        if (halfMoveClock >= 150)
        {
            return GameStatus.DrawBy75MoveRule;
        }

        // 2. check for DrawByThreefoldRepetition
        var parts = positionHistory[^1].Split(' ');
        var currentPosition = string.Join(" ", parts, 0, parts.Length - 2);
        
        if (positionHistory.Count(p =>
            {
                var part = p.Split(' ');
                var pos = string.Join(" ", part, 0, part.Length - 2);
                return pos == currentPosition;
            }) >= 3)
        {
            return GameStatus.DrawByThreefoldRepetition;
        }
        
        // 3. check for DrawByInsufficientMaterial
        var pieces = new List<Piece>();

        foreach (var square in board.CopySquares())
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
                // if bishops are on the same color its a draw
                if (board.BishopsAreSameColor())
                {
                    return GameStatus.DrawByInsufficientMaterial;
                }
            }
        }
        
        // 4. start evaluating for checkmate or stalemate
        var isInCheck = CheckValidator.IsKingInCheck(board, currentTurn);
        var hasLegalMoves = false;
        GameSnapshot snapshot; //TODO

        foreach (var square in board.CopySquares())
        {
            var piece = square.Piece;
            if (piece is null || piece.Color != currentTurn)
            { 
                continue;
            }

            var possiblePositions = piece.GetPiecePositions(board);

            foreach (var pos in possiblePositions)
            {
                if (MoveValidator.ValidateMove(snapshot, new Move(square.Position, pos))
                        .MoveResult ==  MoveResult.Valid)
                {
                    hasLegalMoves = true;
                }
            }
        }

        return isInCheck switch
        {
            true when !hasLegalMoves => currentTurn == Color.White 
                ? GameStatus.BlackWon 
                : GameStatus.WhiteWon,
            
            false when !hasLegalMoves => GameStatus.DrawByStalemate,
            
            _ => GameStatus.InProgress
        };
    }
}