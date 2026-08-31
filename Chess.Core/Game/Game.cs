using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Entities.Pieces.Types;
using chess.Entities.Player;
using chess.Game.GameState;
using chess.Game.Validators;

namespace chess.Game;

public class Game
{
    public Game(Player player1, Player player2)
    {
        Players = [player1, player2];
        Board.InitializeBoard();
    }

    private bool IsOver { get; set; }
    private Color CurrentTurn { get; set; } = Color.White;
    private void SwitchTurn()
    {
        CurrentTurn = CurrentTurn == Color.White ? 
            Color.Black : 
            Color.White;
    }

    private IReadOnlyList<Player> Players { get; }
    private Player GetPlayer(Color color)
    {
        return Players.Single(p => p.Color == color);
    }

    private Board Board { get; } = new();
    private int FullMoveCounter { get; set; } = 1; // increase after black's turn
    private int HalfMoveCounter { get; set; } = 0; // back at 0 after a capture or pawn advance
    private Move PreviousMove { get; set; }
    private Castling Castling { get; set; } = new();

    public async IAsyncEnumerable<GameSnapshot> GameLoop()
    {
        while (true)
        {
            var snapshot = CreateSnapshot();
            yield return snapshot;
            
            if(IsOver) yield break;
            
            MoveResult? result = null;

            while (true)
            {
                var currentPlayer = GetPlayer(CurrentTurn);
                var move = await currentPlayer.GetMoveAsync(snapshot, result);
                
                result = MoveValidator.ValidateMove(snapshot, move);

                if (result == MoveResult.Valid)
                {
                    var movedPiece = Board.GetPiece(move.From);
                    var capturedPiece = Board.GetPiece(move.To);
                    var isCastling = MoveValidator
                        .IsCastlingMove(snapshot, move, CurrentTurn, out var castling);
                    
                    if (isCastling)
                    {
                        HandleCastling(castling);
                    }
                    else
                    {
                        Board.MovePiece(move.From, move.To);
                        UpdateCastlingRights(movedPiece, capturedPiece, move);
                    }
                    break;
                }
                if (result is MoveResult.Checkmate or MoveResult.Stalemate)
                {
                    IsOver = true;
                    break;
                }
            }
            
            SwitchTurn();
            FullMoveCounter++;
        }
    }

    private GameSnapshot CreateSnapshot()
    {
        return new GameSnapshot(
            IsOver, 
            CurrentTurn,
            Board, 
            FullMoveCounter,
            HalfMoveCounter, 
            Castling);
    }

    private void HandleCastling(CastlingInfo castling)
    {
        Board.MovePiece(castling.KingFrom, castling.KingTo);
        Board.MovePiece(castling.RookFrom, castling.RookTo);

        RemoveCastlingRights(castling.Color);
    }

    private void UpdateCastlingRights(Piece? movedPiece, Piece? capturedPiece, Move move)
    {
        if (movedPiece is King)
        {
            RemoveCastlingRights(movedPiece.Color);
        }
        else if (movedPiece is Rook)
        {
            RemoveCastlingRights(movedPiece.Color, move.From);
        }

        if (capturedPiece is Rook)
        {
            RemoveCastlingRights(capturedPiece.Color, move.To);
        }
    }

    private void RemoveCastlingRights(Color color, Position? rookPosition = null)
    {
        if (rookPosition is null)
        {
            Castling.CastlingPositions.RemoveAll(c => c.Color == color);
            return;
        }

        Castling.CastlingPositions.RemoveAll(c =>
            c.Color == color &&
            c.RookFrom == rookPosition);
    }
}
