namespace Chess.Cli.Services;

public static class GameSaver
{
    public static readonly string GamesDirectory = 
        Path.Combine(AppContext.BaseDirectory, "Games");

    public static async Task SaveGameAsync(string pgn)
    {
        var fileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pgn";
        var filePath = Path.Combine(GamesDirectory, fileName);

        if (Directory.Exists(GamesDirectory))
        {
            await File.WriteAllTextAsync(filePath, pgn);
        }
        else
        {
            Directory.CreateDirectory(GamesDirectory);
            
            await File.WriteAllTextAsync(filePath, pgn);
        }
    }
}