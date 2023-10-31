using ContextFlow.Application.Result;
using ContextFlow.Domain;

namespace ContextFlow.Infrastructure.Providers;

using ContextFlow.Infrastructure.Logging;

/// <summary>
/// The asynchronous connection that handles requests to the LLM
/// </summary>
public abstract class LLMConnectionAsync
{

    protected abstract Task<RequestResult> CallAPIAsync(string input, LLMConfig conf, CFLogger log);

    public async Task<RequestResult> GetResponseAsync(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            return await CallAPIAsync(input, conf, log);
        }
        catch (Exception e)
        {
            log.Error("Failed to get the output from the LLM. Exception: {exceptionname}: {exceptionmessage}", e.GetType().Name, e.Message);
            throw new LLMConnectionException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }
}
