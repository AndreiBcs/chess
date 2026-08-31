using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Entities.Player;
using chess.Game.GameState;

namespace Chess.Engine;

public class EnginePlayer : Player
{
    public readonly Uci Uci;

    public EnginePlayer(Uci uci, Color color) : base(color)
    {
        Uci = uci;
    }
    
    public override async Task<Move> GetMoveAsync(GameSnapshot snapshot, MoveResult? previousResult)
    {
        var fen = ToFen(snapshot);
        
        var move = await Uci.GetMove(fen);

        var from = Position.ParsePosition(move[..2]);
        var to = Position.ParsePosition(move[2..4]);

        PieceType? promotion = move.Length switch
        {
            5 => move[4] switch
            {
                'q' => PieceType.Queen,
                'r' => PieceType.Rook,
                'b' => PieceType.Bishop,
                'n' => PieceType.Knight,
                _ => null
            },
            _ => null
        };

        return new Move(from, to, promotion);
    }
    
    private static string ToFen(GameSnapshot snapshot)
    {
        var fen = "";

        for (var row = 0; row < 8; row++)
        {
            var empty = 0;

            for (var col = 0; col < 8; col++)
            {
                var piece = snapshot.Board.GetPiece(new Position(row, col));

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

        fen += snapshot.CurrentTurn == Color.White ? " w " : " b ";
        
        fen += snapshot.Castling.ToString();

        // TODO: en-passant target square
        fen += " - ";

        fen += snapshot.HalfMoveCounter;
        fen += " ";
        fen += snapshot.FullMoveCounter;

        return fen;
    }
}