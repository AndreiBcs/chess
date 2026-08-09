using chess.Entities;

namespace chess.Game;

public class Game
{
    public bool IsOver { get; set; } = false;
    private Player PlayerWhite { get; } = new() { Color = Color.White };
    private Player PlayerBlack { get; } = new() { Color = Color.Black };
    public Board Board { get; } = new();

    public void StartGame()
    {
        PlayerWhite.InitializePlayer();
        PlayerBlack.InitializePlayer();
        Board.InitializeStartingBoard(PlayerWhite, PlayerBlack);
    }

    public void ApplyMove(((int fromRow, int fromCol), (int toRow, int toCol)) move)
    {
        var (fromRow, fromCol) = move;
        // TODO
    }
}