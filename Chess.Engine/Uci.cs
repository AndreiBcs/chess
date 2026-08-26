namespace Chess.Engine;

// ReSharper disable once InconsistentNaming
public interface Uci
{
    Task StartEngine();
    Task NewGame();
    Task SetOption(string name, string value);
    Task<string> GetMove(string fen);
    Task StopEngine();
}