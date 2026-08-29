namespace chess.Entities.Board;

public readonly record struct Position(int Row, int Column)
{
    public override string ToString()
    {
        return $"{(char)('a' + Column)}{8 - Row}";
    }

    public static Position ParsePosition(string position)
    {
         var col = position[0] - 'a';
         var row = 8 - int.Parse(position[1].ToString());
         
         return new Position(row, col);
    }
}
