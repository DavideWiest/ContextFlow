namespace ContextFlow.Domain;

using ContextFlow.Infrastructure.Logging;

public abstract class LLMConnection
{

    protected abstract RequestResult CallAPI(string input, LLMConfig conf, CFLogger log);

    public RequestResult GetResponse(string input, LLMConfig conf, CFLogger log)
    {
        return CallAPI(input, conf, log);
    }
}
