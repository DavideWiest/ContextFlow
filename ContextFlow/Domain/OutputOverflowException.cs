namespace ContextFlow.Domain;

/// <summary>
/// Signals that the output was cut off by the token limit.
/// Thrown if the finishreason of a result is FinishReason.Overflow and the respective option is set to true in the RequestConfig
/// </summary>
public class OutputOverflowException : Exception
{
    public OutputOverflowException(string message) : base(message)
    {

    }
}
