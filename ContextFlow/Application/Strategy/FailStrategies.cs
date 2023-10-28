using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Strategy;

public class LLMFailStrategyRetrySameSettings<TException> : FailStrategy<TException> where TException : Exception
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

    public override RequestResult ExecuteStrategy(LLMRequest request, TException e)
    {
        request.RequestConfig.Logger.Debug($"{GetType().Name} executing its strategy (Retry-count={RetryCount})");

        FailStrategy<TException> nextFailStrategy = RetryCount < MaxRetries ?
                new LLMFailStrategyRetrySameSettings<TException>(RetryCount + 1, MaxRetries)
                : new LLMFailStrategyThrowException<TException>($"An exception has occured and was not handeled by the configured {GetType().Name} because the retry-limit was reached");

        return new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.UsingFailStrategy(
                nextFailStrategy
            ))
            .Build().Complete();
    }
}

public class LLMFailStrategyRetryNewSettings<TException> : FailStrategy<TException> where TException : Exception
{
    public LLMConfig? LLMConf { get; }
    public RequestConfig? RequestConf { get; }
    public Prompt? Prompt { get; }
    public int MaxRetries { get; }
    public int RetryCount { get; } = 1;

    public LLMFailStrategyRetryNewSettings(int maxRetries = 3, LLMConfig? newLLMConf = null, RequestConfig? newRequestConf = null, Prompt? newPrompt = null)
    {
        LLMConf = newLLMConf;
        RequestConf = newRequestConf;
        Prompt = newPrompt;
        MaxRetries = maxRetries;
    }

    public LLMFailStrategyRetryNewSettings(int retryCount, int maxRetries = 3, LLMConfig? newLLMConf = null, RequestConfig? newRequestConf = null, Prompt? newPrompt = null) : this(maxRetries, newLLMConf, newRequestConf, newPrompt)
    {
        RetryCount = retryCount;

        if (maxRetries < 0)
        {
            throw new InvalidDataException("MaxRetries must be at least 1");
        }
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, TException e)
    {
        request.RequestConfig.Logger.Debug($"{GetType().Name} executing its strategy (Retry-count={RetryCount})");

        FailStrategy<TException> nextFailStrategy = RetryCount < MaxRetries - 1 ?
                new LLMFailStrategyRetryNewSettings<TException>(RetryCount + 1, MaxRetries, LLMConf, RequestConf, Prompt)
                : new LLMFailStrategyThrowException<TException>($"An exception has occured and was not handeled by the configured {GetType().Name} because the retry-limit was reached");

        return new LLMRequestBuilder(request)
            .UsingPrompt(Prompt ?? request.Prompt)
            .UsingLLMConfig(LLMConf ?? request.LLMConfig)
            .UsingRequestConfig(RequestConf ?? request.RequestConfig)
            .UsingRequestConfig(request.RequestConfig.UsingFailStrategy(
                nextFailStrategy
            ))
            .Build().Complete();
    }
}

public class LLMFailStrategyThrowException<TException> : FailStrategy<TException> where TException : Exception
{
    private readonly string? InfoMessage = null;

    public LLMFailStrategyThrowException() { }
    public LLMFailStrategyThrowException(string? infoMessage)
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