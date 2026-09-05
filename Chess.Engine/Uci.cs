using System.Diagnostics;

namespace Chess.Engine;

public class Uci
{
    private static readonly TimeSpan ResponseTimeout = TimeSpan.FromSeconds(5);
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
        await SendCommand("go movetime 1000");
        
        using var timeout = new CancellationTokenSource(ResponseTimeout);
        try
        {
            string? line;
            while ((line = await _reader.ReadLineAsync(timeout.Token)) is not null)
            {
                if (line.StartsWith("bestmove", StringComparison.Ordinal))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        return parts[1];
                    }

                    throw new InvalidOperationException($"Engine returned malformed response: '{line}'");
                }
            }
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested)
        {
            throw new TimeoutException($"Engine did not return a best move within {
                ResponseTimeout.TotalSeconds:0} seconds for FEN '{fen}'");
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
        using var timeout = new CancellationTokenSource(ResponseTimeout);
        try
        {
            string? line;
            while ((line = await _reader.ReadLineAsync(timeout.Token)) is not null)
            {
                if (string.Equals(line, expectedResponse,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested)
        {
            throw new TimeoutException($"Stockfish did not return '{expectedResponse}' within {ResponseTimeout.TotalSeconds:0} seconds.");
        }
        
        throw new InvalidOperationException(
            $"Stockfish exited before sending '{expectedResponse}'"
        );
    }
}