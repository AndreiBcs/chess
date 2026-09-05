using chess;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces;
using chess.Player;

namespace Chess.Engine;

public sealed class EnginePlayer : Player, IAsyncDisposable
{
    public readonly Uci Uci; // public because the consumer needs to interact with it

    public EnginePlayer(Color color, ChessEngine chessEngine) : base(color)
    {
        var engineFilePath = chessEngine switch
        {
            ChessEngine.Stockfish => 
                Path.Combine(AppContext.BaseDirectory, 
                    "Stockfish", 
                    "stockfish-windows-x86-64-avx2.exe"),
            _ => ""
        };
        
        Uci = new Uci(engineFilePath);
    }
    
    public override async Task<Move> GetMoveAsync(GameSnapshot snapshot, MoveResult? previousResult)
    {
        var fen = GameSnapshot
            .ToFen(
                snapshot.Board,
                snapshot.CurrentTurn,
                snapshot.CastlingRights,
                snapshot.EnPassantTarget,
                snapshot.HalfMoveClock,
                snapshot.FullMoveCounter);
        
        var move = await Uci.GetMove(fen);

        var from = Position.ParsePosition(move[..2]);
        var to = Position.ParsePosition(move[2..4]);

        PieceType? promotion = move.Length switch
        {
            5 => move[4] switch
            {
                'q' => PieceType.Queen,
                'r' => PieceType.Rook,
                'b' => PieceType.Bishop,
                'n' => PieceType.Knight,
                _ => null
            },
            _ => null
        };

        return new Move(from, to, promotion);
    }

    public async ValueTask DisposeAsync()
    {
        await Uci.DisposeAsync();
    }
}

public enum ChessEngine
{
    Stockfish
}