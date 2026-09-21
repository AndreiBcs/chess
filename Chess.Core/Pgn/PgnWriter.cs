using System.Text;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces;
using chess.Validation.MoveValidation;

namespace chess.Pgn;

public static class PgnWriter
{
    private const string Event = "Chess CLI";
    private const string Site = "Local";
    private const string Round = "-";

    public static string Write(
        List<GameSnapshot> snapshots,
        Color playerColor,
        string playerName,
        string engineName,
        int engineElo)
    {
        var pgn = new StringBuilder();
        var moves = new StringBuilder();
        var index = 0;
        
        // metadata
        pgn.Append($"[Event \"{Event}\"]").AppendLine();
        pgn.Append($"[Site \"{Site}\"]").AppendLine();
        pgn.Append($"[Date \"{DateTime.Now:yyyy.MM.dd}\"]").AppendLine();
        pgn.Append($"[Round \"{Round}\"]").AppendLine();

        if (playerColor == Color.White)
        {
            pgn.Append($"[White \"{playerName}\"]").AppendLine();
            pgn.Append($"[Black \"{engineName}\"]").AppendLine();
        }
        else
        {
            pgn.Append($"[White \"{engineName}\"]").AppendLine();
            pgn.Append($"[Black \"{playerName}\"]").AppendLine();
        }

        if (snapshots[^1].Status is GameStatus.WhiteWon)
        {
            pgn.Append("[Result \"1-0\"]").AppendLine();    
        }
        else if (snapshots[^1].Status is GameStatus.BlackWon)
        {
            pgn.Append("[Result \"0-1\"]").AppendLine();    
        }
        else
        {
            pgn.Append("[Result \"1/2-1/2\"]").AppendLine();
        }
        
        pgn.Append($"[EngineElo \"{engineElo}\"]").AppendLine();
        pgn.AppendLine();

        // moves
        foreach (var snapshot in snapshots[1..]) // without initial one
        {
            var status = snapshot.PreviousMoveStatus;
            var move = snapshot.PreviousMove;
            var previousSnapshot = snapshots[index];

            if (index++ % 2 == 0)
            {
                moves.Append(snapshot.FullMoveCounter).Append(". ");
            }

            // castling
            if (status is { IsCastling: true, CastlingRights: not null })
            {
                if (status.CastlingRights?.LetterId.ToString().ToLower() is "k")
                {
                    moves.Append("O-O");
                }
                else if (status.CastlingRights?.LetterId.ToString().ToLower() is "q")
                {
                    moves.Append("O-O-O");
                }
            }
            else
            {
                var piece = previousSnapshot.Board.GetPiece(move.From);

                if (piece is null)
                {
                    moves.Append(" Error constructing SAN... ");
                    break;
                }
                
                // pawn move
                if (status.IsPawnMove)
                {
                    if (status.IsCapture)
                    {
                        moves.Append(move.From.ToString()[0])
                            .Append('x');
                    }
                }
                // piece move
                else
                {
                    moves.Append(piece.LetterId.ToString().ToUpper())
                        .Append(GetDisambiguation(previousSnapshot, move, piece));

                    if (status.IsCapture)
                    {
                        moves.Append('x');
                    }
                }

                moves.Append(move.To.ToString());

                // promotion
                if (status.IsPromotion && move.Promotion != null)
                {
                    moves.Append('=')
                        .Append(move.Promotion.Value.ToString().ToUpper()[0]);
                }

                // game status
                if (snapshot.Status is GameStatus.BlackWon)
                {
                    moves.Append("# 0-1");
                    break;
                }
                
                if (snapshot.Status is GameStatus.WhiteWon)
                {
                    moves.Append("# 1-0");
                    break;
                }

                if (snapshot.Status is not GameStatus.InProgress)
                {
                    moves.Append(" 1/2-1/2");
                    break;
                }
                
                if (status.IsCheck)
                {
                    moves.Append('+');
                }
            }
            
            moves.Append(' ');
        }
        
        // format moves
        const int maxLineLength = 80;

        var moveText = moves.ToString().Trim();
        var currentLineLength = 0;

        foreach (var move in moveText.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (currentLineLength + move.Length + 1 > maxLineLength)
            {
                pgn.AppendLine();
                currentLineLength = 0;
            }

            if (currentLineLength > 0)
            {
                pgn.Append(' ');
                currentLineLength++;
            }

            pgn.Append(move);
            currentLineLength += move.Length;
        }

        pgn.AppendLine();
        
        return pgn.ToString();
    }
    
    private static string GetDisambiguation(
        GameSnapshot previousSnapshot,
        Move move,
        Piece piece)
    {
        var candidates = previousSnapshot.Board
            .PositionsOfPiecesWithSameTypeAndColor(piece.Type, piece.Color)
            .Where(pos => pos != move.From)
            .Where(pos => MoveValidator.ValidateMove(
                previousSnapshot,
                new Move(pos, move.To)).MoveResult == MoveResult.Valid)
            .ToList();

        if (candidates.Count == 0)
            return string.Empty;

        var sameFileExists = candidates.Any(p => p.Column == move.From.Column);
        var sameRankExists = candidates.Any(p => p.Row == move.From.Row);

        if (sameFileExists && sameRankExists)
            return move.From.ToString(); // file + rank

        if (sameFileExists)
            return $"{8 - move.From.Row}"; // rank only

        if (sameRankExists)
            return $"{(char)('a' + move.From.Column)}"; // file only

        return $"{(char)('a' + move.From.Column)}"; // file is enough
    }
}