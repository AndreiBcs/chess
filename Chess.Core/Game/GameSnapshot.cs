using System.Collections.Immutable;
using chess.Board;
using chess.Moves;

namespace chess.Game;

public sealed class GameSnapshot
{
    public GameStatus GameStatus { get; }
    public Color CurrentTurn { get; }
    public IReadOnlyBoard Board { get; }
    public int FullMoveCounter { get; }
    public int HalfMoveCounter { get; }
    public CastlingRights CastlingRights { get; }
    public ImmutableList<Move> MoveHistory { get; }
    public Position? EnPassantTargetSquare { get; }

    public GameSnapshot(
        GameStatus gameStatus,
        Color currentTurn, 
        Board.Board board, 
        int fullMoveCounter, 
        int halfMoveCounter,
        CastlingRights castlingRights,
        List<Move> moveHistory, 
        Position? enPassantTarget)
    {
        CurrentTurn = currentTurn;
        Board = board;
        FullMoveCounter = fullMoveCounter;
        HalfMoveCounter = halfMoveCounter;
        CastlingRights = castlingRights;
        EnPassantTargetSquare = enPassantTarget;
        GameStatus = gameStatus;
        MoveHistory = moveHistory.ToImmutableList();
    }
    
    public string ToFen(bool justPositionKey = false)
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

            if (empty > 0)
                fen += empty;

            if (row < 7)
                fen += "/";
        }

        fen += CurrentTurn == Color.White ? " w " : " b ";
        
        fen += CastlingRights.ToString();

        if (EnPassantTargetSquare is not null)
        {
            fen += $" {EnPassantTargetSquare.ToString()} ";
        }
        else
        {
            fen += " - ";
        }

        if (!justPositionKey) 
        {
            // because position history doesn't need the move counter
            // so it can match identical positions when check for draw
            fen += AddMoveCounter();
        }

        return fen;
    }

    private string AddMoveCounter()
    {
        return $"{HalfMoveCounter} {FullMoveCounter}";
    }
}