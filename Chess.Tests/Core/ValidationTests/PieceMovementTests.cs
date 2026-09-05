using chess;
using chess.Board;
using chess.Pieces.Types;
using chess.Validation.MoveValidation;

namespace Chess.Tests.Core.ValidationTests;

public class PieceMovementTests
{
    private static readonly Dictionary<Position, List<Position>> BishopPositions = new()
    {
        // center
        [new Position(4, 4)] =
        [
            new(3, 3), new(2, 2), new(1, 1), new(0, 0),
            new(3, 5), new(2, 6), new(1, 7),
            new(5, 3), new(6, 2), new(7, 1),
            new(5, 5), new(6, 6), new(7, 7)
        ],

        // near right edge
        [new Position(4, 7)] =
        [
            new(3, 6), new(2, 5), new(1, 4), new(0, 3),
            new(5, 6), new(6, 5), new(7, 4)
        ],

        // corner
        [new Position(0, 0)] =
        [
            new(1, 1), new(2, 2), new(3, 3),
            new(4, 4), new(5, 5), new(6, 6), new(7, 7)
        ],

        // opposite corner
        [new Position(7, 7)] =
        [
            new(6, 6), new(5, 5), new(4, 4),
            new(3, 3), new(2, 2), new(1, 1), new(0, 0)
        ]
    };

    private static readonly Dictionary<Position, List<Position>> RookPositions = new()
    {
        // center
        [new Position(4, 4)] =
        [
            new(3, 4), new(2, 4), new(1, 4), new(0, 4),
            new(5, 4), new(6, 4), new(7, 4),
            new(4, 3), new(4, 2), new(4, 1), new(4, 0),
            new(4, 5), new(4, 6), new(4, 7)
        ],

        // corner
        [new Position(0, 0)] =
        [
            new(1, 0), new(2, 0), new(3, 0), new(4, 0),
            new(5, 0), new(6, 0), new(7, 0),
            new(0, 1), new(0, 2), new(0, 3), new(0, 4),
            new(0, 5), new(0, 6), new(0, 7)
        ],

        // opposite corner
        [new Position(7, 7)] =
        [
            new(6, 7), new(5, 7), new(4, 7), new(3, 7),
            new(2, 7), new(1, 7), new(0, 7),
            new(7, 6), new(7, 5), new(7, 4), new(7, 3),
            new(7, 2), new(7, 1), new(7, 0)
        ],

        // edge
        [new Position(0, 4)] =
        [
            new(1, 4), new(2, 4), new(3, 4), new(4, 4),
            new(5, 4), new(6, 4), new(7, 4),
            new(0, 3), new(0, 2), new(0, 1), new(0, 0),
            new(0, 5), new(0, 6), new(0, 7)
        ]
    };

    private static readonly Dictionary<Position, List<Position>> QueenPositions = new()
    {
        // center
        [new Position(4, 4)] =
        [
            // rook
            new(3, 4), new(2, 4), new(1, 4), new(0, 4),
            new(5, 4), new(6, 4), new(7, 4),
            new(4, 3), new(4, 2), new(4, 1), new(4, 0),
            new(4, 5), new(4, 6), new(4, 7),

            // bishop
            new(3, 3), new(2, 2), new(1, 1), new(0, 0),
            new(3, 5), new(2, 6), new(1, 7),
            new(5, 3), new(6, 2), new(7, 1),
            new(5, 5), new(6, 6), new(7, 7)
        ],

        // corner
        [new Position(0, 0)] =
        [
            // rook
            new(1, 0), new(2, 0), new(3, 0), new(4, 0),
            new(5, 0), new(6, 0), new(7, 0),
            new(0, 1), new(0, 2), new(0, 3), new(0, 4),
            new(0, 5), new(0, 6), new(0, 7),

            // bishop
            new(1, 1), new(2, 2), new(3, 3),
            new(4, 4), new(5, 5), new(6, 6), new(7, 7)
        ],

        // opposite corner
        [new Position(7, 7)] =
        [
            // rook
            new(6, 7), new(5, 7), new(4, 7), new(3, 7),
            new(2, 7), new(1, 7), new(0, 7),
            new(7, 6), new(7, 5), new(7, 4), new(7, 3),
            new(7, 2), new(7, 1), new(7, 0),

            // bishop
            new(6, 6), new(5, 5), new(4, 4),
            new(3, 3), new(2, 2), new(1, 1), new(0, 0)
        ]
    };

    private static readonly Dictionary<Position, List<Position>> KnightPositions = new()
    {
        // center
        [new Position(4, 4)] =
        [
            new(2, 3), new(2, 5),
            new(3, 2), new(3, 6),
            new(5, 2), new(5, 6),
            new(6, 3), new(6, 5)
        ],

        // near edge
        [new Position(1, 1)] =
        [
            new(0, 3),
            new(2, 3),
            new(3, 0),
            new(3, 2)
        ],

        // corner
        [new Position(0, 0)] =
        [
            new(1, 2),
            new(2, 1)
        ],

        // opposite corner
        [new Position(7, 7)] =
        [
            new(5, 6),
            new(6, 5)
        ]
    };

