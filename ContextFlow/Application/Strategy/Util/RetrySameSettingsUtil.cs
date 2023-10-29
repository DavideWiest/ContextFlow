using ContextFlow.Application.Strategy.Async;

namespace ContextFlow.Application.Strategy.Util;

internal class RetrySameSettingsUtil
{
    public static FailStrategy<TException> GetNextFailStrategy<TException>(string name, int RetryCount, int MaxRetries) where TException : Exception
    {
        return RetryCount < MaxRetries ?
        new FailStrategyRetrySameSettings<TException>(RetryCount + 1, MaxRetries)
                : new FailStrategyThrowException<TException>($"An exception has occured and was not handeled by the configured {name} because the retry-limit was reached");
    }

    public static FailStrategyAsync<TException> GetNextAsyncFailStrategy<TException>(string name, int RetryCount, int MaxRetries) where TException : Exception
    {
        return RetryCount < MaxRetries ?
        new FailStrategyRetrySameSettingsAsync<TException>(RetryCount + 1, MaxRetries)
                : new FailStrategyThrowExceptionAsync<TException>($"An exception has occured and was not handeled by the configured {name} because the retry-limit was reached");
    }
}
