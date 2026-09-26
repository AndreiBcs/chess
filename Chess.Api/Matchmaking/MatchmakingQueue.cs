namespace Chess.Api.Matchmaking;

public sealed class MatchmakingQueue
{
    private readonly Lock _lock = new();
    private QueuedPlayer? _waiting;

    public MatchResult Enqueue(string connectionId, string playerId)
    {
        // store the first caller or atomically pair it with the next caller
        lock (_lock)
        {
            // enqueue if empty
            if (_waiting == null)
            {
                _waiting = new QueuedPlayer(connectionId, playerId);
                return MatchResult.Waiting();
            }

            // keep the enqueued user if the connection is the same
            if (_waiting.Value.ConnectionId == connectionId)
            {
                return MatchResult.Waiting();
            }

            // if the connection is different pair them
            var opponent = _waiting.Value;
            _waiting = null;
            
            return MatchResult.Paired(opponent, new QueuedPlayer(connectionId, playerId));
        }
    }

    public bool TryCancel(string connectionId)
    {
        // cancel the waiting caller only when the connection IDs match
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