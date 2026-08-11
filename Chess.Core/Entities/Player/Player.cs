using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Entities.Pieces.Types;

namespace chess.Entities.Player;

public class Player
{
    public Color Color { get; init; }
    private List<Piece> Pieces { get; } = [];
    public King King { get; private set; } = null!;
    public IEnumerable<Piece> ActivePieces =>
        Pieces.Where(p => !p.IsCaptured);

    public void InitializePlayer()
    {
        Pieces.Clear();

        AddPieces<Pawn>(8);
        AddPieces<Rook>(2);
        AddPieces<Knight>(2);
        AddPieces<Bishop>(2);
        AddPieces<Queen>(1);

        King = new King
        {
            Owner = this
        };
        
        Pieces.Add(King);
    }

    private void AddPieces<T>(int howMany)
        where T : Piece, new()
    {
        for (var i = 0; i < howMany; i++)
        {
            Pieces.Add(new T
            {
                Owner = this
            });
        }
    }
}