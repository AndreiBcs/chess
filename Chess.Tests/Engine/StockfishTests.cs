using Chess.Engine;
using Chess.Engine.Stockfish;
using chess.Entities.Board;
using chess.Entities.Common;
using chess.Game.GameState;
using Xunit.Abstractions;

namespace Chess.Tests.Engine;

public class StockfishTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public StockfishTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Stockfish_ReturnsValidMove()
    {
        var engine = new Stockfish();
        var enginePlayer = new EnginePlayer(engine, Color.White);

        var board = new Board();
        board.InitializeBoard();
        await engine.StartEngine();
        await engine.NewGame();

        var snapshot = new GameSnapshot(
            false,
            Color.White,
            board,
            1,
            0);

        var move = await enginePlayer.GetMoveAsync(snapshot, null);
        _testOutputHelper.WriteLine(move.ToString());
        _testOutputHelper.WriteLine(move.ToNotation());
        
        await engine.StopEngine();

        Assert.Matches("^[a-h][1-8][a-h][1-8][qrbn]?$", move.ToString());
    }
}