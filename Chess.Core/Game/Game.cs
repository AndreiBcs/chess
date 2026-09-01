using chess.Board;
using chess.Moves;
using chess.Pieces;
using chess.Pieces.Types;
using chess.Validation;

namespace chess.Game;

public class Game
{
    public Game(Player.Player player1, Player.Player player2)
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

    private IReadOnlyList<Player.Player> Players { get; }
    private Player.Player GetPlayer(Color color)
    {
        return Players.Single(p => p.Color == color);
    }

    private Board.Board Board { get; } = new();
    private int FullMoveCounter { get; set; } = 1; // increase after black's turn
    private int HalfMoveCounter { get; set; } = 0; // back at 0 after a capture or pawn advance
    private List<Move> MoveHistory { get; } = [];
    private CastlingRights CastlingRights { get; set; } = new();

    public async IAsyncEnumerable<GameSnapshot> GameLoop()
    {
        while (!IsOver)
        {
            var snapshot = CreateSnapshot();
            yield return snapshot;
            
            MoveResult? result = null;

            while (true) // wait for player move and validate
            {
                var currentPlayer = GetPlayer(CurrentTurn);
                var move = await currentPlayer.GetMoveAsync(snapshot, result);
                
                var status = MoveValidator.ValidateMove(snapshot, move);
                result = status.Result;

                if (result is MoveResult.Invalid)
                {
                    continue;
                }
                
                if (result is MoveResult.Checkmate or MoveResult.Stalemate)
                {
                    IsOver = true;
                    break;
                }
                
                ApplyMove(snapshot, move, status);
                
                MoveHistory.Add(move);
                
                if (CurrentTurn == Color.Black)
                    FullMoveCounter++;
            
                SwitchTurn();
                break;
            }
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
            CastlingRights,
            MoveHistory);
    }
    
    private void ApplyMove( GameSnapshot snapshot, Move move, MoveStatus status) 
    {
        if (status.IsCastling)
        {
            HandleCastling(snapshot, move); 
        } 
        else if (status.IsEnPassant)
        {
            HandleEnPassant(move); 
        } 
        else if (status.IsPromotion)
        {
            HandlePromotion(move);
        } 
        else 
        { 
            UpdateCastlingRights(move);
            Board.MovePiece(move.From, move.To);
        } 
        
        UpdateHalfMoveClock(status); 
    }

    private void HandleCastling(GameSnapshot snapshot, Move move)
    {
        CastleValidator.TryGetCastling(snapshot, move, out var castling);
        
        Board.MovePiece(castling.KingFrom, castling.KingTo);
        Board.MovePiece(castling.RookFrom, castling.RookTo);

        RemoveCastlingRights(castling.Color);
    }

    private void UpdateCastlingRights(Move move)
    {
        var movedPiece = Board.GetPiece(move.From);
        var capturedPiece = Board.GetPiece(move.To);
        
        switch (movedPiece)
        {
            case King:
                RemoveCastlingRights(movedPiece.Color);
                break;
            case Rook:
                RemoveCastlingRights(movedPiece.Color, move.From);
                break;
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
            CastlingRights.CastlingPositions.RemoveAll(c => c.Color == color);
            return;
        }

        CastlingRights.CastlingPositions.RemoveAll(c =>
            c.Color == color &&
            c.RookFrom == rookPosition);
    }
    
    private void UpdateHalfMoveClock(MoveStatus status)
    {
        if (status.IsCapture || status.IsPawnMove)
            HalfMoveCounter = 0;
        else
            HalfMoveCounter++;
    }

    private void HandlePromotion(Move move)
    {
        
    }

    private void HandleEnPassant(Move move)
    {
        
    }
}
