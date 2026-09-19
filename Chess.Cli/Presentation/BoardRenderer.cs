using chess.Board;
using chess.Game;
using Spectre.Console;

namespace Chess.Cli.Presentation;

public static class BoardRenderer
{
    public static void Render(
        GameSnapshot snapshot,
        chess.Color playerColor,
        bool textRender)
    {
        Console.Clear();
        
        var table = new Table().Border(TableBorder.None).HideHeaders();
        
        table.AddColumn(new TableColumn("").Centered().PadLeft(0).PadRight(0));
        for (var i = 0; i < 8; i++)
        {
            table.AddColumn(new TableColumn("").Centered().PadLeft(0).PadRight(0));
        }

        var files = playerColor == chess.Color.White
            ? new[] { "A", "B", "C", "D", "E", "F", "G", "H" }
            : new[] { "H", "G", "F", "E", "D", "C", "B", "A" };

        var headerRow = new List<string> { "   " };
        foreach (var file in files)
        {
            headerRow.Add($"[bold yellow] {file} [/]");
        }
        table.AddRow(headerRow.ToArray());

        for (var i = 0; i < 8; i++)
        {
            var boardRow = playerColor == chess.Color.White ? i : 7 - i;
            var rankLabel = (8 - boardRow).ToString();
            
            var rowCells = new List<string> { $"[bold yellow] {rankLabel} [/]" };

            for (var j = 0; j < 8; j++)
            {
                var boardColumn = playerColor == chess.Color.White ? j : 7 - j;
                var piece = snapshot.Board.GetPiece(new Position(boardRow, boardColumn));
                
                var bgColor = (boardRow + boardColumn) % 2 == 0 ? "#b89a74" : "#6b4f35";
                var letterId = piece?.LetterId;
                var symbol = textRender ? GetPieceText(letterId) : GetPieceSymbol(letterId);
                var fgColor = GetPieceTextColor(letterId);

                rowCells.Add($"[{fgColor} on {bgColor}] {symbol} [/]");
            }

            table.AddRow(rowCells.ToArray());
        }

        AnsiConsole.Write(Align.Center(table));

        var endMessage = snapshot.Status switch
        {
            GameStatus.WhiteWon => "White won by checkmate.",
            GameStatus.BlackWon => "Black won by checkmate.",
            GameStatus.DrawBy75MoveRule => "Draw by 75 more rule.",
            GameStatus.DrawByInsufficientMaterial => "Draw by insufficient material.",
            GameStatus.DrawByStalemate => "Draw by stalemate.",
            GameStatus.DrawByThreefoldRepetition => "Draw by threefold repetition.",
            _ => null
        };

        if (endMessage is not null)
        {
            AnsiConsole.MarkupLine($"[green]{endMessage}[/]");
            Console.WriteLine();
        }
    }

    private static string GetPieceText(char? letterId)
    {
        return !letterId.HasValue ? " " : letterId.Value.ToString();
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