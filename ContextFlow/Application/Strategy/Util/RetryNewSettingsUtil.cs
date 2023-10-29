using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Request;
using ContextFlow.Application.Strategy.Async;
using ContextFlow.Domain;

namespace ContextFlow.Application.Strategy.Util;

public class RetryNewSettingsUtil
{
    public static FailStrategy<TException> GetNextFailStrategy<TException>(string name, int RetryCount, int MaxRetries, LLMConfig LLMConf, RequestConfig RequestConf, Prompt Prompt) where TException : Exception
    {
        return RetryCount < MaxRetries - 1 ?
        new FailStrategyRetryNewSettings<TException>(RetryCount + 1, MaxRetries, LLMConf, RequestConf, Prompt)
                : new FailStrategyThrowException<TException>($"An exception has occured and was not handeled by the configured {name} because the retry-limit was reached");
    }

    public static FailStrategyAsync<TException> GetNextAsyncFailStrategy<TException>(string name, int RetryCount, int MaxRetries, LLMConfig LLMConf, RequestConfigAsync RequestConf, Prompt Prompt) where TException : Exception
    {
        return RetryCount < MaxRetries - 1 ?
        new FailStrategyRetryNewSettingsAsync<TException>(RetryCount + 1, MaxRetries, LLMConf, RequestConf, Prompt)
                : new FailStrategyThrowExceptionAsync<TException>($"An exception has occured and was not handeled by the configured {name} because the retry-limit was reached");
    }
}
