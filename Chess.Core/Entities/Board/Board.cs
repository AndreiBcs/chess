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
        Squares[to.Row, to.Column].Piece = Squares[from.Row, from.Column].Piece;
        Squares[to.Row, to.Column].Piece!.Position = new Position(to.Row, to.Column);
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
        Squares[majorRow, 0].Piece = new Rook
        {
            Color = color, 
            Position = new Position(majorRow, 0)
        };
        Squares[majorRow, 1].Piece = new Knight
        {
            Color = color,
            Position = new Position(majorRow, 1)
        };
        Squares[majorRow, 2].Piece = new Bishop
        {
            Color = color,
            Position = new Position(majorRow, 2)
        };
        Squares[majorRow, 3].Piece = new Queen
        {
            Color = color,
            Position = new Position(majorRow, 3)
        };
        Squares[majorRow, 4].Piece = new King
        {
            Color = color,
            Position = new Position(majorRow, 4)
        };
        Squares[majorRow, 5].Piece = new Bishop
        {
            Color = color,
            Position = new Position(majorRow, 5)
        };
        Squares[majorRow, 6].Piece = new Knight
        {
            Color = color,
            Position = new Position(majorRow, 6)
        };
        Squares[majorRow, 7].Piece = new Rook
        {
            Color = color,
            Position = new Position(majorRow, 7)
        };

        for (var i = 0; i < 8; i++)
        {
            Squares[pawnRow, i].Piece = new Pawn
            {
                Color = color,
                Position = new Position(pawnRow, i)
            };
        }
    }
    
}

