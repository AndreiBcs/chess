using chess.Entities.Board;
using chess.Entities.Common;

namespace chess.Entities.Pieces.Types;

public class Pawn : Piece
{
    public Pawn(Color color) : base(color)
    {
    }

    public override PieceType Type => PieceType.Pawn;
    public bool HasMoved { get; set; } = false;

    public override IEnumerable<Position> GetPossiblePositions(IReadOnlyBoard board, Position from)
    {
        var possiblePositions = new List<Position>();

        var direction = Color == Color.White ? -1 : 1;

        var oneForward = new Position(
            from.Row + direction,
            from.Column);

        if (oneForward.Row is >= 0 and < 8 &&
            oneForward.Column is >= 0 and < 8 &&
            board.GetPiece(oneForward) is null)
        {
            possiblePositions.Add(oneForward);

            var twoForward = new Position(
                from.Row + 2 * direction,
                from.Column);

            if (!HasMoved &&
                twoForward.Row is >= 0 and < 8 &&
                board.GetPiece(twoForward) is null)
            {
                possiblePositions.Add(twoForward);
            }
        }
        

        var leftCapture = new Position(
            from.Row + direction,
            from.Column - 1);

        if (leftCapture.Row is >= 0 and < 8 &&
            leftCapture.Column is >= 0 and < 8)
        {
            var piece = board.GetPiece(leftCapture);

            if (piece is not null && piece.Color != Color)
            {
                possiblePositions.Add(leftCapture);
            }
        }
        

        var rightCapture = new Position(
            from.Row + direction,
            from.Column + 1);

        if (rightCapture.Row is >= 0 and < 8 &&
            rightCapture.Column is >= 0 and < 8)
        {
            var piece = board.GetPiece(rightCapture);

            if (piece is not null && piece.Color != Color)
            {
                possiblePositions.Add(rightCapture);
            }
        }


        var rowForEnPassant = direction > 0 ? 4 : 3;

        if (from.Row == rowForEnPassant)
        {
            if (from.Column - 1 >= 0)
            {
                var pos = new Position(from.Row, from.Column - 1);
                var piece = board.GetPiece(pos);

                if (piece is not null && piece.Color != Color && piece.Type is PieceType.Pawn)
                {
                    possiblePositions.Add(pos);
                }
            }

            if (from.Column + 1 < 8)
            {
                var pos = new Position(from.Row, from.Column + 1);
                var piece = board.GetPiece(pos);

                if (piece is not null && piece.Color != Color && piece.Type is PieceType.Pawn)
                {
                    possiblePositions.Add(pos);
                }
            }
        }
        
        return possiblePositions;
    }
}