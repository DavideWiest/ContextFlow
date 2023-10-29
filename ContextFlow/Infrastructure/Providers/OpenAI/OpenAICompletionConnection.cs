using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;
using OpenAI_API.Completions;
using Serilog;

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
            return OpenAIUtil.CompletionResultToRequestResult(GetCompletionResultSync(input, conf, log));
        }
        catch (Exception e)
        {
            log.Error($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }

    private CompletionResult GetCompletionResultSync(string input, LLMConfig conf, CFLogger log)
    {
        return OpenAIUtil.GetCompletionResult(api, input, conf, log).GetAwaiter().GetResult();
    }
}
