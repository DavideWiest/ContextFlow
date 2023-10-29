using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Domain;

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

    public override async Task<RequestResultAsync> ExecuteStrategy(LLMRequestAsync request, TException e)
    {
        request.RequestConfig.Logger.Debug($"{GetType().Name} executing its strategy (Retry-count={RetryCount})");

        FailStrategyAsync<TException> nextFailStrategy = RetryCount < MaxRetries ?
                new FailStrategyRetrySameSettingsAsync<TException>(RetryCount + 1, MaxRetries)
                : new FailStrategyThrowExceptionAsync<TException>($"An exception has occured and was not handeled by the configured {GetType().Name} because the retry-limit was reached");

        return await new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.AddFailStrategyToTop(
                nextFailStrategy
            ))
            .BuildAsync().Complete();
    }
}

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

    public override async Task<RequestResultAsync> ExecuteStrategy(LLMRequestAsync request, TException e)
    {
        request.RequestConfig.Logger.Debug($"{GetType().Name} executing its strategy (Retry-count={RetryCount})");

        FailStrategyAsync<TException> nextFailStrategy = RetryCount < MaxRetries - 1 ?
                new FailStrategyRetryNewSettingsAsync<TException>(RetryCount + 1, MaxRetries, LLMConf, RequestConf, Prompt)
                : new FailStrategyThrowExceptionAsync<TException>($"An exception has occured and was not handeled by the configured {GetType().Name} because the retry-limit was reached");

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

public class FailStrategyThrowExceptionAsync<TException> : FailStrategyAsync<TException> where TException : Exception
{
    private readonly string? InfoMessage = null;

    public FailStrategyThrowExceptionAsync() { }
    public FailStrategyThrowExceptionAsync(string? infoMessage)
    {
        InfoMessage = infoMessage;
    }

    public override async Task<RequestResultAsync> ExecuteStrategy(LLMRequestAsync request, TException e)
    {
        if (InfoMessage != null)
        {
            request.RequestConfig.Logger.Information(InfoMessage);
        }
        throw e;
    }
}