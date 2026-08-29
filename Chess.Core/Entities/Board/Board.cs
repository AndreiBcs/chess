using chess.Entities.Common;
using chess.Entities.Pieces;
using chess.Entities.Pieces.Types;

namespace chess.Entities.Board;

public class Board : IReadOnlyBoard
{
    public Square[,] Squares { get; } = new Square[8, 8];

    public void InitializeBoard()
    {
        InitializeSquares();
        SetupPlayerSide(Color.White, 7, 6);
        SetupPlayerSide(Color.Black, 0, 1);
    }
    
    public Piece? GetPiece(Position position)
    {
        if (position.Row is < 0 or >= 8 ||
            position.Column is < 0 or >= 8)
            return null;
        
        return Squares[position.Row, position.Column].Piece;
    }

    public MoveResult ValidateKingSafety(Move move)
    {
        var piece = GetPiece(move.From);

        if (piece is null)
            return MoveResult.Invalid;
        
        var boardCopy = Copy();
        boardCopy.MovePiece(move.From, move.To);
        
        return VerifyKing(boardCopy, piece.Color);
    } 

    public void MovePiece(Position from, Position to)
    {
        var piece = GetPiece(from);

        if (piece is null) return;

        var capturedPiece = GetPiece(to);
        capturedPiece?.MarkAsCaptured();
        
        Squares[to.Row, to.Column].Piece = piece;
        Squares[from.Row, from.Column].Piece = null;

        if (piece is IMoveTracker moveTracker)
        {
            moveTracker.MarkAsMoved();
        }
    }

    private void InitializeSquares()
    {
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                Squares[row, col] = new Square
                {
                    Color = (row + col) % 2 == 0
                        ? Color.White
                        : Color.Black,
                    Position = new Position(row, col)
                };
            }
        }
    }
    
    private void SetupPlayerSide(Color color, int majorRow, int pawnRow)
    {
        Squares[majorRow, 0].Piece = new Rook(color, false);
        Squares[majorRow, 1].Piece = new Knight(color);
        Squares[majorRow, 2].Piece = new Bishop(color);
        Squares[majorRow, 3].Piece = new Queen(color);
        Squares[majorRow, 4].Piece = new King(color, false);
        Squares[majorRow, 5].Piece = new Bishop(color);
        Squares[majorRow, 6].Piece = new Knight(color);
        Squares[majorRow, 7].Piece = new Rook(color, false);

        for (var i = 0; i < 8; i++)
        {
            Squares[pawnRow, i].Piece = new Pawn(color, false);
        }
    }

    private Position GetKingPosition(Color color)
    {
        var pos = new Position();
        foreach (var sq in Squares)
        {
            if (sq.Piece?.Color == color &&
                sq.Piece.Type == PieceType.King)
            {
                pos = sq.Position;
            }
        }

        return pos;
    }
    
    private Board Copy()
    {
        var board = new Board();

        for (var row = 0; row < 8; row++)
        {
            for (var column = 0; column < 8; column++)
            {
                var originalSquare = Squares[row, column];

                board.Squares[row, column] = new Square
                {
                    Color = originalSquare.Color,
                    Position = new Position(row, column),
                    Piece = originalSquare.Piece?.Copy()
                };
            }
        }

        return board;
    }
    
    private static MoveResult VerifyKing(Board board, Color kingColor)
    {
        var kingPosition = board.GetKingPosition(kingColor);

        var enemyColor = kingColor == Color.White
            ? Color.Black
            : Color.White;
        
        var checkPositions = new List<Position>();
        var friendlyPositions = new List<Position>();

        foreach (var square in board.Squares)
        {
            var piece = square.Piece;

            if (piece?.Color == enemyColor)
            {
                friendlyPositions.AddRange(piece
                    .GetPossiblePositions(board, square.Position));
            }
            else if (piece?.Color == enemyColor)
            {
                checkPositions.AddRange(piece
                    .GetPossiblePositions(board, square.Position));
            }
        }

        if (checkPositions.Contains(kingPosition))
        {
            if (friendlyPositions.Count == 0)
            {
                return MoveResult.Checkmate;
            }
        }
        
        // TODO complete the verification

        return MoveResult.Valid;
    }
}

public interface IReadOnlyBoard
{
    Piece? GetPiece(Position position);
    MoveResult ValidateKingSafety(Move move);
}

