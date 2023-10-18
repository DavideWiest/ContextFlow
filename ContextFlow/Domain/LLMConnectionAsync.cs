namespace ContextFlow.Domain;

using Serilog.Core;

public abstract class LLMConnectionAsync
{

    protected abstract Task<string?> CallAPIAsync(string input, LLMConfig conf, Logger log);

    public async Task<string?> GetResponseAsync(string input, LLMConfig conf, Logger log)
    {
        string? response = null;
        try
        {
            response = await CallAPIAsync(input, conf, log);
        } catch (Exception e)
        {
            log.Error(e.Message);
        }
        return response;
    }
}
