namespace Chess.Cli.Services;

public static class GameSaver
{
    public static readonly string GamesDirectory = 
        Path.Combine(AppContext.BaseDirectory, "Games");

    public static async Task SaveGameAsync(string pgn)
    {
        var fileName = $"{DateTime.Now:g}.pgn";
        var filePath = Path.Combine(GamesDirectory, fileName);
        
        await File.WriteAllTextAsync(filePath, pgn);
    }
}