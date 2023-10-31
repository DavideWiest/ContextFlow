using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;
using OpenAI_API.Chat;

public class OpenAIChatConnection : LLMConnection
{
    readonly OpenAIAPI api;

    public OpenAIChatConnection(string apiKey)
    {
        api = new(apiKey);
    }

    public OpenAIChatConnection()
    {
        // uses default, env ("OPENAI_API_KEY"), or config file
        api = new();
    }

    protected override RequestResult CallAPI(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            return OpenAIUtil.ChatResultToRequestResult(GetChatResultSync(input, conf));
        }
        catch (Exception e)
        {
            log.Error("Failed to get the output from the LLM. Exception: {exceptionname}: {exceptionmessage}", e.GetType().Name, e.Message);
            throw new LLMConnectionException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }

    private ChatResult GetChatResultSync(string input, LLMConfig conf)
    {
        return OpenAIUtil.GetChatResult(api, input, conf).GetAwaiter().GetResult();
    }

}
