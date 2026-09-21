using System.Text;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces;
using chess.Validation.MoveValidation;

namespace chess.Pgn;

public static class PgnWriter
{
    public static string Write(List<GameSnapshot> snapshots)
    {
        var sb = new StringBuilder();
        var index = 0;

        foreach (var snapshot in snapshots[1..]) // without initial one
        {
            var status = snapshot.PreviousMoveStatus;
            var move = snapshot.PreviousMove;
            var previousSnapshot = snapshots[index];

            if (index++ % 2 == 0)
            {
                sb.Append(snapshot.FullMoveCounter).Append(". ");
            }

            // castling
            if (status is { IsCastling: true, CastlingRights: not null })
            {
                if (status.CastlingRights?.LetterId.ToString().ToLower() is "k")
                {
                    sb.Append("O-O");
                }
                else if (status.CastlingRights?.LetterId.ToString().ToLower() is "q")
                {
                    sb.Append("O-O-O");
                }
            }
            else
            {
                var piece = previousSnapshot.Board.GetPiece(move.From);

                if (piece is null)
                {
                    sb.Append(" Error constructing SAN... ");
                    break;
                }
                
                // pawn move
                if (status.IsPawnMove)
                {
                    if (status.IsCapture)
                    {
                        sb.Append(move.From.ToString()[0])
                            .Append('x');
                    }

                    sb.Append(move.To.ToString());
                }
                // piece move
                else
                {
                    sb.Append(piece.LetterId.ToString().ToUpper())
                        .Append(GetDisambiguation(previousSnapshot, move, piece));

                    if (status.IsCapture)
                    {
                        sb.Append('x');
                    }

                    sb.Append(move.To.ToString());
                }
                
                // promotion
                if (status.IsPromotion && move.Promotion != null)
                {
                    sb.Append('=')
                        .Append(move.Promotion.Value.ToString().ToUpper()[0]);
                }

                // game status
                if (snapshot.Status is GameStatus.BlackWon)
                {
                    sb.Append("# 0-1");
                    break;
                }
                
                if (snapshot.Status is GameStatus.WhiteWon)
                {
                    sb.Append("# 1-0");
                    break;
                }

                if (snapshot.Status is not GameStatus.InProgress)
                {
                    sb.Append(" 1/2-1/2");
                    break;
                }
                
                if (status.IsCheck)
                {
                    sb.Append('+');
                }
            }
            
            sb.Append(' ');
        }
        
        return sb.ToString();
    }
    
    private static string GetDisambiguation(
        GameSnapshot previousSnapshot,
        Move move,
        Piece piece)
    {
        var candidates = previousSnapshot.Board
            .PositionsOfPiecesWithSameTypeAndColor(piece.Type, piece.Color)
            .Where(pos => pos != move.From)
            .Where(pos => MoveValidator.ValidateMove(
                previousSnapshot,
                new Move(pos, move.To)).MoveResult == MoveResult.Valid)
            .ToList();

        if (candidates.Count == 0)
            return string.Empty;

        var sameFileExists = candidates.Any(p => p.Column == move.From.Column);
        var sameRankExists = candidates.Any(p => p.Row == move.From.Row);

        if (sameFileExists && sameRankExists)
            return move.From.ToString(); // file + rank

        if (sameFileExists)
            return $"{8 - move.From.Row}"; // rank only

        if (sameRankExists)
            return $"{(char)('a' + move.From.Column)}"; // file only

        return $"{(char)('a' + move.From.Column)}"; // file is enough
    }
}