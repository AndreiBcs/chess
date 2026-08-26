using System.Text;
using Chess.Cli.Board;
using Chess.Cli.CliArgs;
using Chess.Cli.GameRunner;
using Chess.Cli.Interactions;

Console.OutputEncoding = Encoding.UTF8;


var options = new CliArguments().Parse(args);

IBoardRenderer renderer = options.UseTextRenderer 
    ? new TextBoardRenderer() 
    : new SpectreBoardRenderer();

IUserInteraction userInteraction = options.UseTextRenderer 
    ? new TextUserInteraction() 
    : new SpectreUserInteraction();

var gameRunner = new GameRunner(renderer, userInteraction, options.PlayerColor);
gameRunner.Run();

