using ContextFlow.Application.Request;
using ContextFlow.Domain;
using ContextFlow.Application.Prompting;

namespace ContextFlow.Application;

public interface IFailStrategy
{
    public RequestResult? HandleException(LLMRequest request, Exception e);
}

public abstract class FailStrategy<TException> : IFailStrategy where TException : Exception
{
    public RequestResult? HandleException(LLMRequest request, Exception e)
    {
        if (e is TException typedException)
        {
            request.RequestConfig.Logger.Information($"{GetType().Name} handling the Exception {e.GetType().Name}.");
            return ExecuteStrategy(request, typedException);
        }
        request.RequestConfig.Logger.Debug($"{GetType().Name} not handling the Exception {e.GetType().Name} - Not of the specified type");
        return null;
    }

    public abstract RequestResult ExecuteStrategy(LLMRequest request, TException e);
}