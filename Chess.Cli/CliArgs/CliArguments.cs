using System.CommandLine;
using chess.Entities.Common;

namespace Chess.Cli.CliArgs;

public sealed class CliArguments
{
    private readonly Option<bool> _textRender = new(
        "--text-render",
        aliases: ["--text", "-t"])
    {
        Description = "Render the board as plain text"
    };
    
    private readonly Option<int> _elo = new(
        "--difficulty",
        aliases: ["-d"])
    {
        Description = "Choose the elo > 1320 & < 3190",
        DefaultValueFactory = _ => 1320
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
        int Elo, 
        Color PlayerColor);

    public ParsedOptions Parse(string[] args)
    {
        var root = new RootCommand("Chess CLI")
        {
            _textRender, 
            _elo, 
            _playerColor 
        };
        var parseResult = root.Parse(args);
        
        return new ParsedOptions(
            parseResult.GetValue(_textRender),
            parseResult.GetValue(_elo),
            parseResult.GetValue(_playerColor));
    }
}