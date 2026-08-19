using Chess.Cli.GameRunner;

namespace Chess.Cli.Board;

public class TextBoardRenderer : IBoardRenderer
{
    public void Render(chess.Entities.Board.Board board)
    {
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                Console.Write(board.Squares[i, j].Piece != null
                    ? board.Squares[i, j].Piece?.LetterId + "  "
                    : "_  ");
            }

            Console.WriteLine();
        }
    }
}