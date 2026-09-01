using chess;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces;
using chess.Pieces.Types;
using Chess.Tests.Core.Board;
using chess.Validation;

namespace Chess.Tests.Core.Game;

public class CastlingRightsTests
{
    [Fact]
    public void CanCastleKingside()
    {
        var snapshot = CreateSnapshot(
            new King(Color.White),
            new Rook(Color.White),
            new King(Color.Black),
            new Position(7, 4),
            new Position(7, 7),
            new Position(0, 0));

        var result = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(7, 4), new Position(7, 6)));

        Assert.Equal(MoveResult.Valid, result.Result);
    }

    [Fact]
    public void CanCastleQueenside()
    {
        var snapshot = CreateSnapshot(
            new King(Color.White),
            new Rook(Color.White),
            new King(Color.Black),
            new Position(7, 4),
            new Position(7, 0),
            new Position(0, 0));

        var result = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(7, 4), new Position(7, 2)));

        Assert.Equal(MoveResult.Valid, result.Result);
    }

    [Fact]
    public void CannotCastleThroughCheck()
    {
        var snapshot = CreateSnapshot(
            new King(Color.White),
            new Rook(Color.White),
            new King(Color.Black),
            new Position(7, 4),
            new Position(7, 7),
            new Position(0, 4),
            new Rook(Color.Black),
            new Position(0, 5));

        var result = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(7, 4), new Position(7, 6)));

        Assert.Equal(MoveResult.Invalid, result.Result);
    }

    [Fact]
    public void CannotCastleWhileInCheck()
    {
        var snapshot = CreateSnapshot(
            new King(Color.White),
            new Rook(Color.White),
            new King(Color.Black),
            new Position(7, 4),
            new Position(7, 7),
            new Position(0, 0),
            new Rook(Color.Black),
            new Position(0, 4));

        var result = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(7, 4), new Position(7, 6)));

        Assert.Equal(MoveResult.Invalid, result.Result);
    }

    private static GameSnapshot CreateSnapshot(
        Piece whiteKing,
        Piece whiteRook,
        Piece blackKing,
        Position whiteKingPosition,
        Position whiteRookPosition,
        Position blackKingPosition,
        Piece? extraBlackPiece = null,
        Position? extraBlackPiecePosition = null)
    {
        var board = new chess.Board.Board();
        board.CreateEmpty()
            .PlacePiece(whiteKing, whiteKingPosition)
            .PlacePiece(whiteRook, whiteRookPosition)
            .PlacePiece(blackKing, blackKingPosition);

        if (extraBlackPiece is not null && extraBlackPiecePosition is not null)
        {
            board.PlacePiece(extraBlackPiece, extraBlackPiecePosition.Value);
        }

        return new GameSnapshot(
            GameStatus.InProgress, 
            Color.White, 
            board, 
            1, 
            0,
            new CastlingRights(), 
            new List<Move>(), 
            null);
    }
}