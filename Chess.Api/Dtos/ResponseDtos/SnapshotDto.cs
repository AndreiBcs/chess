using chess;
using Chess.Api.Dtos.RequestDtos;
using chess.Board;
using chess.Game;

namespace Chess.Api.Dtos.ResponseDtos;

public readonly record struct SnapshotDto
{
    public ResponseDtoType Type { get; init; } 
    public Square[][] BoardSquares { get; init; }
    public GameStatus Status { get; init; }
    public Color CurrentTurn { get; init; }
    
    public static SnapshotDto ToSnapshotDto(GameSnapshot snapshot)
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
        
        var snapshotDto = new SnapshotDto
        {
            Type = ResponseDtoType.Snapshot,
            CurrentTurn = snapshot.CurrentTurn,
            Status = snapshot.Status,
            BoardSquares = squaresDto
        };
        
        return snapshotDto;
    }
}