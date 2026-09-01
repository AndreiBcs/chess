using chess;
using chess.Board;
using chess.Pieces;

namespace Chess.Tests.Core.Board;

public static class BoardTestExtensions
{
    public static chess.Board.Board CreateEmpty(
        this chess.Board.Board board)
    {
        var squares = board.GetSquares();
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                squares[row, col] = new Square
                {
                    Color = (row + col) % 2 == 0
                        ? Color.White
                        : Color.Black,
                    Position = new Position(row, col)
                };
            }
        }
        return board;
    }
    
    public static chess.Board.Board PlacePiece(
        this chess.Board.Board board,
        Piece piece,
        Position position)
    {
        var squares = board.GetSquares();
        squares[position.Row, position.Column].Piece = piece;
        return board;
    }
}