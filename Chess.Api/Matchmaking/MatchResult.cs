namespace Chess.Api.Matchmaking;

using chess;

public readonly record struct MatchResult(
    bool Matched,
    QueuedPlayer? First,
    QueuedPlayer? Second,
    string? SessionId = null,
    Color? PlayerColor = null)
{
    public static MatchResult Waiting() => 
        new(false, null, null);
    
    public static MatchResult Paired(QueuedPlayer first, QueuedPlayer second) => 
        new(true, first, second);

    public MatchResult ForPlayer(string sessionId, string connectionId, Color color) =>
        new(Matched, First, Second, sessionId, color);
}