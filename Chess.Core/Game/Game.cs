using chess.Moves;
using chess.Validation.MoveValidation;

namespace chess.Game;

public sealed class Game
{
    public GameStatus Status;
    public Game(Player.Player player1, Player.Player player2)
    {
        Players = [player1, player2];
        
        var initialSnapshot = GameSnapshot.GetInitialGameSnapshot();
        _snapshots.Add(initialSnapshot);
        _currentSnapshot = _snapshots[^1];
        Status = _currentSnapshot.Status;
    }
    
    private readonly List<GameSnapshot> _snapshots = [];
    private GameSnapshot _currentSnapshot;
    private IReadOnlyList<Player.Player> Players { get; }
    private Player.Player GetPlayer(Color color)
    {
        return Players.Single(p => p.Color == color);
    }

    public async IAsyncEnumerable<GameSnapshot> GameLoop()
    {
        while (_currentSnapshot.Status == GameStatus.InProgress)
        {
            yield return _currentSnapshot;
            
            MoveResult? result = null;

            while (true) // wait for player move and validate
            {
                var currentPlayer = GetPlayer(_currentSnapshot.CurrentTurn);
                var move = await currentPlayer.GetMoveAsync(_currentSnapshot, result);
                
                var status = MoveValidator.ValidateMove(_currentSnapshot, move);
                result = status.MoveResult;

                if (result == MoveResult.Invalid)
                {
                    continue;
                }

                if (result == MoveResult.Valid)
                {
                    _currentSnapshot = GameSnapshot
                        .GetUpdatedGameSnapshot(_currentSnapshot, move, status);
                    
                    _snapshots.Add(_currentSnapshot);
                    Status = _currentSnapshot.Status;
                    break;
                }
            }
        }
        // return final snapshot after game is over
        yield return _currentSnapshot;
    }
}
