using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI.Async;

using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;
using OpenAI_API.Completions;

public class OpenAICompletionConnectionAsync : LLMConnectionAsync
{
    readonly OpenAIAPI api;

    public OpenAICompletionConnectionAsync(string apiKey)
    {
        api = new(apiKey);
    }

    public OpenAICompletionConnectionAsync()
    {
        // uses default, env ("OPENAI_API_KEY"), or config file
        api = new();
    }

    protected override async Task<RequestResult> CallAPIAsync(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            return OpenAIUtil.CompletionResultToRequestResult(await GetCompletionResultAsync(input, conf));
        }
        catch (Exception e)
        {
            log.Error("Failed to get the output from the LLM. Exception: {exceptionname}: {exceptionmessage}", e.GetType().Name, e.Message);
            throw new LLMConnectionException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }

    private async Task<CompletionResult> GetCompletionResultAsync(string input, LLMConfig conf)
    {
        return await OpenAIUtil.GetCompletionResult(api, input, conf);
    }
}
