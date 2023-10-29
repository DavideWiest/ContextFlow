using ContextFlow.Domain;
using ContextFlow.Application.Result;

namespace ContextFlow.Infrastructure.Providers;

using ContextFlow.Application.Request.Async;
using ContextFlow.Infrastructure.Logging;

public abstract class LLMConnectionAsync
{

    protected abstract Task<RequestResult> CallAPIAsync(string input, LLMConfig conf, CFLogger log);

    public async Task<RequestResult> GetResponseAsync(string input, LLMConfig conf, CFLogger log)
    {
        try { 
            return await CallAPIAsync(input, conf, log);
        }
        catch (Exception e)
        {
            log.Error("Failed to get the output from the LLM. Exception: {exceptionname}: {exceptionmessage}", e.GetType().Name, e.Message);
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }
}
