using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI.Async;

using ContextFlow.Domain;
using ContextFlow.Application.Result;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;
using OpenAI_API.Chat;
using Serilog;

public class OpenAIChatConnectionAsync : LLMConnectionAsync
{
    OpenAIAPI api;

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
            return OpenAIUtil.ChatResultToRequestResult(await GetChatResult(input, conf, log));
        }
        catch (Exception e)
        {
            log.Error($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }

    private async Task<ChatResult> GetChatResult(string input, LLMConfig conf, CFLogger log)
    {
        return await OpenAIUtil.GetChatResult(api, input, conf, log);
    }
}
