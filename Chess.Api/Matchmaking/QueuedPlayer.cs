namespace Chess.Api.Matchmaking;

public readonly record struct QueuedPlayer(
    string ConnectionId,
    string PlayerId);