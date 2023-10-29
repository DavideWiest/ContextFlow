using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Application.Strategy.Util;

namespace ContextFlow.Application.Strategy.Async;

public class FailStrategyRetryNewSettingsAsync<TException> : FailStrategyAsync<TException> where TException : Exception
{
    public LLMConfig? LLMConf { get; }
    public RequestConfigAsync? RequestConf { get; }
    public Prompt? Prompt { get; }
    public int MaxRetries { get; }
    public int RetryCount { get; } = 1;

    public FailStrategyRetryNewSettingsAsync(int maxRetries = 3, LLMConfig? newLLMConf = null, RequestConfigAsync? newRequestConf = null, Prompt? newPrompt = null)
    {
        LLMConf = newLLMConf;
        RequestConf = newRequestConf;
        Prompt = newPrompt;
        MaxRetries = maxRetries;
    }

    public FailStrategyRetryNewSettingsAsync(int retryCount, int maxRetries = 3, LLMConfig? newLLMConf = null, RequestConfigAsync? newRequestConf = null, Prompt? newPrompt = null) : this(maxRetries, newLLMConf, newRequestConf, newPrompt)
    {
        RetryCount = retryCount;

        if (maxRetries < 0)
        {
            throw new InvalidDataException("MaxRetries must be at least 1");
        }
    }

    public override async Task<RequestResult> ExecuteStrategy(LLMRequestAsync request, TException e)
    {
        RetryUtil.LogRetryMessage(request, GetType().Name, RetryCount);

        var nextFailStrategy = RetryNewSettingsUtil.GetNextAsyncFailStrategy<TException>(GetType().Name, RetryCount, MaxRetries, LLMConf, RequestConf, Prompt);

        return await new LLMRequestBuilder(request)
            .UsingPrompt(Prompt ?? request.Prompt)
            .UsingLLMConfig(LLMConf ?? request.LLMConfig)
            .UsingRequestConfig(RequestConf ?? request.RequestConfig)
            .UsingRequestConfig(request.RequestConfig.AddFailStrategyToTop(
                nextFailStrategy
            ))
            .BuildAsync().Complete();
    }
}