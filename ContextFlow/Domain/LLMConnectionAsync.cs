namespace ContextFlow.Domain;

using ContextFlow.Infrastructure.Logging;
public abstract class LLMConnectionAsync
{

    protected abstract Task<RequestResult> CallAPIAsync(string input, LLMConfig conf, CFLogger log);

    public async Task<RequestResult> GetResponseAsync(string input, LLMConfig conf, CFLogger log)
    {
        return await CallAPIAsync(input, conf, log);
    }
}
