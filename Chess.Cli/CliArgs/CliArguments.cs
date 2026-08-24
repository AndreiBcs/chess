using System.CommandLine;
using chess.Entities.Common;
using chess.Entities.Player.ChessEngine;
using chess.Game;

namespace Chess.Cli.CliArgs;

public sealed class CliArguments
{
    private readonly Option<bool> _textRender = new(
        "--text-render",
        aliases: ["--text", "-t"])
    {
        Description = "Render the board as plain text"
    };
    
    private readonly Option<Difficulty> _difficulty = new(
        "--difficulty",
        aliases: ["-d"])
    {
        Description = "Choose the difficulty: Easy | Normal | Hard",
        DefaultValueFactory = _ => Difficulty.Normal
    };

    private readonly Option<Color> _playerColor = new(
        "--play-as",
        aliases: ["--color", "-c"])
    {
        Description = "Choose the side to play as: White or Black",
        DefaultValueFactory = _ => Color.White
    };

    public sealed record ParsedOptions(
        bool UseTextRenderer,
        Difficulty Difficulty, 
        Color PlayerColor);

    public ParsedOptions Parse(string[] args)
    {
        var root = new RootCommand("Chess CLI")
        {
            _textRender, 
            _difficulty, 
            _playerColor 
        };
        var parseResult = root.Parse(args);
        
        return new ParsedOptions(
            parseResult.GetValue(_textRender),
            parseResult.GetValue(_difficulty),
            parseResult.GetValue(_playerColor));
    }
}