using System.Text.RegularExpressions;
using chess.Board;
using Chess.Cli.Arguments;
using Chess.Engine;
using chess.Game;
using chess.Moves;
using chess.Pieces;
using Spectre.Console;
using Color = chess.Color;

namespace Chess.Cli.Presentation;

public static partial class ConsoleInteraction 
{
    public static Task<Move> ReadMove(GameSnapshot snapshot)
    {
        try
        {
            while (true)
            {
                AnsiConsole.WriteLine();

                var fromSquare = AnsiConsole.Prompt(
                    new TextPrompt<string>("[bold blue]Select piece square:[/]")
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
                            $"[bold blue]Move piece from [yellow]{fromSquare.ToLower()}[/] to:[/]")
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

                PieceType? promotion = null;
                var piece = snapshot.Board.GetPiece(new Position(fromRow, fromCol));
                if (piece?.Type == PieceType.Pawn && 
                    piece.Color == snapshot.CurrentTurn && 
                    (piece.Color == Color.White && toRow is 0 ||
                     piece.Color == Color.Black && toRow is 7))
                {
                    promotion = AnsiConsole.Prompt(
                        new SelectionPrompt<PieceType>()
                            .Title("Choose a promotion piece:")
                            .AddChoices(
                                PieceType.Knight,
                                PieceType.Bishop,
                                PieceType.Queen,
                                PieceType.Rook));
                }
            
                return Task.FromResult(new Move
                {
                    From = new Position(fromRow, fromCol),
                    To = new Position(toRow, toCol),
                    Promotion = promotion
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

    public static void ShowMoveError(string warningMessage)
    {
        AnsiConsole.MarkupLine($"[red]{warningMessage}[/]");
    }

    public static Options GetGameOptions()
    {
        var playerColor = AnsiConsole.Prompt(
            new SelectionPrompt<Color>()
                .Title("Choose a color:")
                .AddChoices(Color.White, Color.Black));
        
        var engineType = AnsiConsole.Prompt(
            new SelectionPrompt<ChessEngine>()
                .Title("Choose a chess engine:")
                .AddChoices(ChessEngine.Stockfish, ChessEngine.Deakfish));

        var elo = AnsiConsole.Prompt(
            new TextPrompt<int>("Chess engine Elo:")
                .Validate(value =>
                    value is >= 1320 and <= 3190
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Elo must be between 1320 and 3190.[/]"))
        );

        var pieceRender = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
                .Title("Render pieces as text?")
                .AddChoices(true, false)
                .UseConverter(value => value ? "Yes" : "No")
        );

        return new Options(playerColor, engineType, elo, pieceRender);
    }

    public static bool SaveGameToFile()
    {
        var save = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
                .Title("Save game to file?")
                .AddChoices(true, false)
                .UseConverter(value => value ? "Yes" : "No")
        );
        
        return save;
    }
}