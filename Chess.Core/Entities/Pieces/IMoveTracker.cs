namespace chess.Entities.Pieces;

public interface IMoveTracker
{
    bool HasMoved { get; }
    
    void MarkAsMoved();
}