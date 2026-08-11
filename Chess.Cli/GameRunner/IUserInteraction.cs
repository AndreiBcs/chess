using chess.Entities.Board;

namespace Chess.Cli.GameRunner;

public interface IUserInteraction
{
    Move ReadMove();
}