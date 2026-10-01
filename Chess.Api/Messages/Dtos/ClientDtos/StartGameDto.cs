using System.Runtime.CompilerServices;
using chess;
using Chess.Engine;
using chess.Pieces;
using chess.Validation.ClashModeValidation;

namespace Chess.Api.Messages.Dtos.ClientDtos;

public abstract record StartGameDto(
    ChessGameMode GameMode)
{
    public abstract void Validate();
}

public sealed record NormalPvEStartDto(
    ChessGameMode GameMode,
    Color PlayerColor,
    ChessEngineType ChessEngineType,
    int Elo,
    int Depth,
    int MoveTime,
    long Nodes) : StartGameDto(GameMode)
{
    public override void Validate()
    {
        if (PlayerColor is not Color.White and not Color.Black)
        {
            throw new InvalidDataException("The selected color is invalid for chess.");
        }
        
        if (ChessEngineType is ChessEngineType.Deakfish)
        {
            throw new NotSupportedException("Deakfish chess engine is in development.");
        }
        
        if (Elo is < 1320 or > 3190)
        {
            throw new ArgumentOutOfRangeException(nameof(Elo), "Elo must be between 1320 and 3190.");
        }

        if (Depth is < 1 or > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(Depth), "Depth must be between 1 and 50.");
        }

        if (MoveTime is < 1 or > 600_000)
        {
            throw new ArgumentOutOfRangeException(nameof(MoveTime), "Move time must be between 1 and 600000 ms.");
        }

        if (Nodes is < 1 or > 1_000_000_000)
        {
            throw new ArgumentOutOfRangeException(nameof(Nodes), "Nodes must be between 1 and 1000000000.");
        }
    }
}

public sealed record ClashPvEStartDto(
    ChessGameMode GameMode,
    Color PlayerColor,
    ChessEngineType ChessEngineType,
    int Elo,
    int Depth,
    int MoveTime,
    long Nodes,
    bool RandomizeChessEnginePieces,
    ChessPiecesBuffer PlayerPieces,
    ChessPiecesBuffer? ChessEnginePieces) : StartGameDto(GameMode)
{
    public override void Validate()
    {
        if (PlayerColor is not Color.White and not Color.Black)
        {
            throw new InvalidDataException("The selected color is invalid for chess.");
        }
        
        if (ChessEngineType is ChessEngineType.Deakfish)
        {
            throw new NotSupportedException("Deakfish chess engine is in development.");
        }
        
        if (Elo is < 1320 or > 3190)
        {
            throw new ArgumentOutOfRangeException(nameof(Elo), "Elo must be between 1320 and 3190.");
        }

        if (Depth is < 1 or > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(Depth), "Depth must be between 1 and 50.");
        }

        if (MoveTime is < 1 or > 600_000)
        {
            throw new ArgumentOutOfRangeException(nameof(MoveTime), "Move time must be between 1 and 600000 ms.");
        }

        if (Nodes is < 1 or > 1_000_000_000)
        {
            throw new ArgumentOutOfRangeException(nameof(Nodes), "Nodes must be between 1 and 1000000000.");
        }

        if (!RandomizeChessEnginePieces && ChessEnginePieces is null ||
            RandomizeChessEnginePieces && ChessEnginePieces is not null)
        {
            throw new NotSupportedException("Chess engine pieces must be manually set or randomized.");
        }

        var playerPieces = new List<PieceType>();
        var enginePieces = new List<PieceType>();
        
        foreach (var piece in PlayerPieces)
        {
            playerPieces.Add(piece);
        }

        if (ChessEnginePieces != null)
        {
            foreach (var piece in ChessEnginePieces)
            {
                enginePieces.Add(piece);
            }
        }

        if(!StartingPositionValidator.ValidateStartingPosition(playerPieces))
        {
            throw new InvalidDataException("Player pieces are invalid." +
                                           " Either piece count doesn't match or King is in the first rank.");
        }
        
        if(!StartingPositionValidator.ValidateStartingPosition(enginePieces))
        {
            throw new InvalidDataException("Chess engine pieces are invalid." +
                                           " Either piece count doesn't match or King is in the first rank.");
        }
    }
}

public sealed record NormalPvPStartDto(
    ChessGameMode GameMode,
    string Nickname) : StartGameDto(GameMode)
{
    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Nickname.Trim()))
        {
            throw new InvalidDataException("Nickname cannot be empty.");
        }
    }
}

public sealed record ClashPvPStartDto(
    ChessGameMode GameMode,
    string Nickname,
    ChessPiecesBuffer PlayerPieces) : StartGameDto(GameMode)
{
    public override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Nickname.Trim()))
        {
            throw new InvalidDataException("Nickname cannot be empty.");
        }
        
        var playerPieces = new List<PieceType>();
        
        foreach (var piece in PlayerPieces)
        {
            playerPieces.Add(piece);
        }

        if(!StartingPositionValidator.ValidateStartingPosition(playerPieces))
        {
            throw new InvalidDataException("Player pieces are invalid." +
                                           " Either piece count doesn't match or King is in the first rank.");
        }
    }
}


[InlineArray(16)]
public struct ChessPiecesBuffer
{
    private PieceType _element0;
}