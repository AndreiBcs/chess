namespace Chess.Tests.Core.ValidationTests;

public class PromotionTests
{
    [Fact]
    public void CapturePromotionMovesPawnAndReplacesCapturedPiece()
    {
        var board = chess.Board.Board.CreateInitial()
            .WithMove(new chess.Board.Position(6, 0), new chess.Board.Position(0, 0))
            .WithPromotion(
                new chess.Board.Position(0, 0),
                chess.Pieces.PieceType.Rook,
                chess.Color.White);

        Assert.Null(board.GetPiece(new chess.Board.Position(6, 0)));
        Assert.Equal(chess.Pieces.PieceType.Rook, board.GetPiece(new chess.Board.Position(0, 0))?.Type);
        Assert.Equal(chess.Color.White, board.GetPiece(new chess.Board.Position(0, 0))?.Color);
    }
}