using chess.Board;
using chess.Moves;
using chess.Pieces.Types;
using chess.Validation;

namespace chess.Game;

public class Game
{
    public Game(Player.Player player1, Player.Player player2)
    {
        Players = [player1, player2];
        Board.InitializeBoard();
        
        PositionHistory.Add(CreatePositionKey());
    }

    private bool IsOver { get; set; }
    private bool IsDraw { get; set; }
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
    private int HalfMoveCounter { get; set; } // back at 0 after a capture or pawn advance
    private List<string> PositionHistory { get; } = [];
    private List<Move> MoveHistory { get; } = [];
    private CastlingRights CastlingRights { get; } = new();
    private Position? EnPassantTargetSquare { get; set; }

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
                
                UpdateGameState(snapshot, move, status);

                if (IsGameDraw())
                {
                    IsOver = true;
                    IsDraw = true;
                }
                
                break;
            }
        }
    }

    private void UpdateGameState(GameSnapshot snapshot, Move move, MoveStatus status)
    {
        ApplyMove(snapshot, move, status);
                
        MoveHistory.Add(move);
                
        if (CurrentTurn == Color.Black)
            FullMoveCounter++;
            
        UpdateEnPassantTarget(snapshot, move);
        
        SwitchTurn();
                
        // after every action because it needs to have context for next iteration
        PositionHistory.Add(CreatePositionKey());
    }

    private bool IsGameDraw()
    {
        // TODO check for insufficient material
        return HalfMoveCounter >= 150 || IsThreefoldRepetition();
    }

    private bool IsThreefoldRepetition()
    {
        var currentPosition = PositionHistory[^1];

        return PositionHistory.Count(
            position => position == currentPosition) >= 3;
    }
    
    private string CreatePositionKey()
    {
        var positionKey = "";

        for (var row = 0; row < 8; row++)
        {
            var empty = 0;

            for (var col = 0; col < 8; col++)
            {
                var piece = Board.GetPiece(new Position(row, col));

                if (piece is null)
                {
                    empty++;
                    continue;
                }
                if (empty > 0)
                {
                    positionKey += empty;
                    empty = 0;
                }
                positionKey += piece.LetterId;
            }

            if (empty > 0)
                positionKey += empty;

            if (row < 7)
                positionKey += "/";
        }
        positionKey += CurrentTurn == Color.White
            ? " w "
            : " b ";
        positionKey += CastlingRights.ToString();

        if (EnPassantTargetSquare is not null)
        {
            positionKey += $" {EnPassantTargetSquare.ToString()}";
        }
        else
        {
            positionKey += " -";
        }
        
        return positionKey;
    }

    private void UpdateEnPassantTarget(GameSnapshot snapshot, Move move)
    {
        var piece = Board.GetPiece(move.To);

        if (piece is Pawn && Math.Abs(move.To.Row - move.From.Row) == 2)
        {
            EnPassantTargetSquare = new Position(
                (move.From.Row + move.To.Row) / 2,
                move.From.Column);

            return;
        }
        
        EnPassantTargetSquare = null;
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
            MoveHistory,
            EnPassantTargetSquare,
            IsDraw);
    }
    
    private void ApplyMove(GameSnapshot snapshot, Move move, MoveStatus status) 
    {
        if (status.IsCastling)
        {
            HandleCastling(snapshot, move); 
        } 
        else if (status.IsEnPassant)
        {
            HandleEnPassant(move); 
        }
        else 
        { 
            UpdateCastlingRights(move);
            Board.MovePiece(move.From, move.To);

            if (status.IsPromotion)
            {
                HandlePromotion(move);
            }
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
        Board.ReplacePromotion(move.To, move.Promotion, CurrentTurn);
    }

    private void HandleEnPassant(Move move)
    {
        Board.MovePiece(move.From, move.To);

        var capturedPawnPosition = new Position(move.From.Row, move.To.Column);

        Board.RemovePiece(capturedPawnPosition);
    }
}
