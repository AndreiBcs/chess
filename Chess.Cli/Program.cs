using System.Text;
using Chess.Cli.CliArgs;
using Chess.Cli.GameRunner;

Console.OutputEncoding = Encoding.UTF8;


var options = new CliArguments().Parse(args);

var gameRunner = new GameRunner(options);
await gameRunner.Run();

