using chess.Pieces;

namespace chess.Validation.ClashModeValidation;

public static class StartingPositionValidator
{
    public static bool ValidateStartingPosition(List<PieceType> pieces)
    {
        var pawns = pieces.Where(p => p == PieceType.Pawn).ToList();
        var knights = pieces.Where(p => p == PieceType.Knight).ToList();
        var bishops = pieces.Where(p => p == PieceType.Bishop).ToList();
        var rooks = pieces.Where(p => p == PieceType.Rook).ToList();
        var queens = pieces.Where(p => p == PieceType.Queen).ToList();
        var kings = pieces.Where(p => p == PieceType.King).ToList();

        if (pawns.Count != 8 ||
            knights.Count != 2 ||
            bishops.Count != 2 ||
            rooks.Count != 2 ||
            queens.Count != 1 ||
            kings.Count != 1)
        {
            return false;
        }
        
        var kingIndex = pieces.IndexOf(kings.First());
        
        if(kingIndex is < 8 or > 15)
            return false;
        
        return true;
    }
    
    public static List<PieceType> RandomClashSetup()
    {
        var pieceList = new List<PieceType>();

        for (var i = 0; i < 8; i++)
        {
            pieceList.Add(PieceType.Pawn);
        }
        
        pieceList.Add(PieceType.Bishop);
        pieceList.Add(PieceType.Bishop);
        pieceList.Add(PieceType.Rook);
        pieceList.Add(PieceType.Rook);
        pieceList.Add(PieceType.Knight);
        pieceList.Add(PieceType.Knight);
        pieceList.Add(PieceType.Queen);

        var pieceTypes = pieceList.Shuffle().ToList();
        var randomKingIndex = new Random().Next(8, 16);
        pieceTypes.Insert(randomKingIndex, PieceType.King);
        
        return pieceTypes;
    }
}