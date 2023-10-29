using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using ContextFlow.Application.Strategy.Util;
using ContextFlow.Domain;

namespace ContextFlow.Application.Strategy;

public class FailStrategyRetryNewSettings<TException> : FailStrategy<TException> where TException : Exception
{
    public LLMConfig? LLMConf { get; }
    public RequestConfig? RequestConf { get; }
    public Prompt? Prompt { get; }
    public int MaxRetries { get; }
    public int RetryCount { get; } = 1;

    public FailStrategyRetryNewSettings(int maxRetries = 1, LLMConfig? newLLMConf = null, RequestConfig? newRequestConf = null, Prompt? newPrompt = null)
    {
        LLMConf = newLLMConf;
        RequestConf = newRequestConf;
        Prompt = newPrompt;
        MaxRetries = maxRetries;
    }

    public FailStrategyRetryNewSettings(int retryCount, int maxRetries = 1, LLMConfig? newLLMConf = null, RequestConfig? newRequestConf = null, Prompt? newPrompt = null) : this(maxRetries, newLLMConf, newRequestConf, newPrompt)
    {
        RetryCount = retryCount;

        if (maxRetries < 0)
        {
            throw new InvalidDataException("MaxRetries must be at least 1");
        }
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, TException e)
    {
        RetryUtil.LogRetryMessage(request, GetType().Name, RetryCount);

        var nextFailStrategy = RetryNewSettingsUtil.GetNextFailStrategy<TException>(GetType().Name, RetryCount, MaxRetries, LLMConf, RequestConf, Prompt);

        return new LLMRequestBuilder(request)
            .UsingPrompt(Prompt ?? request.Prompt)
            .UsingLLMConfig(LLMConf ?? request.LLMConfig)
            .UsingRequestConfig((RequestConf ?? request.RequestConfig).AddFailStrategyToTop(
                nextFailStrategy
            ))
            .Build().Complete();
    }
}
