using ContextFlow.Application.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContextFlow.Application.Request.Async;
namespace ContextFlow.Application.Strategy.Async;

public interface IFailStrategyAsync
{
    public Task<RequestResultAsync?> HandleExceptionAsync(LLMRequestAsync request, Exception e);
}

public abstract class FailStrategyAsync<TException> : IFailStrategyAsync where TException : Exception
{
    public async Task<RequestResultAsync?> HandleExceptionAsync(LLMRequestAsync request, Exception e)
    {
        if (e is TException typedException)
        {
            request.RequestConfig.Logger.Information($"{GetType().Name} handling the Exception {e.GetType().Name}.");
            return await ExecuteStrategy(request, typedException);
        }
        request.RequestConfig.Logger.Debug($"{GetType().Name} not handling the Exception {e.GetType().Name} - Not of the specified type");
        return null;
    }

    public abstract Task<RequestResultAsync> ExecuteStrategy(LLMRequestAsync request, TException e);
}
