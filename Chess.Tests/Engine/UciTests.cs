using chess;
using chess.Board;
using Chess.Engine;
using chess.Game;
using Xunit.Abstractions;

namespace Chess.Tests.Engine;

public class UciTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UciTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Stockfish_ReturnsValidMove()
    {
        var enginePlayer = new EnginePlayer(Color.White, ChessEngine.Stockfish);
        var engine = enginePlayer.Uci;

        var board = Board.CreateInitial();
        await engine.StartEngine();
        await engine.NewGame();

        var snapshot = GameSnapshot.GetInitialGameSnapshot();

        var move = await enginePlayer.GetMoveAsync(snapshot, null);
        _testOutputHelper.WriteLine(move.ToString());
        _testOutputHelper.WriteLine(move.ToNotation());
        
        await engine.StopEngine();

        Assert.Matches("^[a-h][1-8][a-h][1-8][qrbn]?$", move.ToString());
    }
}