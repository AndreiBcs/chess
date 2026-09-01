namespace chess.Pieces;

public interface IMoveTracker
{
    bool HasMoved { get; }
    
    void MarkAsMoved();
}