using chess.Board;
using chess.Pieces;
using chess.Validation.ClashModeValidation;

namespace Chess.Tests.Core.ValidationTests;

public class ClashModeTests
{
    [Fact]
    public void ValidStartingPosition()
    {
        var goodList = new List<PieceType>();

        for (var i = 0; i < 8; i++)
        {
            goodList.Add(PieceType.Pawn);
        }
        goodList.Add(PieceType.Knight);
        goodList.Add(PieceType.Bishop);
        goodList.Add(PieceType.Queen);
        goodList.Add(PieceType.King);
        goodList.Add(PieceType.Rook);
        goodList.Add(PieceType.Rook);
        goodList.Add(PieceType.Knight);
        goodList.Add(PieceType.Bishop);
        
        var goodValidationResult = StartingPositionValidator
            .ValidateStartingPosition(goodList);
        
        Assert.True(goodValidationResult);
    }
    
    [Fact]
    public void IncorrectTotalPieceCount()
    {
        var badList = new List<PieceType>();

        for (var i = 0; i < 8; i++)
        {
            badList.Add(PieceType.Pawn);
        }
        
        var badValidationResult = StartingPositionValidator
            .ValidateStartingPosition(badList);
        
        Assert.False(badValidationResult);
    }
    
    [Fact]
    public void NotEveryPieceIsUsed()
    {
        var badList = new List<PieceType>();

        for (var i = 0; i < 14; i++)
        {
            badList.Add(PieceType.Pawn);
        }
        badList.Add(PieceType.Knight);
        badList.Add(PieceType.Bishop);
        badList.Add(PieceType.Queen);
        badList.Add(PieceType.King);
        
        var badValidationResult = StartingPositionValidator
            .ValidateStartingPosition(badList);
        
        Assert.False(badValidationResult);
    }
    
    [Fact]
    public void InvalidKingInFirstRank()
    {
        var badList = new List<PieceType>();

        badList.Add(PieceType.King);
        for (var i = 0; i < 8; i++)
        {
            badList.Add(PieceType.Pawn);
        }
        badList.Add(PieceType.Knight);
        badList.Add(PieceType.Bishop);
        badList.Add(PieceType.Queen);
        badList.Add(PieceType.Knight);
        badList.Add(PieceType.Bishop);
        badList.Add(PieceType.Rook);
        badList.Add(PieceType.Rook);
        
        var badValidationResult = StartingPositionValidator
            .ValidateStartingPosition(badList);
        
        Assert.False(badValidationResult);
    }
    
    [Fact]
    public void RandomizationFunctionIsValid()
    {
        for (var i = 0; i < 10; i++)
        {
            var list = StartingPositionValidator.RandomClashSetup();
            var result = StartingPositionValidator.ValidateStartingPosition(list);
        
            Assert.True(result);
        }
    }

    [Fact]
    public void ClashBoardUsesOneColorPerSideAndPreservesPieceSetups()
    {
        var whitePieces = StandardSetup();
        var blackPieces = StandardSetup();
        var whiteSetup = whitePieces.ToArray();
        var blackSetup = blackPieces.ToArray();

        var board = Board.CreateInitialClashBoard(whitePieces, blackPieces).CopySquares();

        Assert.Equal(whiteSetup, whitePieces);
        Assert.Equal(blackSetup, blackPieces);

        for (var row = 0; row < 8; row++)
        {
            for (var column = 0; column < 8; column++)
            {
                var piece = board[row, column].Piece;
                if (row is 0 or 1)
                {
                    Assert.NotNull(piece);
                    Assert.Equal(chess.Color.Black, piece.Color);
                }
                else if (row is 6 or 7)
                {
                    Assert.NotNull(piece);
                    Assert.Equal(chess.Color.White, piece.Color);
                }
                else
                {
                    Assert.Null(piece);
                }
            }
        }
    }

    private static List<PieceType> StandardSetup() =>
    [
        PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn,
        PieceType.Pawn, PieceType.Pawn, PieceType.Pawn, PieceType.Pawn,
        PieceType.King, PieceType.Rook, PieceType.Knight, PieceType.Bishop,
        PieceType.Queen, PieceType.Bishop, PieceType.Knight, PieceType.Rook
    ];
}