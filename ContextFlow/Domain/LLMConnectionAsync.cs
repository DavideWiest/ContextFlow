namespace ContextFlow.Domain;

using ContextFlow.Infrastructure.Logging;
public abstract class LLMConnectionAsync
{

    protected abstract Task<PartialRequestResult> CallAPIAsync(string input, LLMConfig conf, CFLogger log);

    public async Task<PartialRequestResult> GetResponseAsync(string input, LLMConfig conf, CFLogger log)
    {
        return await CallAPIAsync(input, conf, log);
    }
}
