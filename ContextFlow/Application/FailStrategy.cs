using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ContextFlow.Domain;

namespace ContextFlow.Application;

public abstract class FailStrategy
{
    public abstract RequestResult ExecuteStrategy(LLMRequest request, LLMException e);
}

public class FailStrategyRetrySameSettings : FailStrategy
{
    public int MaxRetries { get; }
    public int RetryCount { get; } = 1;

    public FailStrategyRetrySameSettings(int maxRetries=3)
    {
        MaxRetries = maxRetries;
    }

    public FailStrategyRetrySameSettings(int retryCount, int maxRetries = 3) : this(maxRetries)
    {
        RetryCount = retryCount;
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, LLMException e)
    {
        return new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.UsingFailStrategy(
                RetryCount < MaxRetries ?
                new FailStrategyRetrySameSettings(RetryCount+1, MaxRetries)
                : new FailStrategyThrowException()
            ))
            .Build().Complete();
    }
}

public class FailStrategyRetryNewSettings : FailStrategy
{
    public LLMConfig? LLMConf { get; }
    public RequestConfig? RequestConf { get; }
    public Prompt? Prompt { get; }
    public int MaxRetries { get; }
    public int RetryCount { get; } = 1;

    public FailStrategyRetryNewSettings(int maxRetries = 3, LLMConfig? llmConf = null, RequestConfig? requestConf = null, Prompt? prompt = null)
    {
        LLMConf = llmConf;
        RequestConf = requestConf;
        Prompt = prompt;
        MaxRetries = maxRetries;
    }

    public FailStrategyRetryNewSettings(int retryCount, int maxRetries = 3, LLMConfig? llmConf = null, RequestConfig? requestConf = null, Prompt? prompt = null) : this(maxRetries, llmConf, requestConf, prompt)
    {
        RetryCount = retryCount;
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, LLMException e)
    {
        return new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.UsingFailStrategy(
                RetryCount < MaxRetries ?
                new FailStrategyRetryNewSettings(RetryCount + 1, MaxRetries, LLMConf, RequestConf, Prompt)
                : new FailStrategyThrowException()
            ))
            .Build().Complete();
    }
}

public class FailStrategyThrowException : FailStrategy
{
    public override RequestResult ExecuteStrategy(LLMRequest request, LLMException e)
    {
        throw e;
    }
}