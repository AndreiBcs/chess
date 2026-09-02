using chess.Board;
using chess.Moves;

namespace chess.Game;

public sealed record GameState(
    Board.Board Board,
    Color CurrentTurn,
    List<CastlingRights> CastlingRights,
    Position? EnPassantTarget,
    int HalfMoveClock,
    int FullMoveCounter,
    Move PreviousMove)
{
    public MoveResult TryMakeMove(Move move)
    {
        
    }

    public GameState ApplyMove(Move move)
    {
        
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

        if (EnPassantTarget is not null)
        {
            fen += $" {EnPassantTarget.ToString()} ";
        }
        else
        {
            fen += " - ";
        }

        return fen + $"{HalfMoveClock} {FullMoveCounter}";
    }
}