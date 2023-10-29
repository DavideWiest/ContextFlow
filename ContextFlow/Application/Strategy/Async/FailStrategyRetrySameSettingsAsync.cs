using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;
using ContextFlow.Application.Strategy.Util;

namespace ContextFlow.Application.Strategy.Async;

public class FailStrategyRetrySameSettingsAsync<TException> : FailStrategyAsync<TException> where TException : Exception
{
    public int MaxRetries { get; }
    public int RetryCount { get; } = 1;

    public FailStrategyRetrySameSettingsAsync(int maxRetries = 3)
    {
        MaxRetries = maxRetries;
    }

    public FailStrategyRetrySameSettingsAsync(int retryCount, int maxRetries = 3) : this(maxRetries)
    {
        RetryCount = retryCount;
    }

    public override async Task<RequestResult> ExecuteStrategy(LLMRequestAsync request, TException e)
    {
        RetryUtil.LogRetryMessage(request, GetType().Name, RetryCount);

        var nextFailStrategy = RetrySameSettingsUtil.GetNextAsyncFailStrategy<TException>(GetType().Name, RetryCount, MaxRetries);

        return await new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.AddFailStrategyToTop(
                nextFailStrategy
            ))
            .BuildAsync().Complete();
    }
}
