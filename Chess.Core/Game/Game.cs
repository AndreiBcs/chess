using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Player;
using chess.Game.GameState;
using chess.Game.Validators;

namespace chess.Game;

public class Game
{
    public Game(Player player1, Player player2)
    {
        Players = [player1, player2];
        Board.InitializeBoard();
    }

    private bool IsOver { get; set; } = false;
    private Color CurrentTurn { get; set; } = Color.White;
    private void SwitchTurn()
    {
        CurrentTurn = CurrentTurn == Color.White ? 
            Color.Black : 
            Color.White;
    }

    private IReadOnlyList<Player> Players { get; }
    private Player GetPlayer(Color color)
    {
        return Players.Single(p => p.Color == color);
    }

    private Board Board { get; } = new();
    private int FullMoveCounter { get; set; } = 1; // increase after black's turn
    private int HalfMoveCounter { get; set; } = 0; // back at 0 after a capture or pawn advance
    private Move CurrentMove { get; set; }
    private Move PreviousMove { get; set; }
    // TODO add en-passant & castling info

    public async IAsyncEnumerable<GameSnapshot> GameLoop()
    {
        while (true)
        {
            var snapshot = CreateSnapshot();
            yield return snapshot;
            
            if(IsOver) yield break;
            
            MoveResult? result = null;

            while (true)
            {
                var currentPlayer = GetPlayer(CurrentTurn);
                var move = await currentPlayer.GetMoveAsync(snapshot, result);
                
                result = MoveValidator.ValidateMove(snapshot, move);

                if (result == MoveResult.Valid)
                {
                    Board.MovePiece(move.From, move.To);
                    break;
                }
            }
            
            SwitchTurn();
        }
    }

    private GameSnapshot CreateSnapshot()
    {
        return new GameSnapshot(IsOver, CurrentTurn, Board, FullMoveCounter, HalfMoveCounter);
    }
    
}
