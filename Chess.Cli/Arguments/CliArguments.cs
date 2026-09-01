using System.CommandLine;
using Chess.Engine;
using chess.Entities.Common;

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


    public sealed record ParsedOptions(Color PlayerColor, ChessEngine Engine, int Elo);

    public ParsedOptions Parse(string[] args)
    {
        var root = new RootCommand("Chess")
        { 
            _playerColor,
            _chessEngine,
            _elo
        };
        var parseResult = root.Parse(args);
        
        return new ParsedOptions(
            parseResult.GetValue(_playerColor),
            parseResult.GetValue(_chessEngine),
            parseResult.GetValue(_elo));
    }
}
