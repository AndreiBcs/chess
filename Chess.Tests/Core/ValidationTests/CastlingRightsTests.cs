using chess;
using chess.Board;
using chess.Game;
using chess.Moves;
using chess.Pieces.Types;
using chess.Validation.MoveValidation;

namespace Chess.Tests.Core.ValidationTests;

public class CastlingRightsTests
{
    [Theory]
    [InlineData(Color.White, 7, 6, 'K')]
    [InlineData(Color.White, 7, 2, 'Q')]
    [InlineData(Color.Black, 0, 6, 'k')]
    [InlineData(Color.Black, 0, 2, 'q')]
    public void CastlingRightsTest(
        Color color,
        int row,
        int kingColDestination,
        char castlingLetter)
    {
        var board = Board.CreateInitial()
            // clear pieces for king travel
            .WithoutPiece(new Position(7, 1))
            .WithoutPiece(new Position(7, 2))
            .WithoutPiece(new Position(7, 3))
            .WithoutPiece(new Position(7, 5))
            .WithoutPiece(new Position(7, 6))
            .WithoutPiece(new Position(0, 1))
            .WithoutPiece(new Position(0, 2))
            .WithoutPiece(new Position(0, 3))
            .WithoutPiece(new Position(0, 5))
            .WithoutPiece(new Position(0, 6));

        var castlingRights = CastlingRights.GetInitialCastlingRights().ToList();

        var expectedCastlingElement = castlingRights.Find(c => c.LetterId == castlingLetter);
        
        var snapshot = GameSnapshot.CreateCustomSnapshot(
            GameStatus.InProgress,
            board,
            color,
            castlingRights,
            null,
            null,
            null,
            null,
            null
        );
        
        var moveStatus = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(row, 4), new Position(row, kingColDestination)));
        
        Assert.Equal(
            new MoveStatus(MoveResult.Valid, IsCastling: true, CastlingRights: expectedCastlingElement), 
            moveStatus);
    }
    
    [Theory]
    [InlineData(Color.White, 7, 6)]
    [InlineData(Color.White, 7, 2)]
    [InlineData(Color.Black, 0, 6)]
    [InlineData(Color.Black, 0, 2)]
    public void CannotCastleThroughPiece(
        Color color,
        int row,
        int kingColDestination)
    {
        var board = Board.CreateInitial();

        var castlingRights = CastlingRights.GetInitialCastlingRights().ToList();

        var snapshot = GameSnapshot.CreateCustomSnapshot(
            GameStatus.InProgress,
            board,
            color,
            castlingRights,
            null,
            null,
            null,
            null,
            null
        );
        
        var moveStatus = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(row, 4), new Position(row, kingColDestination)));
        
        Assert.Equal(
            new MoveStatus(MoveResult.Invalid, InvalidMoveReason: "Invalid move for selected piece."), 
            moveStatus);
    }
    
    [Theory]
    [InlineData(Color.White, 7, 6)]
    [InlineData(Color.White, 7, 2)]
    [InlineData(Color.Black, 0, 6)]
    [InlineData(Color.Black, 0, 2)]
    public void CannotCastleThroughCheck(
        Color color,
        int row,
        int kingColDestination)
    {
        var board = Board.CreateInitial()
            // clear pieces for king travel
            .WithoutPiece(new Position(7, 1))
            .WithoutPiece(new Position(7, 2))
            .WithoutPiece(new Position(7, 3))
            .WithoutPiece(new Position(7, 5))
            .WithoutPiece(new Position(7, 6))
            .WithoutPiece(new Position(0, 1))
            .WithoutPiece(new Position(0, 2))
            .WithoutPiece(new Position(0, 3))
            .WithoutPiece(new Position(0, 5))
            .WithoutPiece(new Position(0, 6))
            // clear pieces so king is in check while castling
            .WithoutPiece(new Position(6, 4))
            .WithoutPiece(new Position(6, 3))
            .WithoutPiece(new Position(1, 4))
            .WithoutPiece(new Position(1, 3))
            // add attacking bishops at the edges
            .WithPiece(new Bishop(Color.Black), new Position(2, 0))
            .WithPiece(new Bishop(Color.Black), new Position(2, 7))
            .WithPiece(new Bishop(Color.White), new Position(5, 0))
            .WithPiece(new Bishop(Color.White), new Position(5, 7));

        var castlingRights = CastlingRights.GetInitialCastlingRights().ToList();

        var snapshot = GameSnapshot.CreateCustomSnapshot(
            GameStatus.InProgress,
            board,
            color,
            castlingRights,
            null,
            null,
            null,
            null,
            null
        );
        
        var moveStatus = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(row, 4), new Position(row, kingColDestination)));
        
        Assert.Equal(
            new MoveStatus(MoveResult.Invalid, InvalidMoveReason: "Invalid move for selected piece."), 
            moveStatus);
    }
    
    [Theory]
    [InlineData(Color.White, 7, 6)]
    [InlineData(Color.White, 7, 2)]
    [InlineData(Color.Black, 0, 6)]
    [InlineData(Color.Black, 0, 2)]
    public void CannotCastleWithNoRook(
        Color color,
        int row,
        int kingColDestination)
    {
        var board = Board.CreateInitial()
            // clear pieces for king travel
            .WithoutPiece(new Position(7, 1))
            .WithoutPiece(new Position(7, 2))
            .WithoutPiece(new Position(7, 3))
            .WithoutPiece(new Position(7, 5))
            .WithoutPiece(new Position(7, 6))
            .WithoutPiece(new Position(0, 1))
            .WithoutPiece(new Position(0, 2))
            .WithoutPiece(new Position(0, 3))
            .WithoutPiece(new Position(0, 5))
            .WithoutPiece(new Position(0, 6))
            // clear every rook
            .WithoutPiece(new Position(0, 0))
            .WithoutPiece(new Position(0, 7))
            .WithoutPiece(new Position(7, 7))
            .WithoutPiece(new Position(7, 0));

        var castlingRights = CastlingRights.GetInitialCastlingRights().ToList();

        var snapshot = GameSnapshot.CreateCustomSnapshot(
            GameStatus.InProgress,
            board,
            color,
            castlingRights,
            null,
            null,
            null,
            null,
            null
        );
        
        var moveStatus = MoveValidator.ValidateMove(
            snapshot,
            new Move(new Position(row, 4), new Position(row, kingColDestination)));
        
        Assert.Equal(
            new MoveStatus(MoveResult.Invalid, InvalidMoveReason: "Invalid move for selected piece."), 
            moveStatus);
    }
    
    [Theory]
    [InlineData(Color.White, 7, 6)]
    [InlineData(Color.White, 7, 2)]
    [InlineData(Color.Black, 0, 6)]
    [InlineData(Color.Black, 0, 2)]
    public void CannotCastleIfRookMoved(
        Color color,
        int row,
        int kingColDestination)
    {
        var board = Board.CreateInitial()
            // clear pieces for king travel
            .WithoutPiece(new Position(7, 1))
            .WithoutPiece(new Position(7, 2))
            .WithoutPiece(new Position(7, 3))
            .WithoutPiece(new Position(7, 5))
            .WithoutPiece(new Position(7, 6))
            .WithoutPiece(new Position(0, 1))
            .WithoutPiece(new Position(0, 2))
            .WithoutPiece(new Position(0, 3))
            .WithoutPiece(new Position(0, 5))
            .WithoutPiece(new Position(0, 6));

        var castlingRights = CastlingRights.GetInitialCastlingRights().ToList();

        var snapshot = GameSnapshot.CreateCustomSnapshot(
            GameStatus.InProgress,
            board,
            color,
            castlingRights,
            null,
            null,
            null,
            null,
            null
        );
        
        var rook1 = GameSnapshot.GetUpdatedGameSnapshot(
            snapshot, 
            new Move(new Position(0, 0), new Position(0, 1)),
            new MoveStatus(MoveResult.Valid));
        var rook1Back = GameSnapshot.GetUpdatedGameSnapshot(
            rook1, 
            new Move(new Position(0, 1), new Position(0, 0)),
            new MoveStatus(MoveResult.Valid));
        
        var rook2 = GameSnapshot.GetUpdatedGameSnapshot(
            rook1Back, 
            new Move(new Position(0, 7), new Position(0, 6)),
            new MoveStatus(MoveResult.Valid));
        var rook2Back = GameSnapshot.GetUpdatedGameSnapshot(
            rook2, 
            new Move(new Position(0, 6), new Position(0, 7)),
            new MoveStatus(MoveResult.Valid));
        
        var rook3 = GameSnapshot.GetUpdatedGameSnapshot(
            rook2Back, 
            new Move(new Position(7, 0), new Position(7, 1)),
            new MoveStatus(MoveResult.Valid));
        var rook3Back = GameSnapshot.GetUpdatedGameSnapshot(
            rook3, 
            new Move(new Position(7, 1), new Position(7, 0)),
            new MoveStatus(MoveResult.Valid));
        
        var rook4 = GameSnapshot.GetUpdatedGameSnapshot(
            rook3Back, 
            new Move(new Position(7, 7), new Position(7, 6)),
            new MoveStatus(MoveResult.Valid));
        var rook4Back = GameSnapshot.GetUpdatedGameSnapshot(
            rook4, 
            new Move(new Position(7, 6), new Position(7, 7)),
            new MoveStatus(MoveResult.Valid));
        
        
        var moveStatus = MoveValidator.ValidateMove(
            rook4Back,
            new Move(new Position(row, 4), new Position(row, kingColDestination)));
        
        Assert.Equal(
            new MoveStatus(MoveResult.Invalid, InvalidMoveReason: "Invalid move for selected piece."), 
            moveStatus);
    }
    
    [Theory]
    [InlineData(Color.White, 7, 6)]
    [InlineData(Color.White, 7, 2)]
    [InlineData(Color.Black, 0, 6)]
    [InlineData(Color.Black, 0, 2)]
    public void CannotCastleIfKingMoved(
        Color color,
        int row,
        int kingColDestination)
    {
        var board = Board.CreateInitial()
            // clear pieces for king travel
            .WithoutPiece(new Position(7, 1))
            .WithoutPiece(new Position(7, 2))
            .WithoutPiece(new Position(7, 3))
            .WithoutPiece(new Position(7, 5))
            .WithoutPiece(new Position(7, 6))
            .WithoutPiece(new Position(0, 1))
            .WithoutPiece(new Position(0, 2))
            .WithoutPiece(new Position(0, 3))
            .WithoutPiece(new Position(0, 5))
            .WithoutPiece(new Position(0, 6));

        var castlingRights = CastlingRights.GetInitialCastlingRights().ToList();

        var snapshot = GameSnapshot.CreateCustomSnapshot(
            GameStatus.InProgress,
            board,
            color,
            castlingRights,
            null,
            null,
            null,
            null,
            null
        );
        
        var blackKingMoves = GameSnapshot.GetUpdatedGameSnapshot(
            snapshot, 
            new Move(new Position(0, 4), new Position(0, 3)),
            new MoveStatus(MoveResult.Valid));
        var blackKingMovesBack = GameSnapshot.GetUpdatedGameSnapshot(
            blackKingMoves, 
            new Move(new Position(0, 3), new Position(0, 4)),
            new MoveStatus(MoveResult.Valid));
        
        var whiteKingMoves = GameSnapshot.GetUpdatedGameSnapshot(
            blackKingMovesBack, 
            new Move(new Position(7, 4), new Position(7, 3)),
            new MoveStatus(MoveResult.Valid));
        var whiteKingMovesBack = GameSnapshot.GetUpdatedGameSnapshot(
            whiteKingMoves, 
            new Move(new Position(7, 3), new Position(7, 4)),
            new MoveStatus(MoveResult.Valid));
        
        var moveStatus = MoveValidator.ValidateMove(
            whiteKingMovesBack,
            new Move(new Position(row, 4), new Position(row, kingColDestination)));
        
        Assert.Equal(
            new MoveStatus(MoveResult.Invalid, InvalidMoveReason: "Invalid move for selected piece."), 
            moveStatus);
    }
}