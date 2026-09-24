namespace Chess.Api.Matchmaking;

public sealed class MatchmakingQueue
{
    private readonly Lock _lock = new();
    private QueuedPlayer? _waiting;

    public MatchResult Enqueue(string connectionId, string playerId)
    {
        lock (_lock)
        {
            if (_waiting == null)
            {
                _waiting = new QueuedPlayer(connectionId, playerId);
                return MatchResult.Waiting();
            }

            if (_waiting.Value.ConnectionId == connectionId)
            {
                return MatchResult.Waiting();
            }

            var opponent = _waiting.Value;
            _waiting = null;
            
            return MatchResult.Paired(opponent, new QueuedPlayer(connectionId, playerId));
        }
    }

    public bool TryCancel(string connectionId)
    {
        lock (_lock)
        {
            if (_waiting?.ConnectionId == connectionId)
            {
                _waiting = null;
                return true;
            }
            
            return false;
        }
    }
}