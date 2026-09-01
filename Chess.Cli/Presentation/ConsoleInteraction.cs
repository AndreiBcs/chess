using System.Text.RegularExpressions;
using chess.Board;
using chess.Game;
using chess.Moves;
using Spectre.Console;

namespace Chess.Cli.Presentation;

public partial class ConsoleInteraction 
{
    public Task<Move> ReadMove(GameSnapshot snapshot)
    {
        try
        {
            while (true)
            {
                AnsiConsole.WriteLine();

                var fromSquare = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold blue]Select piece square (e.g., e2):[/]")
                        .PromptStyle("yellow")
                        .Validate(input =>
                        {
                            if (!MyRegex().IsMatch(input))
                            {
                                return ValidationResult.Error(
                                    "[red]Invalid notation. Enter a valid square like 'e2' or 'G7'.[/]");
                            }
                            return ValidationResult.Success();
                        }));

                var toSquare = AnsiConsole.Prompt(
                    new TextPrompt<string>(
                            $"[bold blue]Move piece from [yellow]{fromSquare.ToLower()}[/] to (e.g., e4):[/]")
                        .PromptStyle("yellow")
                        .Validate(input =>
                        {
                            if (!MyRegex().IsMatch(input))
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
            
                return Task.FromResult(new Move
                {
                    From = new Position(fromRow, fromCol),
                    To = new Position(toRow, toCol)
                });
            }
        }
        catch (Exception exception)
        {
            return Task.FromException<Move>(exception);
        }
    }

    private static (int row, int col) ParseNotation(string notation)
    {
        var col = char.ToLower(notation[0]) - 'a'; // 'a'-'h' -> 0-7
        var row = 8 - (notation[1] - '0'); // '1'-'8' -> 7-0
        return (row, col);
    }

    [GeneratedRegex("^[a-hA-H][1-8]$")]
    private static partial Regex MyRegex();

    public void ShowMoveError(MoveResult? previousResult)
    {
        throw new NotImplementedException();
    }
}