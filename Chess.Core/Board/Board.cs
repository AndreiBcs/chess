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
        
        Squares[to.Row, to.Column].Piece = piece;
        Squares[from.Row, from.Column].Piece = null;
    }

    public void RemovePiece(Position position)
    {
        var piece = GetPiece(position);

        Squares[position.Row, position.Column].Piece = null;
    }

    public void ReplacePromotion(
        Position position, 
        PieceType? promotion,
        Color promotionColor)
    {
        if (promotion is null) return;
        
        RemovePiece(position);
        
        Squares[position.Row, position.Column].Piece = promotion switch
        {
            PieceType.Bishop => new Bishop(promotionColor),
            PieceType.Knight => new Knight(promotionColor),
            PieceType.Rook => new Rook(promotionColor),
            PieceType.Queen => new Queen(promotionColor),
            _ => throw new ArgumentException(
                "Invalid promotion piece",
                nameof(promotion))
        };
    }
    
    public Board Copy()
    {
        var board = new Board();

        for (var row = 0; row < 8; row++)
        {
            for (var column = 0; column < 8; column++)
            {
                var originalSquare = Squares[row, column];

                board.Squares[row, column] = 
                    originalSquare with { Position = new Position(row, column) };
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

    public bool BishopsAreSameColor()
    {
        var bishopSquares = 
            (from Square square in Squares 
                where square.Piece?.Type == PieceType.Bishop 
                select square).ToList();
        
        if(bishopSquares.Count != 2)
            return false;
        
        var first = bishopSquares[0].Position;
        var second = bishopSquares[1].Position;

        var firstColor = (first.Row + first.Column) % 2;
        var secondColor = (second.Row + second.Column) % 2;

        return firstColor == secondColor;
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