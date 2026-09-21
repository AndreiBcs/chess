using System.Text;
using Chess.Cli;
using Chess.Cli.Arguments;
using Chess.Cli.Game;
using Chess.Cli.Presentation;
using Chess.Engine;

Console.OutputEncoding = Encoding.UTF8;

Options? options;

if (args.Length == 0)
{
    options = ConsoleInteraction.GetGameOptions();
}
else
{
    try
    {
        options = new CliArguments().Parse(args);
    }
    catch (ArgumentException ex)
    {
        // catch option parse errors
        Console.WriteLine(ex.Message);
        return ExitCodes.InvalidArguments;
    }
    
    if (options is null)
    {
        return ExitCodes.Success;
    }
}

try
{
    if (options.Engine is ChessEngine.Deakfish)
    {
        throw new EngineException("Deakfish is currently in development.");
    }
}
catch (EngineException ex)
{
    Console.WriteLine(ex.Message);
    return ExitCodes.EngineError;
}


// start the game runner
var gameRunner = new GameRunner(options);

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

