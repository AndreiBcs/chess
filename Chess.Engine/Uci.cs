namespace Chess.Engine;

// ReSharper disable once InconsistentNaming
public interface Uci
{
    void StartEngine();
    void NewGame();
    void SetOption(string name, string value);
    string GetMove(string fen);
    void StopEngine();
}