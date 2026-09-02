namespace chess.Game;

public class Game
{
    public Game(Player.Player player1, Player.Player player2)
    {
        Players = [player1, player2];
    }

    private readonly List<GameState> _states = [];
    public IReadOnlyList<GameState> States => _states;
    public GameState CurrentState => _states[^1];
    private IReadOnlyList<Player.Player> Players { get; }
    private Player.Player GetPlayer(Color color)
    {
        return Players.Single(p => p.Color == color);
    }
    private GameStatus GameStatus { get; set; } = GameStatus.InProgress;

    public async IAsyncEnumerable<GameState> RunGame()
    {
        while (GameStatus == GameStatus.InProgress)
        {
            yield return CurrentState;
        }
    }
}