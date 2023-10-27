using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Application.Request;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

public class OpenAICompletionConnection : LLMConnection
{
    OpenAIAPI api;

    public OpenAICompletionConnection(string apiKey)
    {
        api = new(apiKey);
    }

    public OpenAICompletionConnection()
    {
        // uses default, env ("OPENAI_API_KEY"), or config file
        api = new();
    }

    protected override RequestResult CallAPI(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            var result = OpenAIUtil.GetCompletionResult(api, input, conf, log).GetAwaiter().GetResult();
            return OpenAIUtil.CompletionResultToRequestResult(result);
        }
        catch (Exception e)
        {
            log.Error($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }
}
