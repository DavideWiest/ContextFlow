using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Strategy;

public class LLMFailStrategyRetrySameSettings : FailStrategy<LLMException>
{
    public int MaxRetries { get; }
    public int RetryCount { get; } = 1;

    public LLMFailStrategyRetrySameSettings(int maxRetries = 3)
    {
        MaxRetries = maxRetries;
    }

    public LLMFailStrategyRetrySameSettings(int retryCount, int maxRetries = 3) : this(maxRetries)
    {
        RetryCount = retryCount;
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, LLMException e)
    {
        request.RequestConfig.Logger.Debug($"{GetType().Name} executing its strategy (Retry-count={RetryCount})");

        FailStrategy<LLMException> nextFailStrategy = RetryCount < MaxRetries ?
                new LLMFailStrategyRetrySameSettings(RetryCount + 1, MaxRetries)
                : new LLMFailStrategyThrowException($"An exception has occured and was not handeled by the configured {GetType().Name} because the retry-limit was reached");

        return new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.UsingFailStrategy(
                nextFailStrategy
            ))
            .Build().Complete();
    }
}

public class LLMFailStrategyRetryNewSettings : FailStrategy<LLMException>
{
    public LLMConfig? LLMConf { get; }
    public RequestConfig? RequestConf { get; }
    public Prompt? Prompt { get; }
    public int MaxRetries { get; }
    public int RetryCount { get; } = 1;

    public LLMFailStrategyRetryNewSettings(int maxRetries = 3, LLMConfig? llmConf = null, RequestConfig? requestConf = null, Prompt? prompt = null)
    {
        LLMConf = llmConf;
        RequestConf = requestConf;
        Prompt = prompt;
        MaxRetries = maxRetries;
    }

    public LLMFailStrategyRetryNewSettings(int retryCount, int maxRetries = 3, LLMConfig? llmConf = null, RequestConfig? requestConf = null, Prompt? prompt = null) : this(maxRetries, llmConf, requestConf, prompt)
    {
        RetryCount = retryCount;

        if (maxRetries < 0)
        {
            throw new InvalidDataException("MaxRetries must be at least 1");
        }
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, LLMException e)
    {
        request.RequestConfig.Logger.Debug($"{GetType().Name} executing its strategy (Retry-count={RetryCount})");

        FailStrategy<LLMException> nextFailStrategy = RetryCount < MaxRetries - 1 ?
                new LLMFailStrategyRetryNewSettings(RetryCount + 1, MaxRetries, LLMConf, RequestConf, Prompt)
                : new LLMFailStrategyThrowException($"An exception has occured and was not handeled by the configured {GetType().Name} because the retry-limit was reached");

        return new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.UsingFailStrategy(
                nextFailStrategy
            ))
            .Build().Complete();
    }
}

public class LLMFailStrategyThrowException : FailStrategy<LLMException>
{
    private readonly string? InfoMessage = null;

    public LLMFailStrategyThrowException() { }
    public LLMFailStrategyThrowException(string? infoMessage)
    {
        InfoMessage = infoMessage;
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, LLMException e)
    {
        if (InfoMessage != null)
        {
            request.RequestConfig.Logger.Information(InfoMessage);
        }
        throw e;
    }
}