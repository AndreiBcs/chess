using Chess.Cli.GameRunner;

namespace Chess.Cli.Interactions;

public class TextUserInteraction : IUserInteraction
{
    public ((int rowFrom, int colFrom), (int rowTo, int colTo)) ReadMove()
    {
        throw new NotImplementedException();
    }
}