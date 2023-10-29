using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;

namespace ContextFlow.Application.Strategy.Util;

internal class RetryUtil
{
    public static void LogRetryMessage(LLMRequest request, string name, int retryCount)
    {
        request.RequestConfig.Logger.Debug("{name} executing its strategy (Retry-count={retryCount})", name, retryCount);
    }
    public static void LogRetryMessage(LLMRequestAsync request, string name, int retryCount)
    {
        request.RequestConfig.Logger.Debug("{name} executing its strategy (Retry-count={retryCount})", name, retryCount);
    }
}
