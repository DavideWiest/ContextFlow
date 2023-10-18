using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
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

    protected override PartialRequestResult CallAPI(string input, LLMConfig conf, CFLogger log)
    {
        var chat = api.Chat.CreateConversation();

        chat.AppendSystemMessage(conf.SystemMessage);
        chat.AppendUserInput(input);
        var output = chat.GetResponseFromChatbotAsync().GetAwaiter().GetResult();

        return new PartialRequestResult(output, FinishReason.Stop);
    }
}
