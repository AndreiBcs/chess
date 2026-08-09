namespace Chess.Cli;

public interface IUserInteraction
{
    (string from, string to) ReadMove(chess.Entities.Board board);
}