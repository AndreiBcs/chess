using Chess.Api.Game;
using chess.Board;
using chess.Moves;
using chess.Pieces;

namespace Chess.Tests.Api;

public class GameSessionManagerTests
{
    [Fact]
    public void RegisterConnection_MapsConnectionIdToSessionId()
    {
        using var manager = new GameSessionManager(null!);

        manager.RegisterConnection("connection-1", "session-1");

        Assert.Equal("session-1", manager.GetSessionIdForConnection("connection-1"));
    }

    [Fact]
    public void RemoveConnection_ClearsSessionMapping()
    {
        using var manager = new GameSessionManager(null!);

        manager.RegisterConnection("connection-1", "session-1");
        manager.RemoveConnection("connection-1");

        Assert.Null(manager.GetSessionIdForConnection("connection-1"));
    }

    [Fact]
    public void RegisterConnection_ReassignsConnectionToNewSession()
    {
        using var manager = new GameSessionManager(null!);

        manager.RegisterConnection("connection-1", "session-1");
        manager.RegisterConnection("connection-1", "session-2");

        Assert.Equal("session-2", manager.GetSessionIdForConnection("connection-1"));
    }

}
