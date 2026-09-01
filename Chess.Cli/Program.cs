using System.Text;
using Chess.Cli.Arguments;
using Chess.Cli.Game;

Console.OutputEncoding = Encoding.UTF8;


var options = new CliArguments().Parse(args);

var gameRunner = new GameRunner(options);
await gameRunner.Run();

