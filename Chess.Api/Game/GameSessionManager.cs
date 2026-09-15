using Chess.Api.Dtos.RequestDtos;
using Chess.Api.Dtos.ResponseDtos;
using Chess.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Chess.Api.Game;

public sealed class GameSessionManager : IDisposable
{
    private static readonly TimeSpan IdleTimeout = TimeSpan.FromMinutes(7);
    private static readonly TimeSpan SweepInterval = TimeSpan.FromMinutes(1);

    private readonly Dictionary<string, GameSession> _sessions = new();
    private readonly Dictionary<string, string> _connectionToSession = new();
    private readonly Dictionary<string, HashSet<string>> _sessionConnections = new();
    private readonly Dictionary<string, DateTime> _idleSince = new();
    private readonly Lock _lock = new();
    private readonly IHubContext<GameHub> _hubContext;
    private readonly Timer _sweepTimer;

    public GameSessionManager(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
        _sweepTimer = new Timer(_ => SweepStaleSessions(), null, SweepInterval, SweepInterval);
    }

    public GameSession CreateSession(string id, StartRequestDto request)
    {
        lock (_lock)
        {
            if (_sessions.ContainsKey(id))
            {
                throw new InvalidOperationException($"Session {id} already exists");
            }

            var session = GameSession.Create(id, request);
            WireNotifications(session);
            _sessions[id] = session;
            _sessionConnections[id] = new HashSet<string>();
            return session;
        }
    }

    private void WireNotifications(GameSession session)
    {
        var group = _hubContext.Clients.Group(session.SessionId);

        session.SnapshotPublished += (snapshot, ct) =>
            group.SendAsync("ReceiveMessage", SnapshotDto.ToSnapshotDto(snapshot), ct);

        session.MoveStatusReceived += (moveStatus, ct) =>
            group.SendAsync("ReceiveMessage", MoveStatusDto.ToMoveStatusDto(moveStatus), ct);

        session.ErrorOccurred += (message, ct) =>
            group.SendAsync("ReceiveMessage", message, ct);
    }

    public GameSession? GetSession(string id)
    {
        lock (_lock)
        {
            _sessions.TryGetValue(id, out var session);
            return session;
        }
    }

    public string? GetSessionIdForConnection(string connectionId)
    {
        lock (_lock)
        {
            _connectionToSession.TryGetValue(connectionId, out var sessionId);
            return sessionId;
        }
    }

    public void RegisterConnection(string connectionId, string sessionId)
    {
        lock (_lock)
        {
            if (_connectionToSession.TryGetValue(connectionId, out var previousSessionId) &&
                previousSessionId != sessionId &&
                _sessionConnections.TryGetValue(previousSessionId, out var previousConnections))
            {
                previousConnections.Remove(connectionId);

                if (previousConnections.Count == 0)
                {
                    _idleSince[previousSessionId] = DateTime.UtcNow;
                }
            }

            _connectionToSession[connectionId] = sessionId;

            if (!_sessionConnections.TryGetValue(sessionId, out var connections))
            {
                connections = new HashSet<string>();
                _sessionConnections[sessionId] = connections;
            }

            connections.Add(connectionId);

            // a live connection cancels any pending eviction for this session
            _idleSince.Remove(sessionId);
        }
    }

    public void RemoveConnection(string connectionId)
    {
        lock (_lock)
        {
            if (!_connectionToSession.Remove(connectionId, out var sessionId))
            {
                return;
            }

            if (_sessionConnections.TryGetValue(sessionId, out var connections))
            {
                connections.Remove(connectionId);

                if (connections.Count == 0)
                {
                    _idleSince[sessionId] = DateTime.UtcNow;
                }
            }
        }
    }

    private void SweepStaleSessions()
    {
        lock (_lock)
        {
            var cutoff = DateTime.UtcNow - IdleTimeout;
            var staleIds = _idleSince
                .Where(kvp => kvp.Value <= cutoff)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var sessionId in staleIds)
            {
                if (_sessions.Remove(sessionId, out var session))
                {
                    session.Cancel();
                }

                _sessionConnections.Remove(sessionId);
                _idleSince.Remove(sessionId);
            }
        }
    }

    public void Dispose() => _sweepTimer.Dispose();
}