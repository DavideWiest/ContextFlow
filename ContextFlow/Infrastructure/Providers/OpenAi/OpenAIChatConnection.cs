using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Domain;
using Serilog.Core;

public class OpenAIChatConnection : LLMConnection
{
    OpenAIAPI api = default!;

    public OpenAIChatConnection(string apiKey)
    {
        api = new(apiKey);
    }

    public OpenAIChatConnection()
    {
        // tries to use the environment variable OPENAI_API_KEY
        api = new();
    }

    // should not be public 
    protected override string? CallAPI(string input, LLMConfig conf, Logger log)
    {
        var chat = api.Chat.CreateConversation();

        chat.AppendSystemMessage(conf.SystemMessage);
        chat.AppendUserInput(input);
        var output = chat.GetResponseFromChatbotAsync().GetAwaiter().GetResult();

        return output;
    }
}