    private static readonly Dictionary<Position, List<Position>> KingPositions = new()
    {
        // center
        [new Position(4, 4)] =
        [
            new(3, 3), new(3, 4), new(3, 5),
            new(4, 3),             new(4, 5),
            new(5, 3), new(5, 4), new(5, 5)
        ],

        // edge
        [new Position(0, 4)] =
        [
            new(0, 3), new(0, 5),
            new(1, 3), new(1, 4), new(1, 5)
        ],

        // corner
        [new Position(0, 0)] =
        [
            new(0, 1),
            new(1, 0),
            new(1, 1)
        ],

        // opposite corner
        [new Position(7, 7)] =
        [
            new(6, 6),
            new(6, 7),
            new(7, 6)
        ]
    };

    private static readonly Dictionary<Position, List<Position>> WhitePawnPositions = new()
    {
        // starting rank
        [new Position(6, 4)] =
        [
            new(5, 4),
            new(4, 4)
        ],

        // normal position
        [new Position(4, 4)] =
        [
            new(3, 4)
        ],

        // one square from promotion
        [new Position(1, 4)] =
        [
            new(0, 4)
        ]
    };

    private static readonly Dictionary<Position, List<Position>> BlackPawnPositions = new()
    {
        // starting rank
        [new Position(1, 4)] =
        [
            new(2, 4),
            new(3, 4)
        ],

        // normal position
        [new Position(4, 4)] =
        [
            new(5, 4)
        ],

        // one square from promotion
        [new Position(6, 4)] =
        [
            new(7, 4)
        ]
    };


    [Theory]
    [InlineData(4, 4)]
    [InlineData(4, 7)]
    [InlineData(0, 0)]
    [InlineData(7, 7)]
    public void BishopMovementTest(int fromRow, int fromCol)
    {
        var from = new Position(fromRow, fromCol);

        var bishop = new Bishop(Color.White);

        var board = Board.CreateEmptyBoard()
            .WithPiece(bishop, from);

        var piecePossiblePosition = bishop.GetPiecePositions(board);
        var expectedPositions = BishopPositions[from];

        Assert.Equivalent(expectedPositions, piecePossiblePosition);
    }


    [Theory]
    [InlineData(4, 4)]
    [InlineData(0, 0)]
    [InlineData(7, 7)]
    [InlineData(0, 4)]
    public void RookMovementTest(int fromRow, int fromCol)
    {
        var from = new Position(fromRow, fromCol);

        var rook = new Rook(Color.White);

        var board = Board.CreateEmptyBoard()
            .WithPiece(rook, from);

        var piecePossiblePosition = rook.GetPiecePositions(board);
        var expectedPositions = RookPositions[from];

        Assert.Equivalent(expectedPositions, piecePossiblePosition);
    }


    [Theory]
    [InlineData(4, 4)]
    [InlineData(0, 0)]
    [InlineData(7, 7)]
    public void QueenMovementTest(int fromRow, int fromCol)
    {
        var from = new Position(fromRow, fromCol);

        var queen = new Queen(Color.White);

        var board = Board.CreateEmptyBoard()
            .WithPiece(queen, from);

        var piecePossiblePosition = queen.GetPiecePositions(board);
        var expectedPositions = QueenPositions[from];

        Assert.Equivalent(expectedPositions, piecePossiblePosition);
    }


    [Theory]
    [InlineData(4, 4)]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    [InlineData(7, 7)]
    public void KnightMovementTest(int fromRow, int fromCol)
    {
        var from = new Position(fromRow, fromCol);

        var knight = new Knight(Color.White);

        var board = Board.CreateEmptyBoard()
            .WithPiece(knight, from);

        var piecePossiblePosition = knight.GetPiecePositions(board);
        var expectedPositions = KnightPositions[from];

        Assert.Equivalent(expectedPositions, piecePossiblePosition);
    }


    [Theory]
    [InlineData(4, 4)]
    [InlineData(0, 4)]
    [InlineData(0, 0)]
    [InlineData(7, 7)]
    public void KingMovementTest(int fromRow, int fromCol)
    {
        var from = new Position(fromRow, fromCol);

        var king = new King(Color.White);

        var board = Board.CreateEmptyBoard()
            .WithPiece(king, from);

        var piecePossiblePosition = king.GetPiecePositions(board);
        var expectedPositions = KingPositions[from];

        Assert.Equivalent(expectedPositions, piecePossiblePosition);
    }


    [Theory]
    [InlineData(6, 4)]
    [InlineData(4, 4)]
    [InlineData(1, 4)]
    public void WhitePawnMovementTest(int fromRow, int fromCol)
    {
        var from = new Position(fromRow, fromCol);

        var pawn = new Pawn(Color.White);

        var board = Board.CreateEmptyBoard()
            .WithPiece(pawn, from);

        var piecePossiblePosition = pawn.GetPiecePositions(board);
        var expectedPositions = WhitePawnPositions[from];

        Assert.Equivalent(expectedPositions, piecePossiblePosition);
    }


    [Theory]
    [InlineData(1, 4)]
    [InlineData(4, 4)]
    [InlineData(6, 4)]
    public void BlackPawnMovementTest(int fromRow, int fromCol)
    {
        var from = new Position(fromRow, fromCol);

        var pawn = new Pawn(Color.Black);

        var board = Board.CreateEmptyBoard()
            .WithPiece(pawn, from);

        var piecePossiblePosition = pawn.GetPiecePositions(board);
        var expectedPositions = BlackPawnPositions[from];

        Assert.Equivalent(expectedPositions, piecePossiblePosition);
    }
}