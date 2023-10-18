namespace ContextFlow.Domain;

using Serilog.Core;

public abstract class LLMConnectionAsync
{

    protected abstract Task<string?> CallAPI(string input, LLMConfig conf, Logger log);

    public async Task<string?> GetResponse(string input, LLMConfig conf, Logger log)
    {
        string? response = null;
        try
        {
            response = await CallAPI(input, conf, log);
        } catch (Exception e)
        {
            log.Error(e.Message);
        }
        return response;
    }
}
