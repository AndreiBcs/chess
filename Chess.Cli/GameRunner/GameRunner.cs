using Chess.Engine.Configs;
using chess.Entities.Common;
using chess.Game;

namespace Chess.Cli.GameRunner;

public class GameRunner
{
    private readonly IBoardRenderer _renderer;
    private readonly IUserInteraction _interaction;
    private readonly Game _game;

    public GameRunner(
        IBoardRenderer renderer, 
        IUserInteraction userInteraction, 
        Difficulty difficulty, 
        Color playerColor)
    {
        _renderer = renderer;
        _interaction = userInteraction;
        //_game = new Game();
    }

    public void Run()
    {
        _game.StartGame();
        while (!_game.IsOver)
        {
            _renderer.Render(_game.Board);
            var move = _interaction.ReadMove();
        }
    }
}