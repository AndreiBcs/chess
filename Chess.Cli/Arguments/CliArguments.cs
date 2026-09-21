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
    
    private readonly Option<int> _depth = new(
        "--depth")
    {
        Description = "Choose how deep the engine should search",
        DefaultValueFactory = _ => 20
    };
    
    private readonly Option<int> _moveTime = new(
        "--move-time")
    {
        Description = "Choose how much time (ms) the engine should spend on a move",
        DefaultValueFactory = _ => 1500
    };
    
    private readonly Option<long> _nodes = new(
        "--nodes")
    {
        Description = "Choose how many nodes should the engine travers while searching",
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
                result.AddError("Stockfish elo ranges between 1320 and 3190.");
            }
        });
        
        _depth.Validators.Add(result =>
        {
            var depth = result.GetValue(_depth);

            if (depth is < 1 or > 50)
            {
                result.AddError("Depth must be between 1 and 50.");
            }
        });
        
        _moveTime.Validators.Add(result =>
        {
            var time = result.GetValue(_moveTime);

            if (time is < 1 or > 600_000)
            {
                result.AddError("Move time must be between 1 and 600000 ms.");
            }
        });
        
        _nodes.Validators.Add(result =>
        {
            var nodes = result.GetValue(_nodes);

            if (nodes is < 1 or > 1_000_000_000)
            {
                result.AddError("Nodes must be between 1 and 1000000000.");
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
            parseResult.GetValue(_depth),
            parseResult.GetValue(_moveTime),
            parseResult.GetValue(_nodes),
            parseResult.GetValue(_textRender));
    }
}