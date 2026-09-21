using System.Collections.Immutable;
using chess.Board;
using chess.Moves;
using chess.Pieces;
using chess.Validation.MoveValidation;
using chess.Validation.StateValidation;

namespace chess.Game;

public sealed record GameSnapshot
{
    public readonly GameStatus Status;
    public readonly Board.Board Board;
    public readonly Color CurrentTurn;
    public readonly ImmutableList<CastlingRights> CastlingRights;
    public readonly Position? EnPassantTarget;
    public readonly int HalfMoveClock;
    public readonly int FullMoveCounter;
    public readonly Move PreviousMove;
    public readonly MoveStatus PreviousMoveStatus;
    public readonly ImmutableList<string> PositionHistory;

    private GameSnapshot( 
        GameStatus status,
        Board.Board board,
        Color currentTurn,
        ImmutableList<CastlingRights> castlingRights,
        Position? enPassantTarget,
        int halfMoveClock,
        int fullMoveCounter,
        Move previousMove,
        MoveStatus previousMoveStatus,
        ImmutableList<string> positionHistory)
    {
        Status = status;
        Board = board;
        CurrentTurn = currentTurn;
        CastlingRights = castlingRights;
        EnPassantTarget = enPassantTarget;
        HalfMoveClock = halfMoveClock;
        FullMoveCounter = fullMoveCounter;
        PreviousMove = previousMove;
        PositionHistory = positionHistory;
        PreviousMoveStatus = previousMoveStatus;
    }
    
    public static GameSnapshot GetInitialGameSnapshot()
    {
        const GameStatus status = GameStatus.InProgress;
        var board = chess.Board.Board.CreateInitial();
        const Color currentTurn = Color.White;
        var castlingRightsList = chess.Game.CastlingRights.GetInitialCastlingRights().ToImmutableList();
        const int halfMoveClock = 0;
        const int fullMoveCounter = 1;
        Position? enPassantTarget = null;
        var previousMove = new Move(new Position(0, 0), new Position(0, 0));
        var previousStatus = new MoveStatus(MoveResult.Valid);
        var positionHistory = new List<string>().ToImmutableList();

        return new GameSnapshot(
            status,
            board, 
            currentTurn,
            castlingRightsList, 
            enPassantTarget, 
            halfMoveClock, 
            fullMoveCounter, 
            previousMove,
            previousStatus,
            positionHistory);
    }
    
    public static string ToFen(
        Board.Board board,
        Color currentTurn,
        ImmutableList<CastlingRights> castlingRights,
        Position? enPassantTarget,
        int halfMoveClock,
        int fullMoveCounter)
    {
        var fen = "";
        for (var row = 0; row < 8; row++)
        {
            var empty = 0;
            for (var col = 0; col < 8; col++)
            {
                var piece = board.GetPiece(new Position(row, col));

                if (piece is null)
                {
                    empty++;
                    continue;
                }
                if (empty > 0)
                {
                    fen += empty;
                    empty = 0;
                }
                fen += piece.LetterId;
            }

            if (empty > 0) fen += empty;
            
            if (row < 7) fen += "/";
        }
        fen += currentTurn == Color.White ? " w " : " b ";
        
        if (castlingRights.Count > 0)
        {
            var rights = "";
            
            foreach (var castling in castlingRights)
            {
                rights += string.Join(' ', castling.LetterId);
            }
            
            fen += rights.Trim();
        } 
        else
        {
            fen += "-";
        }
        
        fen += enPassantTarget is not null 
            ? $" {enPassantTarget.ToString()} " 
            : " - ";
        
        return fen + $"{halfMoveClock} {fullMoveCounter}";
    }

