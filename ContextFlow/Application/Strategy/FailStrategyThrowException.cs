using ContextFlow.Application.Request;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Strategy;

public class FailStrategyThrowException<TException> : FailStrategy<TException> where TException : Exception
{
    private readonly string? InfoMessage = null;

    public FailStrategyThrowException() { }
    public FailStrategyThrowException(string? infoMessage)
    {
        InfoMessage = infoMessage;
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, TException e)
    {
        if (InfoMessage != null)
        {
            request.RequestConfig.Logger.Information(InfoMessage);
        }
        throw e;
    }
}