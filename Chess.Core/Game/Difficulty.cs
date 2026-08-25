namespace chess.Game;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public readonly record struct ChessEngineDifficulty(Difficulty Difficulty)
{
    public int GetDifficultyElo()
    {
        return Difficulty switch
        {
            Difficulty.Easy => 1000,
            Difficulty.Normal => 1500,
            Difficulty.Hard => 2000,
            _ => throw new ArgumentOutOfRangeException(nameof(Difficulty))
        };
    }
}