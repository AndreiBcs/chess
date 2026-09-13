namespace Chess.Engine;

public sealed class EngineException : Exception
{
    public EngineException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}