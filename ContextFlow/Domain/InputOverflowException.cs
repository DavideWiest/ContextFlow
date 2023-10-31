
namespace ContextFlow.Domain;

/// <summary>
/// An exception triggered if the input exceeded the configured token limit
/// </summary>
public class InputOverflowException : Exception
{
    public InputOverflowException(string message) : base(message)
    {

    }
}
