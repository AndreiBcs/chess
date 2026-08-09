using Chess.Cli;
using Chess.Cli.Board;
using Chess.Cli.Interactions;
using chess.Game;

var useText = args.Length > 0 && args[0] == "--text";

IUserInteraction renderer = useText
    ? new TextUserInteraction(new TextBoardRenderer())
    : new SpectreUserInteraction(new SpectreBoardRenderer());

var game = new Game();
game.StartGame();

