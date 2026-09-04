using chess.Board;
using chess.Moves;

namespace chess.Game;

public sealed record GameSnapshot
{
    public readonly GameStatus Status;
    public readonly Board.Board Board;
    public readonly Color CurrentTurn;
    public readonly List<CastlingRights> CastlingRights;
    public readonly Position? EnPassantTarget;
    public readonly int HalfMoveClock;
    public readonly int FullMoveCounter;
    public readonly Move PreviousMove;

    private GameSnapshot(
        GameStatus status,
        Board.Board board,
        Color currentTurn,
        List<CastlingRights> castlingRights,
        Position? enPassantTarget,
        int halfMoveClock,
        int fullMoveCounter,
        Move previousMove)
    {
        Status = status;
        Board = board;
        CurrentTurn = currentTurn;
        CastlingRights = castlingRights;
        EnPassantTarget = enPassantTarget;
        HalfMoveClock = halfMoveClock;
        FullMoveCounter = fullMoveCounter;
        PreviousMove = previousMove;
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
        };
        const int halfMoveClock = 0;
        const int fullMoveCounter = 1;
        Position? enPassantTarget = null;
        var previousMove = new Move(new Position(0, 0), new Position(0, 0));

        return new GameSnapshot(
            status,
            board, 
            currentTurn,
            castlingRightsList, 
            enPassantTarget, 
            halfMoveClock, 
            fullMoveCounter, 
            previousMove);
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
}