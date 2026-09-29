using chess.Game;
using chess.Moves;
using chess.Pieces;

namespace chess.Player;

public abstract class Player
{
    protected Player(Color color)
    {
        Color = color;
    }
    
    protected Player(Color color, List<PieceType> clashPieces)
    {
        Color = color;
        ClashPieces = clashPieces;
    }

    public Color Color { get; }
    public List<PieceType>? ClashPieces { get; }
    
    // game passes the last move status only if the move was invalid
    public abstract Task<Move> GetMoveAsync(
        GameSnapshot snapshot,
        MoveStatus? moveStatus,
        CancellationToken cancellationToken = default);
}