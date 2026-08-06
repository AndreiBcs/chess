using chess.Entities.Pieces;

namespace chess.Entities;

public class Player
{
    public Color Color { get; init; }
    public int Score { get; set; } = 0;
    public Pawn[] Pawns { get; set; } = [];
    public Rook[] Rooks { get; set; } = [];
    public Knight[] Knights { get; set; } = [];
    public Bishop[] Bishops { get; set; } = [];
    public Queen Queen { get; set; } = new();
    public King King { get; set; } = new();

    public void InitializePlayer()
    {
        Pawns = CreatePiece<Pawn>(Color, 'P', '_', 8);
        Rooks  = CreatePiece<Rook>(Color, 'R', '_', 2);
        Knights = CreatePiece<Knight>(Color, 'N', '_', 2);
        Bishops = CreatePiece<Bishop>(Color, 'B', '_', 2);
        Queen = new Queen
        {
            Color = Color,
            LetterId = Color == Color.White ? 'Q' : 'q',
            Icon = Color == Color.White ? ' ' : '_'
        };
        King = new King{
            Color = Color,
            LetterId = Color == Color.White ? 'K' : 'k',
            Icon = Color == Color.White ? ' ' : '_'
        };
    }

    private static T[] CreatePiece<T>
    (
        Color color, 
        char letter, 
        char icon, 
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
                    letter.ToString().ToLower().First(),
                Icon = color == Color.White ? ' ' : icon
            };
        }

        return pieces;
    }
}
