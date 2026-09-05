using System.Text;
using Chess.Cli.Arguments;
using Chess.Cli.Game;

Console.OutputEncoding = Encoding.UTF8;

var options = new CliArguments().Parse(args);


GameRunner? gameRunner = null;

AppDomain.CurrentDomain.ProcessExit += (_, _) => 
    gameRunner?.DisposeAsync().AsTask().Wait();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    gameRunner?.DisposeAsync().AsTask().Wait();
    Environment.Exit(0);
};

try
{
    gameRunner = new GameRunner(options);
    await gameRunner.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    gameRunner?.DisposeAsync().AsTask().Wait();
}

