using System.Diagnostics;

namespace Chess.Engine;

public class Uci
{
    private readonly Process _process;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    public Uci(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("engine not found", path);
        }

        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                RedirectStandardInput = true,
                CreateNoWindow = true
            }
        };
        
        _process.Start();
        _reader = _process.StandardOutput;
        _writer = _process.StandardInput;
    }

    public async Task StartEngine()
    {
        await SendCommand("uci");
        await WaitForResponse("uciok");
        await SendCommand("isready");
        await WaitForResponse("readyok");
    }

    public async Task NewGame()
    {
        await SendCommand("ucinewgame");
        await SendCommand("isready");
        await WaitForResponse("readyok");
    }

    private async Task SetOption(string name, string value)
    {
        await SendCommand($"setoption name {name} value {value}");
    }

    public async Task<string> GetMove(string fen)
    {
        await SendCommand($"position fen {fen}");
        // spend at most 1000 ms to calculate the move
        await SendCommand("go movetime 1000");
        
        string? line;
        while ((line = await _reader.ReadLineAsync()) is not null)
        {
            if (line.StartsWith("bestmove"))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return parts[1];
            }
        }

        throw new InvalidOperationException(
            "Engine did not return a best move"
        );
    }

    public async Task StopEngine()
    {
        if (!_process.HasExited)
        {
            await SendCommand("quit");
            await _process.WaitForExitAsync();
        }
        _reader.Dispose();
        await _writer.DisposeAsync();
        _process.Dispose();
    }

    public async Task SetElo(int elo)
    {
        elo = Math.Clamp(elo, 1320, 3190);
        await SetOption("UCI_LimitStrength", "true");
        await SetOption("UCI_Elo", elo.ToString());
        await SendCommand("isready");
        await WaitForResponse("readyok");
    }
    
    
    private async Task SendCommand(string command)
    {
        await _writer.WriteLineAsync(command);
        await _writer.FlushAsync();
    }
    
    private async Task WaitForResponse(string expectedResponse)
    {
        string? line;
        while ((line = await _reader.ReadLineAsync()) is not null)
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