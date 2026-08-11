using chess.Game;

namespace Chess.Cli.GameRunner;

public class CliGameLoop
{
    private readonly IBoardRenderer _renderer;
    private readonly IUserInteraction _interaction;
    private readonly Game _game;

    public CliGameLoop(IBoardRenderer renderer, IUserInteraction userInteraction)
    {
        _renderer = renderer;
        _interaction = userInteraction;
        _game = new Game();
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