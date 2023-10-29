using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using ContextFlow.Application.Strategy.Util;


namespace ContextFlow.Application.Strategy;

public class FailStrategyRetrySameSettings<TException> : FailStrategy<TException> where TException : Exception
{
    public int MaxRetries { get; }
    public int RetryCount { get; } = 1;

    public FailStrategyRetrySameSettings(int maxRetries = 1)
    {
        MaxRetries = maxRetries;
    }

    public FailStrategyRetrySameSettings(int retryCount, int maxRetries = 1) : this(maxRetries)
    {
        RetryCount = retryCount;
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, TException e)
    {
        RetryUtil.LogRetryMessage(request, GetType().Name, RetryCount);

        var nextFailStrategy = RetrySameSettingsUtil.GetNextFailStrategy<TException>(GetType().Name, RetryCount, MaxRetries);

        return new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.AddFailStrategyToTop(
                nextFailStrategy
            ))
            .Build().Complete();
    }
}
