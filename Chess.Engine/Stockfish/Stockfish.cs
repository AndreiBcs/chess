using System.Diagnostics;

namespace Chess.Engine.Stockfish;

public class Stockfish : Uci
{
    private readonly Process _process;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    public Stockfish()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Stockfish", "stockfish-windows-x86-64-avx2.exe");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("stockfish not found", path);
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
    }

    public void StartEngine()
    {
        SendCommand("uci");
        WaitForResponse("uciok");
        SendCommand("isready");
        WaitForResponse("readyok");
    }

    public void NewGame()
    {
        SendCommand("ucinewgame");
        SendCommand("isready");
        WaitForResponse("readyok");
    }

    public void SetOption(string name, string value)
    {
        SendCommand($"setoption name {name} value {value}");
    }

    public string GetMove(string fen)
    {
        SendCommand($"position fen {fen}");
        // spend at most 1000 ms to calculate the move
        SendCommand("go movetime 1000");
        
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

    public void StopEngine()
    {
        if (!_process.HasExited)
        {
            SendCommand("quit");
            _process.WaitForExit();
        }
        _reader.Dispose();
        _writer.Dispose();
        _process.Dispose();
    }

    public void SetElo(int elo)
    {
        elo = Math.Clamp(elo, 1320, 3190);
        SetOption("UCI_LimitStrength", "true");
        SetOption("UCI_Elo", elo.ToString());
        SendCommand("isready");
        WaitForResponse("readyok");
    }
    
    
    private void SendCommand(string command)
    {
        _writer.WriteLine(command);
        _writer.Flush();
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
}