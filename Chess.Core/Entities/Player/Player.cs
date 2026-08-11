using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Entities.Pieces.Types;

namespace chess.Entities.Player;

public class Player
{
    public Color Color { get; init; }
    public List<Piece> Pieces { get; } = [];
    public King King { get; private set; } = null!;
    public IEnumerable<Piece> ActivePieces => Pieces.Where(p => !p.IsCaptured);

    public void InitializePlayer()
    {
        Pawns = CreatePiece<Pawn>(Color, 'P', 8);
        Rooks  = CreatePiece<Rook>(Color, 'R', 2);
        Knights = CreatePiece<Knight>(Color, 'N', 2);
        Bishops = CreatePiece<Bishop>(Color, 'B', 2);
        Queen = new Queen
        {
            Color = Color,
            LetterId = Color == Color.White ? 'Q' : 'q'
        };
        King = new King{
            Color = Color,
            LetterId = Color == Color.White ? 'K' : 'k'
        };
    }

    private static T[] CreatePiece<T>
    (
        Color color, 
        char letter, 
        int howMany) 
        where T : Piece, new()
    {
        var pieces = new T[howMany];
        
        for (var i = 0; i < howMany; i++)
        {
            pieces[i] = new T
            {
                Color = color,
                LetterId = color == Color.White ? 
                    letter.ToString().ToUpper().First() : 
                    letter.ToString().ToLower().First()
            };
        }

        return pieces;
    }
}
