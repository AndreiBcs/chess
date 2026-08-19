using System.Diagnostics;

namespace Chess.Engine;

public class Engine : IDisposable
{
    private readonly Process _process;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private bool _disposed;
    
    public Engine()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Executable", "stockfish-windows-x86-64-avx2.exe");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("stockfish.exe not found", path);
        }

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            }
        };
        Console.WriteLine($"Engine path: {path}");
        Console.WriteLine($"Exists: {File.Exists(path)}");
        var fileInfo = new FileInfo(path);
        Console.WriteLine($"Size: {fileInfo.Length} bytes");
        _process.Start();
        _reader = _process.StandardOutput;
        _writer = _process.StandardInput;
        
        StartUci();
    }

    private void StartUci()
    {
        SendCommand("uci");
        WaitForResponse("uciok");
        
        SendCommand("isready");
        WaitForResponse("readyok");
    }

    private void WaitForResponse(string expectedResponse)
    {
        string? line;
        while ((line = _reader.ReadLine()) is not null)
        {
            if (string.Equals(line, expectedResponse, 
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }
        
        throw new InvalidOperationException(
            $"Stockfish exited before sending '{expectedResponse}'"
        );
    }

    public void NewGame()
    {
        SendCommand("ucinewgame");
        SendCommand("isready");
        WaitForResponse("readyok");
    }
    
    private void SetOption(string name, string value)
    {
        SendCommand($"setoption name {name} value {value}");
    }

    public void SetElo(int elo)
    {
        SetOption("UCI_LimitStrength", "true");
        SetOption("UCI_Elo", elo.ToString());

        SendCommand("isready");
        WaitForResponse("readyok");
    }
    
    public string GetBestMove(string fen, int depth = 15)
    {
        SendCommand($"position fen {fen}");
        SendCommand($"go depth {depth}");
        
        string? line;
        while ((line = _reader.ReadLine()) is not null)
        {
            if (line.StartsWith("bestmove"))
            {
                var parts = line.Split(' ');
                return  parts[1];
            }
        }

        throw new InvalidOperationException(
            "Engine did not return a best move"
        );
    }
    
    private void SendCommand(string command)
    {
        _writer.WriteLine(command);
        _writer.Flush();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        
        if (!_process.HasExited)
        {
            SendCommand("quit");
            _process.WaitForExit();
        }
        _reader.Dispose();
        _writer.Dispose();
        _process.Dispose();
    }
}
