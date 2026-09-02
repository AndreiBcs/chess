using chess.Board;
using chess.Moves;
using chess.Pieces;
using chess.Pieces.Types;
using chess.Validation;

namespace chess.Game;

public class OldGame
{
    public OldGame(Player.Player player1, Player.Player player2)
    {
        Players = [player1, player2];
        Board.InitializeBoard();

        GameStatus = GameStatus.InProgress;
        PositionHistory.Add(CreateSnapshot().ToFen(true));
    }

    private bool IsOver { get; set; }
    public bool IsFinished => IsOver;
    private GameStatus GameStatus { get; set; }
    public GameStatus Status => GameStatus;
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
                
                UpdateGameState(snapshot, move, status);

                switch (result)
                {
                    case MoveResult.Checkmate:
                        IsOver = true;
                        GameStatus = CurrentTurn == Color.White
                            ? GameStatus.WhiteWon
                            : GameStatus.BlackWon;
                        break;

                    case MoveResult.Stalemate:

                    case MoveResult.Valid when IsGameDraw():
                        IsOver = true;
                        GameStatus = GameStatus.Draw;
                        break;
                }
                
                break;
            }
        }

        yield return CreateSnapshot(); // return the game end snapshot
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
        PositionHistory.Add(snapshot.ToFen(true));
    }

    private bool IsGameDraw()
    {
        return HalfMoveCounter >= 150
               || IsThreefoldRepetition()
               || IsInsufficientMaterial();
    }

    private bool IsThreefoldRepetition()
    {
        var currentPosition = PositionHistory[^1];

        return PositionHistory.Count(
            position => position == currentPosition) >= 3;
    }

    private bool IsInsufficientMaterial()
    {
        var pieces = new List<Piece>();

        foreach (var square in Board.GetSquares())
        {
            if (square.Piece is not null)
            {
                pieces.Add(square.Piece);
            }
        }

        if (pieces.All(piece => piece.Type == PieceType.King))
        {
            return true;
        }

        if (pieces.Any(piece =>
                piece.Type is PieceType.Pawn or PieceType.Rook or PieceType.Queen))
        {
            return false;
        }
        
        // only bishops, knights and kings remain
        var nonKings = pieces
            .Where(piece => piece.Type != PieceType.King).ToList();

        // king + knight vs king
        if (nonKings.Count == 1 && nonKings[0].Type == PieceType.Knight)
        {
            return true;
        }
        
        // king + bishop vs king
        if (nonKings.Count == 1 && nonKings[0].Type == PieceType.Bishop)
        {
            return true;
        }

        // king + bishop vs king + bishop
        if (nonKings.Count == 2 &&
            nonKings.All(piece => piece.Type == PieceType.Bishop))
        {
            // if they are on the same color is a draw
            return Board.BishopsAreSameColor();
        }
        
        return false;
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
        return new GameSnapshot(GameStatus, CurrentTurn, Board, FullMoveCounter, HalfMoveCounter, CastlingRights, MoveHistory, EnPassantTargetSquare);
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
