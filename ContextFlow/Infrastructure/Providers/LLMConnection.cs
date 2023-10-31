using ContextFlow.Domain;

namespace ContextFlow.Infrastructure.Providers;

using ContextFlow.Application.Result;
using ContextFlow.Infrastructure.Logging;

/// <summary>
/// The connection that handles requests to the LLM
/// </summary>
public abstract class LLMConnection
{

    protected abstract RequestResult CallAPI(string input, LLMConfig conf, CFLogger log);

    public RequestResult GetResponse(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            return CallAPI(input, conf, log);
        }
        catch (Exception e)
        {
            log.Error("Failed to get the output from the LLM. Exception: {exceptionname}: {exceptionmessage}", e.GetType().Name, e.Message);
            throw new LLMConnectionException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }
}
