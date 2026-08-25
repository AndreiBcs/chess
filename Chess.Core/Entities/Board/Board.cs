using chess.Entities.Common;
using chess.Entities.Pieces.Types;

namespace chess.Entities.Board;

public class Board
{
    public Square[,] Squares { get; } = new Square[8, 8];

    public void InitializeBoard()
    {
        InitializeSquares();
        SetupPlayerSide(Color.White, 7, 6);
        SetupPlayerSide(Color.Black, 0, 1);
    }

    public void MovePiece(Position from, Position to)
    {
        var piece = Squares[from.Row, from.Column].Piece;

        if (piece is null)
            throw new InvalidOperationException(
                "Cannot move a piece from an empty square.");

        Squares[to.Row, to.Column].Piece = piece;
        Squares[from.Row, from.Column].Piece = null;
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
    
    private void SetupPlayerSide(Color color, int majorRow, int pawnRow)
    {
        Squares[majorRow, 0].Piece = new Rook(color);
        Squares[majorRow, 1].Piece = new Knight(color);
        Squares[majorRow, 2].Piece = new Bishop(color);
        Squares[majorRow, 3].Piece = new Queen(color);
        Squares[majorRow, 4].Piece = new King(color);
        Squares[majorRow, 5].Piece = new Bishop(color);
        Squares[majorRow, 6].Piece = new Knight(color);
        Squares[majorRow, 7].Piece = new Rook(color);

        for (var i = 0; i < 8; i++)
        {
            Squares[pawnRow, i].Piece = new Pawn(color);
        }
    }
    
}

