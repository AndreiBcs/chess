using System.CommandLine;
using chess.Game;

namespace Chess.Cli.CliArgs;

public sealed class CliArguments
{
    private Option<bool> TextRender { get; } = new("--text-render")
    {
        Description = "Render the board as plain text"
    };
    
    private Option<Difficulty> Difficulty { get; } = new("--difficulty")
    {
        Description = "Choose the difficulty: Easy | Normal | Hard",
        DefaultValueFactory = _ => chess.Game.Difficulty.Normal
    };

    private Option<chess.Entities.Color> PlayerColor { get; } = new("--play-as")
    {
        Description = "Choose the side to play as: White or Black",
        DefaultValueFactory = _ => chess.Entities.Color.White
    };

    public sealed record ParsedOptions(
        bool UseTextRenderer,
        Difficulty Difficulty, 
        chess.Entities.Color PlayerColor);

    public RootCommand Build()
    {
        var root = new RootCommand("Chess CLI")
        {
            Options = { TextRender, Difficulty, PlayerColor }
        };
        return root;
    }

    public Task<int> InvokeAsync(string[] args, Func<ParsedOptions, int> run)
    {
        var root = Build();

        root.SetAction(parseResult =>
        {
            var options = new ParsedOptions(
                parseResult.GetValue(TextRender),
                parseResult.GetValue(Difficulty),
                parseResult.GetValue(PlayerColor));

            return run(options);
        });

        return root.Parse(args).InvokeAsync();
    }
}