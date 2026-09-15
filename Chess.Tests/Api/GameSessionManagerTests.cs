using Chess.Api.Dtos.RequestDtos;
using Chess.Api.Game;

namespace Chess.Tests.Api;

public class GameSessionManagerTests
{
    [Fact]
    public void RegisterConnection_MapsConnectionIdToSessionId()
    {
        var manager = new GameSessionManager(null);

        manager.RegisterConnection("connection-1", "session-1");

        Assert.Equal("session-1", manager.GetSessionIdForConnection("connection-1"));
    }

    [Fact]
    public void RemoveConnection_ClearsSessionMapping()
    {
        var manager = new GameSessionManager(null);

        manager.RegisterConnection("connection-1", "session-1");
        manager.RemoveConnection("connection-1");

        Assert.Null(manager.GetSessionIdForConnection("connection-1"));
    }

    [Fact]
    public void StartRequestDto_Validate_RejectsInvalidPlayerColor()
    {
        var request = new StartRequestDto("yellow", "stockfish", 1600);

        var exception = Assert.Throws<ArgumentException>(() => StartRequestDto.Validate(request));

        Assert.Contains("PlayerColor", exception.ParamName ?? exception.Message);
    }

    [Fact]
    public void StartRequestDto_Validate_RejectsInvalidElo()
    {
        var request = new StartRequestDto("white", "stockfish", 0);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => StartRequestDto.Validate(request));

        Assert.Equal("Elo", exception.ParamName);
    }
}
