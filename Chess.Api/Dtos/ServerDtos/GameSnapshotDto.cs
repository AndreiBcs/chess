using chess;
using chess.Board;
using chess.Game;

namespace Chess.Api.Dtos.ServerDtos;

public readonly record struct GameSnapshotDto
{
    public Square[][] BoardSquares { get; init; }
    public GameStatus Status { get; init; }
    public Color CurrentTurn { get; init; }
    
    public static GameSnapshotDto ToSnapshotDto(GameSnapshot snapshot)
    {
        var squares = snapshot.Board.CopySquares();
        var squaresDto = new Square[8][];

        for (var i = 0; i < 8; i++)
        {
            squaresDto[i] = new Square[8];
            for (var j = 0; j < 8; j++)
            {
                squaresDto[i][j] = squares[i, j];
            }
        }
        
        var snapshotDto = new GameSnapshotDto
        {
            CurrentTurn = snapshot.CurrentTurn,
            Status = snapshot.Status,
            BoardSquares = squaresDto
        };
        
        return snapshotDto;
    }
}