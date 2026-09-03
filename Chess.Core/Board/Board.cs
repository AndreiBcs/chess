using chess.Pieces;
using chess.Pieces.Types;

namespace chess.Board;

public sealed record Board : IReadOnlyBoard
{
    private readonly Square[,] _squares;
    private Board(Square[,] squares)
    {
        _squares = squares;
    }
    
    public Piece? GetPiece(Position position)
    {
        if (position.Row is < 0 or >= 8 ||
            position.Column is < 0 or >= 8)
            return null;
        
        return _squares[position.Row, position.Column].Piece;
    }

    public void MovePiece(Position from, Position to)
    {
        var piece = GetPiece(from);
        
        _squares[to.Row, to.Column].Piece = piece;
        _squares[from.Row, from.Column].Piece = null;
    }

    public void RemovePiece(Position position)
    {
        var piece = GetPiece(position);

        _squares[position.Row, position.Column].Piece = null;
    }

    public void ReplacePromotion(
        Position position, 
        PieceType? promotion,
        Color promotionColor)
    {
        if (promotion is null) return;
        
        RemovePiece(position);
        
        _squares[position.Row, position.Column].Piece = promotion switch
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
        var board = new Board( );

        for (var row = 0; row < 8; row++)
        {
            for (var column = 0; column < 8; column++)
            {
                var originalSquare = _squares[row, column];

                board._squares[row, column] = 
                    originalSquare with { Position = new Position(row, column) };
            }
        }

        return board;
    }
    
    public Position GetKingPosition(Color color)
    {
        var pos = new Position();
        foreach (var sq in _squares)
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
        return _squares;
    }

    public bool BishopsAreSameColor()
    {
        var bishopSquares = 
            (from Square square in _squares 
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

    public static Board CreateInitial()
    {
        var squares = new Square[8, 8];
        
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                var position = new Position(row, col);
                squares[row, col] = new Square
                {
                    Color = (row + col) % 2 == 0
                        ? Color.White
                        : Color.Black,
                    Position = position,
                    Piece = StartingPiece(position)
                };
            }
        }

        return new Board(squares);
    }
    
    private static Piece? StartingPiece(Position position)
    {
        Color? color = position.Row switch
        {
            0 or 1 => Color.Black,
            6 or 7 => Color.White,
            _ => null
        };
        
        if (color is null) return null;

        if (position.Row is 1 or 6)
        {
            return new Pawn(color.Value);
        }

        return position.Column switch
        {
            0 or 7 => new Rook(color.Value),
            1 or 6 => new Knight(color.Value),
            2 or 5 => new Bishop(color.Value),
            3 => new Queen(color.Value),
            4 => new King(color.Value),
            _ => null
        };
    }
}