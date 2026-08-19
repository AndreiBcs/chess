using System.Diagnostics;

namespace Chess.Engine;

public class Engine
{
    private readonly Process _process;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    
    public Engine()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Executable", "stockfish.exe");

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

    public string GetBestMove(string fen, int depth)
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
    
    public void QuitUci()
    {
        if (_process.HasExited) return;
        
        SendCommand("quit");
        _process.WaitForExit();
    }
    
    private void SendCommand(string command)
    {
        _writer.WriteLine(command);
        _writer.Flush();
    }
}
