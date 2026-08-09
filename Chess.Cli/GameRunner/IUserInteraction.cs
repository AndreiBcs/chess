namespace Chess.Cli.GameRunner;

public interface IUserInteraction
{
    ((int rowFrom, int colFrom), (int rowTo, int colTo)) ReadMove();
}