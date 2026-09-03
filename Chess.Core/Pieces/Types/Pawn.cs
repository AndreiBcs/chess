using chess.Board;

namespace chess.Pieces.Types;

public sealed record Pawn(Color Color) : Piece(Color)
{
    public override PieceType Type => PieceType.Pawn;

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

    public override IEnumerable<Position> GetAttackPositions(IReadOnlyBoard board, Position from)
    {
        var attackPositions = new List<Position>();
        
        var direction = Color == Color.White ? -1 : 1;
        
        var leftCapture = new Position(
            from.Row + direction,
            from.Column - 1);

        if (leftCapture.Row is >= 0 and < 8 &&
            leftCapture.Column is >= 0 and < 8)
        {
            attackPositions.Add(leftCapture);
        }

        var rightCapture = new Position(
            from.Row + direction,
            from.Column + 1);

        if (rightCapture.Row is >= 0 and < 8 &&
            rightCapture.Column is >= 0 and < 8)
        {
            attackPositions.Add(rightCapture);
        }

        return attackPositions;
    }

    public override (int Row, int Column)[] GetMoveDirections()
    {
        var direction = Color == Color.White ? -1 : 1;
        
        if (!HasMoved)
        {
            return 
            [
                (direction, 0),
                (direction * 2, 0)
            ];
        }

        return [(direction, 0)];
    }

    public override (int Rox, int Column)[] GetAttackDirections()
    {
        var direction = Color == Color.White ? -1 : 1;
        
        return 
        [
            (direction, 1),
            (direction, -1)
        ];
    }
}