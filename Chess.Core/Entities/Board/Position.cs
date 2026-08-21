using chess.Entities.Pieces;

namespace chess.Entities.Board;

public readonly record struct Position(int Row, int Column)
{
    public override string ToString()
    {
        return $"{(char)('a' + Column)}{8 - Row}";
    }
}

public readonly record struct Move(Position From, Position To, Piece? Piece = null);
