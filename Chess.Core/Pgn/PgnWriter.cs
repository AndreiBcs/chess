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
            
            // todo
        }
        
        return sb.ToString();
    }
}