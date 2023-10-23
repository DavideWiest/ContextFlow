
namespace ContextFlow.Domain;

public class InputOverflowException : Exception
{
    public InputOverflowException(string message) : base(message)
    {

    }
}
