using chess.Entities.Board;
using chess.Entities.Common;
using chess.Entities.Pieces;

namespace Chess.Tests.Core.MoveValidation;

public class PawnMoveValidationTests
{
    
    [Theory]
    [InlineData(4, 4, 3, 4)]
    [InlineData(6, 0, 5, 0)]
    [InlineData(1, 2, 0, 2)]
    public void Pawn_White_CanMoveOneSquare(int fromRow, int fromCol, int toRow, int toCol)
    {
        var snapshot = MoveValidationTestsExtensions.GetSnapshotWithPiece(
            PieceType.Pawn,
            Color.White,
            fromRow,
            fromCol);

        var move = new Move
        {
            From = new Position(fromRow, fromCol),
            To = new Position(toRow, toCol)
        };
        
        var result = MoveValidationTestsExtensions.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Valid, result);
    }
    
    [Theory]
    [InlineData(4, 4, 5, 4)]
    [InlineData(6, 0, 7, 0)]
    [InlineData(1, 2, 2, 2)]
    public void Pawn_Black_CanMoveOneSquare(int fromRow, int fromCol, int toRow, int toCol)
    {
        var snapshot = MoveValidationTestsExtensions.GetSnapshotWithPiece(
            PieceType.Pawn,
            Color.Black,
            fromRow,
            fromCol);

        var move = new Move
        {
            From = new Position(fromRow, fromCol),
            To = new Position(toRow, toCol)
        };
        
        var result = MoveValidationTestsExtensions.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Valid, result);
    }
    
    [Theory]
    [InlineData(4, 4, 2, 4)]
    [InlineData(6, 0, 4, 0)]
    public void Pawn_CanMoveTwoSquares(int fromRow, int fromCol, int toRow, int toCol)
    {
        var snapshot = MoveValidationTestsExtensions.GetSnapshotWithPiece(
            PieceType.Pawn,
            Color.White,
            fromRow,
            fromCol);

        var move = new Move
        {
            From = new Position(fromRow, fromCol),
            To = new Position(toRow, toCol)
        };
        
        var result = MoveValidationTestsExtensions.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Valid, result);
    }
    
    [Theory]
    [InlineData(0, 1, -1, 0)]
    public void Pawn_CannotMoveOutsideTheBoard(int fromRow, int fromCol, int toRow, int toCol)
    {
        var snapshot = MoveValidationTestsExtensions.GetSnapshotWithPiece(
            PieceType.Pawn,
            Color.White,
            fromRow,
            fromCol,
            true);

        var move = new Move
        {
            From = new Position(fromRow, fromCol),
            To = new Position(toRow, toCol)
        };
        
        var result = MoveValidationTestsExtensions.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
    
    [Theory]
    [InlineData(6, 0, 4, 0)]
    public void Pawn_CannotMoveTwoSquaresIfAlreadyMoved(
        int fromRow,
        int fromCol, 
        int toRow, 
        int toCol)
    {
        var snapshot = MoveValidationTestsExtensions.GetSnapshotWithPiece(
            PieceType.Pawn,
            Color.White,
            fromRow,
            fromCol,
            true);

        var move = new Move
        {
            From = new Position(fromRow, fromCol),
            To = new Position(toRow, toCol)
        };
        
        var result = MoveValidationTestsExtensions.ValidateMove(snapshot, move);
        
        Assert.Equal(MoveResult.Invalid, result);
    }
}