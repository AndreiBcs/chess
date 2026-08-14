namespace Chess.Engine.Services;

public class Stockfish
{
    public Stockfish()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Executable", "stockfish.exe");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("stockfish.exe not found", path);
        }

    }
}
