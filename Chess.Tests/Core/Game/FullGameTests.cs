using chess;
using Chess.Engine;
using chess.Game;
using Xunit.Abstractions;
// ReSharper disable Xunit.XunitTestWithConsoleOutput

namespace Chess.Tests.Core.Game;

public class FullGameTests
{
    private readonly ITestOutputHelper _output;

    public FullGameTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task StockfishPlaysStockfish()
    {
        const int games = 1;
        
        for (var gameNumber = 1; gameNumber <= games; gameNumber++)
        {
            Console.WriteLine($"Starting game {gameNumber}");
            //_output.WriteLine($"======== Game {gameNumber} ========");
            
            using var white = new EnginePlayer(Color.White, ChessEngine.Stockfish);
            using var black = new EnginePlayer(Color.Black, ChessEngine.Stockfish);
            
            Console.WriteLine("Starting white Stockfish...");
            await white.Uci.StartEngine();
            
            Console.WriteLine("Starting black Stockfish...");
            await black.Uci.StartEngine();
            
            Console.WriteLine("Starting white new game...");
            await white.Uci.NewGame();
            
            Console.WriteLine("Starting black new game...");
            await black.Uci.NewGame();
            
            var game = new chess.Game.OldGame(white, black);
            var moveNumber = 0;

            try
            {
                Console.WriteLine("Starting GameLoop...");
                
                await foreach (var snapshot in game.GameLoop())
                {
                    Console.WriteLine($"Fen pos: {snapshot.ToFen()}");
                    
                    moveNumber++;
                    
                    //_output.WriteLine($"Move: {moveNumber} | Turn: {snapshot.CurrentTurn}");
                    
                    //_output.WriteLine($"Fen: {snapshot.ToFen()}");
                    
                    Assert.True(moveNumber <= 500, $"Game {gameNumber} exceeded 500 moves");
                }
                
                Console.WriteLine($"Game {gameNumber} finished");
                //_output.WriteLine($"Game {gameNumber} finished after {moveNumber} moves");
                //_output.WriteLine($"Result: {game.Status}");
                
                Assert.True(game.IsFinished, $"Game {gameNumber} ended without IsOver being true");
                Assert.Contains(game.Status, new []
                {
                    GameStatus.Draw,
                    GameStatus.BlackWon,
                    GameStatus.WhiteWon
                });
            }
            catch (Exception ex)
            {
                //_output.WriteLine($"GAME {gameNumber} FAILED on move {moveNumber}");
                //_output.WriteLine($"Exception: {ex}");
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}