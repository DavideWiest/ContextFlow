namespace ContextFlow.Domain;

using Serilog.Core;

public abstract class LLMConnection
{

    protected abstract string? CallAPI(string input, LLMConfig conf, Logger log);

    public string? GetResponse(string input, LLMConfig conf, Logger log)
    {
        string? response = null;
        try
        {
            response = CallAPI(input, conf, log);
        } catch (Exception e)
        {
            log.Error(e.Message);
        }
        return response;
    }
}
