using Chess.Cli.GameRunner;
using Spectre.Console;

namespace Chess.Cli.Board;

public class SpectreBoardRenderer : IBoardRenderer
{
    private readonly Table _table = new Table().Border(TableBorder.None).HideHeaders();

    public void Render(chess.Entities.Board.Board board)
    {
        _table.AddColumn(new TableColumn("").Centered().PadLeft(0).PadRight(0));
        for (var i = 0; i < 8; i++)
        {
            _table.AddColumn(new TableColumn("").Centered().PadLeft(0).PadRight(0));
        }

        var headerRow = new List<string> { "   " };
        foreach (var file in new[] { "A", "B", "C", "D", "E", "F", "G", "H" })
        {
            headerRow.Add($"[bold yellow] {file} [/]");
        }
        _table.AddRow(headerRow.ToArray());

        for (var i = 0; i < 8; i++)
        {
            var rankLabel = (8 - i).ToString();
            
            var rowCells = new List<string> { $"[bold yellow] {rankLabel} [/]" };

            for (var j = 0; j < 8; j++)
            {
                var square = board.Squares[i, j];

                var isLightSquare = (i + j) % 2 == 0;
                var bgColor = isLightSquare ? "#b89a74" : "#6b4f35";

                var letterId = square.Piece?.LetterId;
                var symbol = GetPieceSymbol(letterId);
                var fgColor = GetPieceTextColor(letterId);

                rowCells.Add($"[{fgColor} on {bgColor}] {symbol} [/]");
            }

            _table.AddRow(rowCells.ToArray());
        }

        AnsiConsole.Write(Align.Center(_table));
    }

    private static string GetPieceSymbol(char? letterId) => letterId switch
    {
        'p' => "♙",
        'r' => "♖",
        'n' => "♘",
        'b' => "♗",
        'q' => "♕",
        'k' => "♔",

        'P' => "♟",
        'R' => "♜",
        'N' => "♞",
        'B' => "♝",
        'Q' => "♛",
        'K' => "♚",

        _   => " "
    };

    private static string GetPieceTextColor(char? letterId)
    {
        if (!letterId.HasValue) return "white";
        return char.IsUpper(letterId.Value) ? "bold rgb(255,255,255)" : "bold rgb(0,0,0)";
    }
}