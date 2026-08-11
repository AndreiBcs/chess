using chess.Entities.Common;
using chess.Entities.Pieces.Types;

namespace chess.Entities.Board;

public class Board
{
    public Square[,] Squares { get; } = new Square[8, 8];

    public void InitializeBoard(Player.Player white, Player.Player black)
    {
        InitializeSquares();
        SetupPlayerSide(white, 7, 6);
        SetupPlayerSide(black, 0, 1);
    }

    private void InitializeSquares()
    {
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                Squares[row, col] = new Square
                {
                    Color = (row + col) % 2 == 0
                        ? Color.White
                        : Color.Black,
                    Position = new Position(row, col)
                };
            }
        }
    }
    
    private void SetupPlayerSide(Player.Player player, int majorRow, int pawnRow)
    {
        var rooks = player.ActivePieces.OfType<Rook>().ToArray();
        var pawns = player.ActivePieces.OfType<Pawn>().ToArray();
        var knights = player.ActivePieces.OfType<Knight>().ToArray();
        var bishops = player.ActivePieces.OfType<Bishop>().ToArray();
        var queen = player.ActivePieces.OfType<Queen>().Single();
        var king = player.ActivePieces.OfType<King>().Single();
        
        Squares[majorRow, 0].Piece = rooks[0];
        Squares[majorRow, 1].Piece = knights[0];
        Squares[majorRow, 2].Piece = bishops[0];Squares[majorRow, 3].Piece = queen;
        Squares[majorRow, 4].Piece = king;
        Squares[majorRow, 5].Piece = bishops[1];
        Squares[majorRow, 6].Piece = knights[1];
        Squares[majorRow, 7].Piece = rooks[1];

        for (var i = 0; i < 8; i++)
        {
            Squares[pawnRow, i].Piece = pawns[i];
        }
    }

    public string ToFen()
    {
        // TODO
        return "";
    }
    
}

