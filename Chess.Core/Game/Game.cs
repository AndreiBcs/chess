using System.Runtime.CompilerServices;
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
        Snapshots.Add(initialSnapshot);
        _currentSnapshot = Snapshots[^1];
        Status = _currentSnapshot.Status;
    }
    
    public readonly List<GameSnapshot> Snapshots = [];
    private GameSnapshot _currentSnapshot;
    private IReadOnlyList<Player.Player> Players { get; }
    private Player.Player GetPlayer(Color color)
    {
        return Players.Single(p => p.Color == color);
    }

    public async IAsyncEnumerable<GameSnapshot> GameLoop(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (_currentSnapshot.Status == GameStatus.InProgress)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return _currentSnapshot;
            
            MoveStatus? moveStatus = null;

            while (true) // wait for player move and validate
            {
                var currentPlayer = GetPlayer(_currentSnapshot.CurrentTurn);
                var move = await currentPlayer.GetMoveAsync(
                    _currentSnapshot,
                    moveStatus,
                    cancellationToken);
                
                moveStatus = MoveValidator.ValidateMove(_currentSnapshot, move);

                if (moveStatus.Value.MoveResult == MoveResult.Invalid)
                {
                    // move status is not null and is being passed as a warning for the player 
                    continue;
                }

                if (moveStatus.Value.MoveResult == MoveResult.Valid)
                {
                    _currentSnapshot = GameSnapshot
                        .GetUpdatedGameSnapshot(_currentSnapshot, move, moveStatus.Value);
                    
                    Snapshots.Add(_currentSnapshot);
                    Status = _currentSnapshot.Status;
                    break;
                }
            }
        }
        // return final snapshot after game is over
        yield return _currentSnapshot;
    }
}
