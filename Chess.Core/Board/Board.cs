using System.Collections.Immutable;
using chess.Pieces;
using chess.Pieces.Types;

namespace chess.Board;

public sealed record Board
{
    private readonly ImmutableArray<Square> _squares;
    private Board(ImmutableArray<Square> squares) => _squares = squares;
    private static int Index(Position p) => p.Row * 8 + p.Column;
    private static bool IsOnBoard(Position p) 
        => p.Row is >= 0 and < 8 && p.Column is >= 0 and < 8;
    private Board WithSquares(Func<ImmutableArray<Square>, ImmutableArray<Square>> transform)
        => new(transform(_squares));
    
    public IEnumerable<Square> Squares => _squares;

    public Piece? GetPiece(Position position)
    {
        return IsOnBoard(position) ? _squares[Index(position)].Piece : null;
    }

    public Board WithMove(Position from, Position to)
    {
        var piece = GetPiece(from);
        var moved = piece is not null ? piece with { HasMoved = true } : null;

        return WithSquares(squares => squares
            .SetItem(Index(to), squares[Index(to)] with { Piece = moved })
            .SetItem(Index(from), squares[Index(from)] with { Piece = null }));
    }

    public Board WithoutPiece(Position position) =>
        WithSquares(squares =>
            squares.SetItem(Index(position), squares[Index(position)] with { Piece = null }));

    public Board WithPromotion(Position position, PieceType type, Color color)
    {
        Piece promoted = type switch
        {
            PieceType.Queen => new Queen(color),
            PieceType.Rook => new Rook(color),
            PieceType.Bishop => new Bishop(color),
            PieceType.Knight => new Knight(color),
            _ => throw new ArgumentException("Invalid promotion piece", nameof(type))
        };

        return WithSquares(squares =>
            squares.SetItem(Index(position), squares[Index(position)] with { Piece = promoted }));
    }
    
    public bool BishopsAreSameColor()
    {
        var bishops = _squares.Where(sq =>
            sq.Piece?.Type == PieceType.Bishop).ToList();
        
        if (bishops.Count != 2) return false;

        var a = bishops[0].Position;
        var b = bishops[1].Position;
        return (a.Row + a.Column) % 2 == (b.Row + b.Column) % 2;
    }
    
    public Position GetKingPosition(Color color)
    {
        foreach (var square in _squares)
            if (square.Piece is King && square.Piece.Color == color)
                return square.Position;

        throw new InvalidOperationException($"No {color} king on the board");
    }
    
    public static Board CreateInitial()
    {
        var squares = ImmutableArray.CreateBuilder<Square>(64);
        
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                var position = new Position(row, col);
                squares.Add(new Square
                {
                    Color = (row + col) % 2 == 0 ? Color.White : Color.Black,
                    Position = position,
                    Piece = StartingPiece(position)
                });
            }
        }
        return new Board(squares.ToImmutableArray());
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
        
        if (position.Row is 1 or 6) return new Pawn(color.Value);

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