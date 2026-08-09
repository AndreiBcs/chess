using Chess.Cli.Board;
using Chess.Cli.GameRunner;
using Chess.Cli.Interactions;
using chess.Game;

var useText = args.Length > 0 && args[0] == "--text";

IBoardRenderer renderer = useText ? new TextBoardRenderer() : new SpectreBoardRenderer();
IUserInteraction userInteraction = useText ? new TextUserInteraction() : new SpectreUserInteraction();

var game = new Game();
game.StartGame();

var gameLoop = new CliGameLoop(renderer, userInteraction, game);
gameLoop.Run();

