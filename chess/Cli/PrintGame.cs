using chess.Entities;

namespace chess.Cli;

public static class PrintGame
{
    public static void PrintBoard(this Board Board)
    {
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                Console.Write(Board.Squares[i,j].Piece != null ? 
                    Board.Squares[i,j].Piece?.LetterId + "  " :
                    Board.Squares[i,j].Position + ' ');
            }
            Console.WriteLine();
        }
    }
}