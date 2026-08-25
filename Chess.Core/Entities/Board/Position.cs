namespace chess.Entities.Board;

public readonly record struct Position(int Row, int Column)
{
    public override string ToString()
    {
        return $"{(char)('a' + Column)}{8 - Row}";
    }

    public static Position ParsePosition(string fen)
    {
         var col = fen[0] - 'a';
         var row = 8 - fen[1];
         
         return new Position(col, row);
    }
}
