using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Strategy.Async;

public interface IFailStrategyAsync
{
    public Task<RequestResult?> HandleExceptionAsync(LLMRequestAsync request, Exception e);
}

public abstract class FailStrategyAsync<TException> : IFailStrategyAsync where TException : Exception
{
    public async Task<RequestResult?> HandleExceptionAsync(LLMRequestAsync request, Exception e)
    {
        if (e is TException typedException)
        {
            request.RequestConfig.Logger.Information("{failstrategyname} handling the Exception {exceptionname}.", GetType().Name, e.GetType().Name);
            return await ExecuteStrategy(request, typedException);
        }
        request.RequestConfig.Logger.Debug("{failstrategyname} not handling the Exception {exceptionname} - Not of the specified type", GetType().Name, e.GetType().Name);
        return null;
    }

    public abstract Task<RequestResult> ExecuteStrategy(LLMRequestAsync request, TException e);
}