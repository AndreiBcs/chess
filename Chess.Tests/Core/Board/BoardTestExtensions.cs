using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;

namespace Chess.Tests.Core.Board;

public static class BoardTestExtensions
{
    public static chess.Entities.Board.Board CreateEmpty(
        this chess.Entities.Board.Board board)
    {
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                board.Squares[row, col] = new Square
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
    
    public static chess.Entities.Board.Board PlacePiece(
        this chess.Entities.Board.Board board,
        Piece piece,
        Position position)
    {
        board.Squares[position.Row, position.Column].Piece = piece;
        return board;
    }
}