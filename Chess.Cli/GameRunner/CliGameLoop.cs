using chess.Game;

namespace Chess.Cli.GameRunner;

public class CliGameLoop
{
    private readonly IBoardRenderer _renderer;
    private readonly IUserInteraction _interaction;
    private readonly Game _game;

    public CliGameLoop(IBoardRenderer renderer, IUserInteraction userInteraction, Game game)
    {
        _renderer = renderer;
        _interaction = userInteraction;
        _game = game;
    }

    public void Run()
    {
        while (!_game.IsOver)
        {
            _renderer.Render(_game.Board);
            var move = _interaction.ReadMove();
            _game.ApplyMove(move);
        }
    }
}