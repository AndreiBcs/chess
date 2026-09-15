namespace Chess.Api.Game;

public sealed class GameSessionManager
{
    private readonly Dictionary<string, GameSession> _sessions = new();
    private readonly Lock _lock = new();

    public GameSession CreateSession(string id, GameRunner runner)
    {
        lock (_lock)
        {
            if (_sessions.ContainsKey(id))
            {
                throw new InvalidOperationException($"Session {id} already exists");
            }

            var session = new GameSession(id, runner);
            _sessions[id] = session;
            return session;
        }
    }

    public GameSession? GetSession(string id)
    {
        lock (_lock)
        {
            _sessions.TryGetValue(id, out var session);
            return session;
        }
    }
    
    public void RemoveSession(string id)
    {
        lock (_lock)
        {
            _sessions.Remove(id);
        }
    }

    public IEnumerable<string> GetActiveSessions()
    {
        lock (_lock)
        {
            return _sessions.Keys.ToList();
        }
    }
}