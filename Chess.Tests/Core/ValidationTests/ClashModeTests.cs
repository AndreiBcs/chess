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
}