using System.CommandLine;
using System.Reflection;
using chess;
using Chess.Engine;

namespace Chess.Cli.Arguments;

public sealed class CliArguments
{
    private readonly Option<Color> _playerColor = new(
        "--color", aliases: ["-c"])
    {
        Description = "Choose the color to play as",
        DefaultValueFactory = _ => Color.White
    };

    private readonly Option<ChessEngine> _chessEngine = new(
        "--engine")
    {
        Description = "Choose the engine to play against",
        DefaultValueFactory = _ => ChessEngine.Stockfish
    };
    
    private readonly Option<int> _elo = new(
        "--elo")
    {
        Description = "Choose the elo of the engine",
        DefaultValueFactory = _ => 1400
    };

    private readonly Option<bool> _textRender = new(
        "--text")
    {
        Description = "Render the chess pieces as text",
        DefaultValueFactory = _ => false
    };

    public CliArguments()
    {
        _elo.Validators.Add(result =>
        {
            var elo = result.GetValue(_elo);

            if (elo is < 1320 or > 3190)
            {
                result.AddError("Stockfish Elo must be between 1320 and 3190.");
            }
        });
    }
    
    public Options? Parse(string[] args)
    {
        var root = new RootCommand("Chess")
        { 
            _playerColor,
            _chessEngine,
            _elo,
            _textRender
        };
        var parseResult = root.Parse(args);

        if (args.Contains("--help") || args.Contains("-h"))
        {
            parseResult.Invoke();
            return null;
        }

        if (args.Contains("--version") || args.Contains("-v"))
        {
            var version = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
            
            Console.WriteLine($"{version}");
            return null;
        }

        if (parseResult.Errors.Count > 0)
        {
            foreach (var error in parseResult.Errors)
                throw new ArgumentException(error.ToString());
        }
        
        return new Options(
            parseResult.GetValue(_playerColor),
            parseResult.GetValue(_chessEngine),
            parseResult.GetValue(_elo),
            parseResult.GetValue(_textRender));
    }
}