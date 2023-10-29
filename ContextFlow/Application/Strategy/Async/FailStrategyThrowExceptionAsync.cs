using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Strategy.Async;

public class FailStrategyThrowExceptionAsync<TException> : FailStrategyAsync<TException> where TException : Exception
{
    private readonly string? InfoMessage = null;

    public FailStrategyThrowExceptionAsync() { }
    public FailStrategyThrowExceptionAsync(string? infoMessage)
    {
        InfoMessage = infoMessage;
    }

    public override async Task<RequestResult> ExecuteStrategy(LLMRequestAsync request, TException e)
    {
        if (InfoMessage != null)
        {
            request.RequestConfig.Logger.Information(InfoMessage);
        }
        throw e;
    }
}