using System.Collections.Immutable;
using chess.Board;
using chess.Moves;
using chess.Pieces;

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
    }
    
    public GameSnapshot GetInitialGameSnapshot()
    {
        const GameStatus status = GameStatus.InProgress;
        var board = chess.Board.Board.CreateInitial();
        const Color currentTurn = Color.White;
        var castlingRightsList = new List<CastlingRights>
        {
            new(
                'K',
                Color.White,
                new Position(7, 4), new Position(7, 6),
                new Position(7, 7), new Position(7, 5),
                [new Position(7, 4), new Position(7, 5), new Position(7, 6)]
            ),
            new(
                'Q',
                Color.White,
                new Position(7, 4), new Position(7, 2),
                new Position(7, 0), new Position(7, 3),
                [new Position(7, 4), new Position(7, 3), new Position(7, 2)]
            ),
            new(
                'k',
                Color.Black,
                new Position(0, 4), new Position(0, 6),
                new Position(0, 7), new Position(0, 5),
                [new Position(0, 4), new Position(0, 5), new Position(0, 6)]
            ),
            new(
                'q',
                Color.Black,
                new Position(0, 4), new Position(0, 2),
                new Position(0, 0), new Position(0, 3),
                [new Position(0, 4), new Position(0, 3), new Position(0, 2)]
            )
        }.ToImmutableList();
        const int halfMoveClock = 0;
        const int fullMoveCounter = 1;
        Position? enPassantTarget = null;
        var previousMove = new Move(new Position(0, 0), new Position(0, 0));
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
            positionHistory);
    }
    
    public string ToFen()
    {
        var fen = "";
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
                    fen += empty;
                    empty = 0;
                }
                fen += piece.LetterId;
            }

            if (empty > 0) fen += empty;
            
            if (row < 7) fen += "/";
        }
        fen += CurrentTurn == Color.White ? " w " : " b ";
        fen += CastlingRights.ToString();
        fen += EnPassantTarget is not null ? $" {EnPassantTarget.ToString()} " : " - ";
        
        return fen + $"{HalfMoveClock} {FullMoveCounter}";
    }

    public GameSnapshot GetUpdatedGameSnapshot(
        GameSnapshot previousSnapshot,
        Move currentMove,
        MoveStatus moveStatus)
    {
        var previousMove = currentMove;
        var currentTurn = previousSnapshot.CurrentTurn == Color.White 
            ? Color.Black 
            : Color.White;
        
        if (moveStatus.IsCastling)
        {
            var kingFrom = moveStatus.CastlingRights!.Value.KingFrom;
            var kingTo = moveStatus.CastlingRights.Value.KingTo;
            var rookFrom = moveStatus.CastlingRights.Value.RookFrom;
            var rookTo = moveStatus.CastlingRights.Value.RookTo;
            var castlingBoard = previousSnapshot.Board
                .WithMove(kingFrom, kingTo)
                .WithMove(rookFrom, rookTo);
            
        }
        else if (moveStatus.IsEnPassant)
        {
            var capturedPawnPos = new Position(currentMove.From.Row, currentMove.To.Column);
            var enPassantBoard = previousSnapshot.Board
                .WithMove(currentMove.From, currentMove.To)
                .WithoutPiece(capturedPawnPos);
        }
        else
        {
            // move piece
            var normalBoard = previousSnapshot.Board.WithMove(currentMove.From, currentMove.To);
            
            if (moveStatus.IsPromotion && currentMove.Promotion != null)
            {
                var promotionBoard = previousSnapshot.Board
                    .WithPromotion(currentMove.To, currentMove.Promotion.Value, CurrentTurn);
            }
        }
        
        // castling rights
        var movedPiece = previousSnapshot.Board.GetPiece(currentMove.From);
        var capturedPiece = previousSnapshot.Board.GetPiece(currentMove.To);

        var castlingRights = movedPiece!.Type switch
        {
            PieceType.King => previousSnapshot.CastlingRights
                .Where(c => c.Color == previousSnapshot.CurrentTurn)
                .ToImmutableList(),
                
            PieceType.Rook => previousSnapshot.CastlingRights
                .Where(c => 
                    c.Color == previousSnapshot.CurrentTurn &&
                    c.RookFrom == currentMove.From)
                .ToImmutableList()
        };

        if (capturedPiece?.Type == PieceType.Rook)
        {
            castlingRights.Where(c =>
                    c.Color != previousSnapshot.CurrentTurn &&
                    c.RookFrom == currentMove.To)
                .ToImmutableList();
        }
        
        var halfMoveClock = moveStatus.IsCapture || moveStatus.IsPawnMove 
            ? 0 
            : previousSnapshot.HalfMoveClock + 1;

        var fullMoveCounter = CurrentTurn == Color.Black 
            ? FullMoveCounter + 1 
            : FullMoveCounter;
        
        var pieceMoved = Board.GetPiece(currentMove.From);
        
        Position? enPassantTarget = pieceMoved?.Type == PieceType.Pawn
                                    && Math.Abs(currentMove.To.Row - currentMove.From.Row) == 2
            ? currentMove.From with { Row = (currentMove.From.Row + currentMove.To.Row) / 2 }
            : null;

        var positionHistory = PositionHistory.Add(ToFen());
        
        // TODO check if updated snapshot is checkmate/stalemate/draw

        return null;
    }
}