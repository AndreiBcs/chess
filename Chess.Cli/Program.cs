using System.Text;
using Chess.Cli;
using Chess.Cli.Arguments;
using Chess.Cli.Game;
using Chess.Engine;

Console.OutputEncoding = Encoding.UTF8;

var options = new CliArguments().Parse(args);

if (options is null)
{
    // exit after "help" or "version" options
    return ExitCodes.Success; 
}

// start the game runner
GameRunner gameRunner;

try
{
    gameRunner = new GameRunner(options);
}
catch (Exception ex)
{
    // catch option parse errors
    Console.WriteLine(ex.Message);
    return ExitCodes.InvalidArguments;
}

try
{
    await gameRunner.Run();
}
catch (EngineException ex)
{
    Console.WriteLine(ex.Message);
    return ExitCodes.EngineError;
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return ExitCodes.RuntimeError;
}
finally
{
    await gameRunner.DisposeAsync();
}

return ExitCodes.Success;

