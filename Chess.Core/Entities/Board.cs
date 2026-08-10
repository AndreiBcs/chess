namespace chess.Entities;

public class Board
{
    public Square[,] Squares { get; set; } = new Square[8, 8];

    public void InitializeStartingBoard(Player white, Player black, bool isUserWhite)
    {
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                Squares[i,j] = new Square
                {
                    Position = $"{(char)('a' + j)}{8 - i}",
                    BoardPosition = (i, j),
                    Color = (i + j) % 2 == 0 ? Color.White : Color.Black
                };
            }
        }
        PlaceStartingPieces(white, black, isUserWhite);
    }

    private void PlaceStartingPieces(Player white, Player black, bool isUserWhite)
    {
        if (isUserWhite)
        {
            SetupPlayerSide(black, majorRow: 0, pawnRow: 1, true);
            SetupPlayerSide(white, majorRow: 7, pawnRow: 6, true);
        }
        else
        {
            SetupPlayerSide(black, majorRow: 7, pawnRow: 6, false);
            SetupPlayerSide(white, majorRow: 0, pawnRow: 1, false);
        }
    }
    
    private void SetupPlayerSide(Player player, int majorRow, int pawnRow, bool isUserWhite)
    {
        Squares[majorRow, 0].Piece = player.Rooks[0];
        Squares[majorRow, 7].Piece = player.Rooks[1];
        
        Squares[majorRow, 1].Piece = player.Knights[0];
        Squares[majorRow, 6].Piece = player.Knights[1];
        
        Squares[majorRow, 2].Piece = player.Bishops[0];
        Squares[majorRow, 5].Piece = player.Bishops[1];

        if (isUserWhite)
        {
            Squares[majorRow, 3].Piece = player.Queen;
            Squares[majorRow, 4].Piece = player.King;
        }
        else
        {
            Squares[majorRow, 4].Piece = player.Queen;
            Squares[majorRow, 3].Piece = player.King;
        }

        for (var i = 0; i < 8; i++)
        {
            Squares[pawnRow, i].Piece = player.Pawns[i];
        }
    }
    
}

public record struct Square
{
    public Color Color { get; init; }  
    public string Position { get; init; }
    public (int Row, int Col) BoardPosition { get; init; }
    public Piece? Piece { get; set; }
}

