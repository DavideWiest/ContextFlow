using ContextFlow.Application.Request;
using ContextFlow.Application.Result;

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
            request.RequestConfig.Logger.Information("{failstrategyname} handling the Exception {exceptionname}.", GetType().Name, e.GetType().Name);
            return ExecuteStrategy(request, typedException);
        }
        request.RequestConfig.Logger.Debug("{failstrategyname} not handling the Exception {exceptionname} - Not of the specified type", GetType().Name, e.GetType().Name);
        return null;
    }

    public abstract RequestResult ExecuteStrategy(LLMRequest request, TException e);
}

