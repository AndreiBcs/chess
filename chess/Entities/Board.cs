using chess.Entities.Pieces;

namespace chess.Entities;

public class Board
{
    private Square[][] Squares { get; set; }

    public void InitializeStartingBoard(Player white, Player black)
    {
        Squares = new Square[8][];
        
        for (var i = 0; i < 8; i++)
        {
            Squares[i] = new Square[8];
            for (var j = 0; j < 8; j++)
            {
                var ascii = 'a' + j;
                var letterIndex = (char)ascii;
                var numberIndex = 8 - i;

                Squares[i][j] = new Square
                {
                    Position = $"{letterIndex.ToString()}{numberIndex.ToString()}"
                };

                if (i % 2 == 0 && j % 2 == 0)
                {
                    Squares[i][j].Color = SquareColor.White;
                }
                
                // black pieces
                // rooks
                if (i == 0 && j == 0)
                {
                    Squares[i][j].Piece = black.Rooks[0];
                }
                if (i == 0 && j == 7)
                {
                    Squares[i][j].Piece = black.Rooks[1];
                }
                // knights
                if (i == 0 && j == 1)
                {
                    Squares[i][j].Piece = black.Knights[0];
                }
                if (i == 0 && j == 7)
                {
                    Squares[i][j].Piece = black.Knights[1];
                }
                // bishops
                if (i == 0 && j == 2)
                {
                    Squares[i][j].Piece = black.Bishops[0];
                }
                if (i == 0 && j == 5)
                {
                    Squares[i][j].Piece = black.Bishops[1];
                }
                // queen
                if (i == 0 && j == 5)
                {
                    Squares[i][j].Piece = black.Queen;
                }
                // king
                if (i == 0 && j == 4)
                {
                    Squares[i][j].Piece = black.King;
                }
                // pawns
                if(i == 1)
                {
                    Squares[i][j].Piece = black.Pawns[j];
                }
                
                
                Console.Write(Squares[i][j].Piece != null ? 
                    Squares[i][j].Piece?.LetterId.ToString() + ' ' :  
                    Squares[i][j].Position + ' ');
            }
            Console.WriteLine();
        }
    }
}

public record Square
{
    public SquareColor Color { get; set; }  
    public string Position { get; set; }
    public Piece? Piece { get; set; }
}

public enum SquareColor
{
    White,
    Black
}

