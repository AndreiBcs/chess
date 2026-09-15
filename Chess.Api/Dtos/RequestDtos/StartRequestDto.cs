namespace Chess.Api.Dtos.RequestDtos;

public readonly record struct StartRequestDto(
    string PlayerColor,
    string EngineType,
    int Elo,
    string? GameId = null)
{
    public static void Validate(StartRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerColor))
        {
            throw new ArgumentException("Color is required.", nameof(PlayerColor));
        }

        if (string.IsNullOrWhiteSpace(request.EngineType))
        {
            throw new ArgumentException("Chess engine is required.", nameof(EngineType));
        }

        _ = request.PlayerColor.Trim().ToLowerInvariant() switch
        {
            "white" => 0,
            "black" => 0,
            _ => throw new ArgumentException("Color must be 'white' or 'black'.", nameof(PlayerColor))
        };

        _ = request.EngineType.Trim().ToLowerInvariant() switch
        {
            "stockfish" => 0,
            _ => throw new ArgumentException("Chess engine not supported", nameof(EngineType))
        };

        if (request.Elo <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Elo), "Elo must be greater than zero.");
        }
    }
}