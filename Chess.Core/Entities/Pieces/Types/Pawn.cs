using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Entities.Pieces.Types;

public class Pawn : Piece
{
    public override PieceType Type => PieceType.Pawn;

    public override IEnumerable<Position> GetPossiblePositions(Board.Board board, Position from)
    {
        var legalMoves = new List<Position>();

        var direction = Owner.Color == Color.White ? -1 : 1;

        var oneForward = new Position(
            from.Row - direction,
            from.Column);

        if (oneForward.Row is >= 0 and < 8 &&
            oneForward.Column is >= 0 and < 8 &&
            board.Squares[oneForward.Row, oneForward.Column].Piece is null)
        {
            legalMoves.Add(oneForward);

            var twoForward = new Position(
                from.Row - 2 * direction,
                from.Column);

            if (!HasMoved &&
                twoForward.Row is >= 0 and < 8 &&
                board.Squares[twoForward.Row, twoForward.Column].Piece is null)
            {
                legalMoves.Add(twoForward);
            }
        }

        var leftCapture = new Position(
            from.Row - direction,
            from.Column - direction);

        if (leftCapture.Row is >= 0 and < 8 &&
            leftCapture.Column is >= 0 and < 8)
        {
            var piece = board.Squares[leftCapture.Row, leftCapture.Column].Piece;

            if (piece is not null && piece.Owner != Owner)
            {
                legalMoves.Add(leftCapture);
            }
        }

        var rightCapture = new Position(
            from.Row - direction,
            from.Column + direction);

        if (rightCapture.Row is >= 0 and < 8 &&
            rightCapture.Column is >= 0 and < 8)
        {
            var piece = board.Squares[rightCapture.Row, rightCapture.Column].Piece;

            if (piece is not null && piece.Owner != Owner)
            {
                legalMoves.Add(rightCapture);
            }
        }
        
        return legalMoves;
    }

    private bool HasMoved { get; set; } = false;
}