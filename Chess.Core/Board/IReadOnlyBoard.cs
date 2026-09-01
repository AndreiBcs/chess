using chess.Pieces;

namespace chess.Board;

public interface IReadOnlyBoard
{
    Piece? GetPiece(Position position);
    Board Copy();
    Position GetKingPosition(Color color);
    Square[,] GetSquares();
}