using System.Text;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace Chess.Cli.Interactions;

public class SpectreUserInteraction : IUserInteraction
{
    private readonly IBoardRenderer _renderer;

    public SpectreUserInteraction(IBoardRenderer renderer)
    {
        _renderer = renderer;
    }

    public (string from, string to) ReadMove(chess.Entities.Board board)
    {
        while (true)
        {
            AnsiConsole.Clear();
            Console.OutputEncoding = Encoding.UTF8;
            _renderer.Render(board);
            Console.OutputEncoding = Encoding.UTF8;
            AnsiConsole.WriteLine();

            var fromSquare = AnsiConsole.Prompt(
                new TextPrompt<string>("[bold cyan]Select piece square (e.g., e2):[/]")
                    .PromptStyle("yellow")
                    .Validate(input =>
                    {
                        if (!Regex.IsMatch(input, "^[a-hA-H][1-8]$"))
                        {
                            return ValidationResult.Error(
                                "[red]Invalid notation. Enter a valid square like 'e2' or 'G7'.[/]");
                        }

                        var (row, col) = ParseNotation(input);
                        if (board.Squares[row, col].Piece == null)
                        {
                            return ValidationResult.Error("[red]There is no piece on that square. Try again.[/]");
                        }

                        return ValidationResult.Success();
                    }));

            var toSquare = AnsiConsole.Prompt(
                new TextPrompt<string>(
                        $"[bold cyan]Move piece from [yellow]{fromSquare.ToLower()}[/] to (e.g., e4):[/]")
                    .PromptStyle("yellow")
                    .Validate(input =>
                    {
                        if (!Regex.IsMatch(input, "^[a-hA-H][1-8]$"))
                        {
                            return ValidationResult.Error(
                                "[red]Invalid notation. Enter a valid square like 'e4' or 'C3'.[/]");
                        }

                        if (input.Equals(fromSquare, StringComparison.OrdinalIgnoreCase))
                        {
                            return ValidationResult.Error(
                                "[red]Target square must be different from source square.[/]");
                        }

                        return ValidationResult.Success();
                    }));

            var (fromRow, fromCol) = ParseNotation(fromSquare);
            var (toRow, toCol) = ParseNotation(toSquare);
        }
    }

    private static (int row, int col) ParseNotation(string notation)
    {
        var col = char.ToLower(notation[0]) - 'a'; // 'a'-'h' -> 0-7
        var row = 8 - (notation[1] - '0'); // '1'-'8' -> 7-0
        return (row, col);
    }
}