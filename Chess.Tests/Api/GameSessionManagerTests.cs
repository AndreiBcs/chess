using Chess.Api.Dtos.RequestDtos;
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

    [Fact]
    public void StartRequestDto_Validate_RejectsUnsupportedEngine()
    {
        var request = new StartRequestDto("white", "leela", 1600);

        var exception = Assert.Throws<ArgumentException>(() => StartRequestDto.Validate(request));

        Assert.Equal("EngineType", exception.ParamName);
    }

    [Fact]
    public void MoveRequestDto_ConvertsPromotionIgnoringCaseAndWhitespace()
    {
        var request = new MoveRequestDto
        {
            From = new PositionDto(6, 0),
            To = new PositionDto(7, 0),
            Promotion = " Queen "
        };

        var move = MoveRequestDto.FromMoveDto(request);

        Assert.Equal(new Position(6, 0), move.From);
        Assert.Equal(new Position(7, 0), move.To);
        Assert.Equal(PieceType.Queen, move.Promotion);
    }

    [Fact]
    public void MoveRequestDto_RejectsUnsupportedPromotion()
    {
        var request = new MoveRequestDto { Promotion = "king" };

        var exception = Assert.Throws<ArgumentException>(() => MoveRequestDto.FromMoveDto(request));

        Assert.Contains("Promotion", exception.Message);
    }

    [Fact]
    public void MoveRequestDto_AllowsMoveWithoutPromotion()
    {
        var request = new MoveRequestDto
        {
            From = new PositionDto(6, 4),
            To = new PositionDto(4, 4)
        };

        var move = MoveRequestDto.FromMoveDto(request);

        Assert.Null(move.Promotion);
    }
}
