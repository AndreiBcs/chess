using System.Text;
using chess.Game;

namespace chess.Pgn;

public static class PgnWriter
{
    public static string Write(List<GameSnapshot> snapshots)
    {
        var sb = new StringBuilder();

        foreach (var snapshot in snapshots[1..]) // without initial one
        {
            var status = snapshot.PreviousMoveStatus;
            var move = snapshot.PreviousMove;
            
            sb.Append(snapshot.FullMoveCounter).Append(". ");

            if (status is { IsCastling: true, CastlingRights: not null })
            {
                if (status.CastlingRights?.LetterId.ToString().ToLower() is "k")
                {
                    sb.Append("O-O ");
                }
                else if (status.CastlingRights?.LetterId.ToString().ToLower() is "q")
                {
                    sb.Append("O-O-O ");
                }
            }
            else
            {
                if (status.IsCapture)
                {
                    if (status.IsPawnMove)
                    {
                        sb.Append(move.From.ToString()[0])
                            .Append('x')
                            .Append(move.To.ToString());
                    }
                    else
                    {
                        sb.Append(snapshot.Board.GetPiece(move.From)!.LetterId)
                            .Append('x')
                            .Append(move.To.ToString());
                    }
                }
                else
                {
                    if (status.IsPawnMove)
                    {
                        sb.Append(move.From.ToString());
                    }
                    else
                    {
                        // TODO identical pieces can move to the same position
                        sb.Append(snapshot.Board.GetPiece(move.From)!.LetterId)
                            .Append(move.To.ToString());
                    }
                }

                if (snapshot.Status is not GameStatus.InProgress)
                {
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
                    
                    sb.Append(" 1/2-1/2");
                    break;
                }
                
                if (status.IsCheck)
                {
                    sb.Append("+ ");
                }
                else
                {
                    sb.Append(' ');
                }
            }
        }
        
        return sb.ToString();
    }
}