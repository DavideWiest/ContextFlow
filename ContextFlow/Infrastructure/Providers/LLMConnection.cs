using ContextFlow.Domain;

namespace ContextFlow.Infrastructure.Providers;

using ContextFlow.Infrastructure.Logging;

public abstract class LLMConnection
{

    protected abstract RequestResult CallAPI(string input, LLMConfig conf, CFLogger log);

    public RequestResult GetResponse(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            return CallAPI(input, conf, log);
        } catch (Exception e)
        {
            log.Error($"Failed to get the output from the LLM. Exception: {e.GetType()}: {e.Message}");
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType()}: {e.Message}");
        }
    }
}
