using System.Text;
using Chess.Cli.Board;
using Chess.Cli.GameRunner;
using Chess.Cli.Interactions;

Console.OutputEncoding = Encoding.UTF8;


var useText = args.Length > 0 && args[0] == "--text";

IBoardRenderer renderer = !useText ? new TextBoardRenderer() : new SpectreBoardRenderer();
IUserInteraction userInteraction = useText ? new TextUserInteraction() : new SpectreUserInteraction();

var gameLoop = new CliGameLoop(renderer, userInteraction);
gameLoop.Run();

