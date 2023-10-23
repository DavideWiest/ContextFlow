namespace ContextFlow.Domain;

public class OutputOverflowException : Exception
{
    public OutputOverflowException(string message) : base(message)
    {

    }
}
