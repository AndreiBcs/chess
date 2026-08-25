using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Game;

public static class MoveValidator
{
    public static bool ValidateMove(this Game game)
    {
        var move = game.CurrentMove;
        var possibleMoves = move.Piece?
            .GetPossiblePositions(game.Board, move.From);

        if (possibleMoves is null || !possibleMoves.Contains(move.To))
        {
            return false;
        }
        
        game.Board.MovePiece(move.From, move.To);
        
        // get every covered square for Checks

        var enemyPlayer = game.CurrentTurn == Color.White ? 
            game.PlayerBlack : game.PlayerWhite;

        var squaresCoveredByEnemy = new List<Position>();

        foreach (var p in enemyPlayer.ActivePieces)
        {
            var positions = p.GetPossiblePositions(game.Board, p.Position).ToList();
            squaresCoveredByEnemy.AddRange(positions);
        }
        
        var king = game.CurrentTurn == Color.White ? 
            game.PlayerWhite.King : game.PlayerBlack.King;

        if (squaresCoveredByEnemy.Contains(king.Position))
        {
            game.Board.MovePiece(move.To, move.From); // put piece back
            return false;
        }
        
        return true;
    }
}