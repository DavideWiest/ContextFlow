namespace ContextFlow.Domain;

/// <summary>
/// A generic exception that represents an error on the connection-side.
/// </summary>
public class LLMConnectionException : Exception
{
    public LLMConnectionException(string message) : base(message)
    {

    }
}
