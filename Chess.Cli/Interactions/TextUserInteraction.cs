namespace Chess.Cli.Interactions;

public class TextUserInteraction : IUserInteraction
{
    private readonly IBoardRenderer _renderer;

    public TextUserInteraction(IBoardRenderer renderer)
    {
        _renderer = renderer;
    }

    public (string from, string to) ReadMove(chess.Entities.Board board)
    {
        throw new NotImplementedException();
    }
}