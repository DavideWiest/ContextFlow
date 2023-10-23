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
            request.RequestConfig.Logger.Information($"{GetType()} handling the Exception {e.GetType()}.");
            return ExecuteStrategy(request, typedException);
        }
        request.RequestConfig.Logger.Debug($"{GetType()} not handling the Exception {e.GetType()} - Not of the specified type");
        return null;
    }

    public abstract RequestResult ExecuteStrategy(LLMRequest request, TException e);
}