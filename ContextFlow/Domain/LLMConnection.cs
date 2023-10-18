namespace ContextFlow.Domain;

using ContextFlow.Infrastructure.Logging;

public abstract class LLMConnection
{

    protected abstract PartialRequestResult CallAPI(string input, LLMConfig conf, CFLogger log);

    public PartialRequestResult GetResponse(string input, LLMConfig conf, CFLogger log)
    {
        return CallAPI(input, conf, log);
    }
}
