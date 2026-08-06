using chess.Entities.Pieces;

namespace chess.Entities;

public class Player
{
    public PlayerColor Color { get; set; }
    public byte Score { get; set; }
    public List<Pawn> Pawns { get; set; } = [];
    public List<Rook> Rooks { get; set; } = [];
    public List<Knight> Knights { get; set; } = [];
    public List<Bishop> Bishops { get; set; } = [];
    public Queen Queen { get; set; } = new Queen();
    public King King { get; set; } = new King();

    public void InitializePlayer(PlayerColor color)
    {
        Color = color;
        
        Score = 0;
        
        var pieceColor = color == PlayerColor.PlayerWhite ? 
                            PieceColor.White : 
                            PieceColor.Black;
        
        var isWhite = color == PlayerColor.PlayerWhite;
        
        for (var i = 0; i < 8; i++)
        {
            Pawns.Add(new Pawn
            {
                Color = pieceColor, 
                LetterId = isWhite ? 'P' : 'p',
                Icon = isWhite ? '♟' : '♙'
            });
        }
        
        Rooks.Add(new Rook
        {
            Color = pieceColor, 
            LetterId = isWhite ? 'R' : 'r',
            Icon = isWhite ? '♜' : '♖'
        });
        Rooks.Add(new Rook
        {
            Color = pieceColor, 
            LetterId = isWhite ? 'R' : 'r',
            Icon = isWhite ? '♜' : '♖'
        });
        
        Knights.Add(new Knight
        {
            Color = pieceColor, 
            LetterId = isWhite ? 'N' : 'n',
            Icon = isWhite ? '♞' : '♘'
        });
        Knights.Add(new Knight
        {
            Color = pieceColor, 
            LetterId = isWhite ? 'N' : 'n',
            Icon = isWhite ? '♞' : '♘'
        });
        
        Bishops.Add(new Bishop
        {
            Color = pieceColor, 
            LetterId = isWhite ? 'B' : 'b',
            Icon = isWhite ? '♝' : '♗'
        });
        Bishops.Add(new Bishop
        {
            Color = pieceColor, 
            LetterId = isWhite ? 'B' : 'b',
            Icon = isWhite ? '♝' : '♗'
        });
        
        Queen = new Queen
        {
            Color = pieceColor, 
            LetterId = isWhite ? 'Q' : 'q',
            Icon = isWhite ? '♛' : '♕'
        };
        
        King = new King
        {
            Color = pieceColor, 
            LetterId = isWhite ? 'K' : 'k',
            Icon = isWhite ? '♚' : '♔'
        };
    }
}

public enum PlayerColor
{
    // unrelated to piece color or square color
    PlayerWhite,
    PlayerBlack
}