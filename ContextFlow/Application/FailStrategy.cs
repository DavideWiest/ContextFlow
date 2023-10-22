using ContextFlow.Application.Request;
using ContextFlow.Domain;
using ContextFlow.Application.Prompting;

namespace ContextFlow.Application;

public interface IFailStrategy
{
    public RequestResult? HandleException(LLMRequest request, Exception e);
}

public abstract class FailStrategy<TException> : IFailStrategy where TException : Exception
{
    public RequestResult? HandleException(LLMRequest request, Exception e)
    {
        if (e is TException typedException)
        {
            request.RequestConfig.Logger.Information($"{GetType()} handling the Exception {e.GetType()}.");
            return ExecuteStrategy(request, typedException);
        }
        request.RequestConfig.Logger.Debug($"{GetType()} not handling the Exception {e.GetType()} - Not of the specified type");
        return null;
    }

    public abstract RequestResult ExecuteStrategy(LLMRequest request, TException e);
}


public class FailStrategyRetrySameSettings : FailStrategy<LLMException>
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
        request.RequestConfig.Logger.Debug($"{GetType()} executing its strategy (Retry-count={RetryCount})");

        FailStrategy<LLMException> nextFailStrategy = RetryCount < MaxRetries ?
                new FailStrategyRetrySameSettings(RetryCount + 1, MaxRetries)
                : new FailStrategyThrowException($"An exception has occured and was not handeled by the configured {GetType()} because the retry-limit was reached");

        return new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.UsingFailStrategy(
                nextFailStrategy
            ))
            .Build().Complete();
    }
}

public class FailStrategyRetryNewSettings : FailStrategy<LLMException>
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

        if (maxRetries < 0)
        {
            throw new InvalidDataException("MaxRetries must be at least 1");
        }
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, LLMException e)
    {
        request.RequestConfig.Logger.Debug($"{GetType()} executing its strategy (Retry-count={RetryCount})");

        FailStrategy<LLMException> nextFailStrategy = RetryCount < MaxRetries - 1 ?
                new FailStrategyRetryNewSettings(RetryCount + 1, MaxRetries, LLMConf, RequestConf, Prompt)
                : new FailStrategyThrowException($"An exception has occured and was not handeled by the configured {GetType()} because the retry-limit was reached");

        return new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.UsingFailStrategy(
                nextFailStrategy
            ))
            .Build().Complete();
    }
}

public class FailStrategyThrowException : FailStrategy<LLMException>
{
    private readonly string? InfoMessage = null;

    public FailStrategyThrowException() { }
    public FailStrategyThrowException(string? infoMessage)
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