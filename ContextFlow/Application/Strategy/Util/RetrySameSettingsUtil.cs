using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Request;
using ContextFlow.Application.Strategy.Async;
using ContextFlow.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Strategy.Util;

internal class RetrySameSettingsUtil
{
    public static FailStrategy<TException> GetNextFailStrategy<TException>(string name, int RetryCount, int MaxRetries) where TException : Exception
    {
        return RetryCount < MaxRetries - 1 ?
        new FailStrategyRetrySameSettings<TException>(RetryCount + 1, MaxRetries)
                : new FailStrategyThrowException<TException>($"An exception has occured and was not handeled by the configured {name} because the retry-limit was reached");
    }

    public static FailStrategyAsync<TException> GetNextAsyncFailStrategy<TException>(string name, int RetryCount, int MaxRetries) where TException : Exception
    {
        return RetryCount < MaxRetries - 1 ?
        new FailStrategyRetrySameSettingsAsync<TException>(RetryCount + 1, MaxRetries)
                : new FailStrategyThrowExceptionAsync<TException>($"An exception has occured and was not handeled by the configured {name} because the retry-limit was reached");
    }
}
