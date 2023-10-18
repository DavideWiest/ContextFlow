using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Domain;
using Serilog.Core;

public class OpenAIChatConnectionAsync : LLMConnectionAsync
{
    OpenAIAPI api = default!;

    public OpenAIChatConnectionAsync(string apiKey)
    {
        api = new(apiKey);
    }

    public OpenAIChatConnectionAsync()
    {
        // tries to use the environment variable OPENAI_API_KEY
        api = new();
    }

    // should not be public 
    protected override async Task<string?> CallAPIAsync(string input, LLMConfig conf, Logger log)
    {
        var chat = api.Chat.CreateConversation();

        chat.AppendSystemMessage(conf.SystemMessage);
        chat.AppendUserInput(input);
        var output = await chat.GetResponseFromChatbotAsync();

        return output;
    }
}
