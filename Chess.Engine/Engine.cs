using System.Diagnostics;

namespace Chess.Engine;

public class Engine
{
    private Process _process;
    private StreamReader _reader;
    private StreamWriter _writer;
    
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
        _reader = _process.StandardOutput;
        _writer = _process.StandardInput;
        
        try
        {
            _process.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        StartUci();
    }

    private void StartUci()
    {
        SendCommand("uci");
        
        while (true)
        {
            var line = _reader.ReadLine();
            if (string.Equals(line, "uciok", 
                    StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
        }
        
        SendCommand("isready");
        
        var ready = _reader.ReadLine();
        if (ready is null || !string.Equals(ready, "readyok",
                StringComparison.OrdinalIgnoreCase))
        {
            QuitUci();
            
        }
    }
    
    public void QuitUci()
    {
        SendCommand("quit");
        _process.WaitForExit();
    }
    
    private void SendCommand(string command)
    {
        _writer.WriteLine(command);
        _writer.Flush();
    }
}
