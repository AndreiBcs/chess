using chess.Pieces;
using chess.Pieces.Types;

namespace chess.Board;

public class Board : IReadOnlyBoard
{
    private Square[,] Squares { get; } = new Square[8, 8];

    public void InitializeBoard()
    {
        InitializeSquares();
        SetupPlayerSide(Color.White, 7, 6);
        SetupPlayerSide(Color.Black, 0, 1);
    }
    
    public Piece? GetPiece(Position position)
    {
        if (position.Row is < 0 or >= 8 ||
            position.Column is < 0 or >= 8)
            return null;
        
        return Squares[position.Row, position.Column].Piece;
    }

    public void MovePiece(Position from, Position to)
    {
        var piece = GetPiece(from);

        var capturedPiece = GetPiece(to);
        capturedPiece?.MarkAsCaptured();
        
        Squares[to.Row, to.Column].Piece = piece;
        Squares[from.Row, from.Column].Piece = null;

        if (piece is IMoveTracker moveTracker)
        {
            moveTracker.MarkAsMoved();
        }
    }
    
    public Board Copy()
    {
        var board = new Board();

        for (var row = 0; row < 8; row++)
        {
            for (var column = 0; column < 8; column++)
            {
                var originalSquare = Squares[row, column];

                board.Squares[row, column] = new Square
                {
                    Color = originalSquare.Color,
                    Position = new Position(row, column),
                    Piece = originalSquare.Piece?.Copy()
                };
            }
        }

        return board;
    }
    
    public Position GetKingPosition(Color color)
    {
        var pos = new Position();
        foreach (var sq in Squares)
        {
            if (sq.Piece?.Color == color &&
                sq.Piece.Type == PieceType.King)
            {
                pos = sq.Position;
            }
        }

        return pos;
    }

    public Square[,] GetSquares()
    {
        return Squares;
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
        Squares[majorRow, 0].Piece = new Rook(color, false);
        Squares[majorRow, 1].Piece = new Knight(color);
        Squares[majorRow, 2].Piece = new Bishop(color);
        Squares[majorRow, 3].Piece = new Queen(color);
        Squares[majorRow, 4].Piece = new King(color, false);
        Squares[majorRow, 5].Piece = new Bishop(color);
        Squares[majorRow, 6].Piece = new Knight(color);
        Squares[majorRow, 7].Piece = new Rook(color, false);

        for (var i = 0; i < 8; i++)
        {
            Squares[pawnRow, i].Piece = new Pawn(color, false);
        }
    }
}