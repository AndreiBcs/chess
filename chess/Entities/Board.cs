namespace chess.Entities;

public class Board
{
    private Square[][] Squares { get; set; } = new Square[8][];

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
                else
                {
                    Squares[i][j].Color = SquareColor.Black;
                }
            }
        }
        PlaceStartingPieces(white, black);
    }

    private void PlaceStartingPieces(Player white, Player black)
    {
        Squares[0][0].Piece = black.Rooks[0];
        Squares[0][7].Piece = black.Rooks[1];
        
        Squares[0][1].Piece = black.Knights[0];
        Squares[0][6].Piece = black.Knights[1];
        
        Squares[0][2].Piece = black.Bishops[0];
        Squares[0][5].Piece = black.Bishops[0];
        
        Squares[0][3].Piece = black.Queen;
        Squares[0][4].Piece = black.King;

        for (var i = 0; i < 8; i++)
        {
            Squares[1][i].Piece = black.Pawns[i];
        }
        
        // white
        Squares[7][0].Piece = white.Rooks[0];
        Squares[7][7].Piece = white.Rooks[1];
        
        Squares[7][1].Piece = white.Knights[0];
        Squares[7][6].Piece = white.Knights[1];
        
        Squares[7][2].Piece = white.Bishops[0];
        Squares[7][5].Piece = white.Bishops[0];
        
        Squares[7][3].Piece = white.Queen;
        Squares[7][4].Piece = white.King;
        
        for (var i = 0; i < 8; i++)
        {
            Squares[6][i].Piece = white.Pawns[i];
        }
    }

    public void PrintBoard()
    {
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                Console.Write(Squares[i][j].Piece != null ? 
                    Squares[i][j].Piece?.LetterId + "  " :
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

