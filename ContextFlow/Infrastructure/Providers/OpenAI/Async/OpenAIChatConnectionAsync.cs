using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI.Async;

using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;
using OpenAI_API.Chat;

public class OpenAIChatConnectionAsync : LLMConnectionAsync
{
    readonly OpenAIAPI api;

    public OpenAIChatConnectionAsync(string apiKey)
    {
        api = new(apiKey);
    }

    public OpenAIChatConnectionAsync()
    {
        // tries to use the environment variable OPENAI_API_KEY
        api = new();
    }

    protected override async Task<RequestResult> CallAPIAsync(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            return OpenAIUtil.ChatResultToRequestResult(await GetChatResult(input, conf));
        }
        catch (Exception e)
        {
            log.Error("Failed to get the output from the LLM. Exception: {exceptionname}: {exceptionmessage}", e.GetType().Name, e.Message);
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }

    private async Task<ChatResult> GetChatResult(string input, LLMConfig conf)
    {
        return await OpenAIUtil.GetChatResult(api, input, conf);
    }
}