    public static GameSnapshot GetUpdatedGameSnapshot(
        GameSnapshot previousSnapshot,
        Move currentMove,
        MoveStatus moveStatus)
    {
        // update previous move
        var previousMove = currentMove;
        // update current turn
        var currentTurn = previousSnapshot.CurrentTurn == Color.White 
            ? Color.Black 
            : Color.White;

        var board = previousSnapshot.Board.CopyBoard();
        
        // update board after move
        if (moveStatus.IsCastling)
        {
            var kingFrom = moveStatus.CastlingRights!.Value.KingFrom;
            var kingTo = moveStatus.CastlingRights.Value.KingTo;
            var rookFrom = moveStatus.CastlingRights.Value.RookFrom;
            var rookTo = moveStatus.CastlingRights.Value.RookTo;
            board = board
                .WithMove(kingFrom, kingTo)
                .WithMove(rookFrom, rookTo);
            
        }
        else if (moveStatus.IsEnPassant)
        {
            var capturedPawnPos = new Position(currentMove.From.Row, currentMove.To.Column);
            board = board
                .WithMove(currentMove.From, currentMove.To)
                .WithoutPiece(capturedPawnPos);
        }
        else
        {
            if (moveStatus.IsPromotion && currentMove.Promotion != null)
            {
                board = board
                    .WithMove(currentMove.From, currentMove.To)
                    .WithPromotion(
                        currentMove.To,
                        currentMove.Promotion.Value,
                        previousSnapshot.CurrentTurn);
            }
            else
            {
                board = board.WithMove(currentMove.From, currentMove.To);
            }
        }
        
        // update castling rights
        var currentMovedPiece = previousSnapshot.Board.GetPiece(currentMove.From);
        var capturedPiece = previousSnapshot.Board.GetPiece(currentMove.To);

        var castlingRights = currentMovedPiece!.Type switch
        {
            PieceType.King => previousSnapshot.CastlingRights
                .Where(c => 
                    c.Color != previousSnapshot.CurrentTurn)
                .ToImmutableList(),
                
            PieceType.Rook => previousSnapshot.CastlingRights
                .Where(c => 
                    !(c.Color == previousSnapshot.CurrentTurn &&
                      c.RookFrom == currentMove.From))
                .ToImmutableList(),
            
            _ => previousSnapshot.CastlingRights
        };

        if (capturedPiece?.Type == PieceType.Rook) 
        {
            castlingRights = castlingRights
                .Where(c => 
                    !(c.Color == capturedPiece.Color && 
                      c.RookFrom == currentMove.To))
                .ToImmutableList();
        }
        
        // update half move clock
        var halfMoveClock = moveStatus.IsCapture || moveStatus.IsPawnMove 
            ? 0 
            : previousSnapshot.HalfMoveClock + 1;

        // update full move counter
        var fullMoveCounter = previousSnapshot.CurrentTurn == Color.Black 
            ? previousSnapshot.FullMoveCounter + 1 
            : previousSnapshot.FullMoveCounter;
        
        // update en passant target square
        var lastMovedPiece = previousSnapshot.Board.GetPiece(currentMove.From);
        
        Position? enPassantTarget = lastMovedPiece is { Type: PieceType.Pawn, HasMoved: false }
                                    && Math.Abs(currentMove.To.Row - currentMove.From.Row) == 2
            ? currentMove.From with { Row = (currentMove.From.Row + currentMove.To.Row) / 2 }
            : null;

        
        // update position history
        var newPosition = ToFen(
            board,
            currentTurn,
            castlingRights,
            enPassantTarget,
            halfMoveClock,
            fullMoveCounter);
        
        var positionHistory = previousSnapshot.PositionHistory.Add(newPosition);
        
        // update previous move status
        moveStatus = CheckValidator.IsKingInCheck(board, currentTurn) 
            ? moveStatus with { IsCheck = true } 
            : moveStatus;

        // update game status
        var tempSnapshot = new GameSnapshot(
            GameStatus.InProgress,
            board,
            currentTurn,
            castlingRights,
            enPassantTarget,
            halfMoveClock,
            fullMoveCounter,
            previousMove,
            moveStatus,
            positionHistory);
        
        var gameStatus = StateValidator.ValidateState(tempSnapshot);

        return new GameSnapshot(
            gameStatus,
            board,
            currentTurn,
            castlingRights,
            enPassantTarget,
            halfMoveClock,
            fullMoveCounter,
            previousMove,
            moveStatus,
            positionHistory);
    }

    public static GameSnapshot CreateCustomSnapshot(
        GameStatus? gameStatus,
        Board.Board board,
        Color currentTurn,
        List<CastlingRights>? castlingRights,
        Position? enPassantTarget,
        int? halfMoveClock,
        int? fullMoveCounter,
        Move? previousMove,
        List<string>? positionHistory)
    {
        return new GameSnapshot(
            gameStatus ?? GameStatus.InProgress,
            board,
            currentTurn,
            castlingRights?.ToImmutableList() ?? ImmutableList<CastlingRights>.Empty,
            enPassantTarget,
            halfMoveClock ?? 0,
            fullMoveCounter ?? 1,
            previousMove ?? new Move(new Position(0, 0), new Position(0, 0)),
            new MoveStatus(MoveResult.Valid),
            positionHistory?.ToImmutableList() ?? ImmutableList<string>.Empty);
    }
}