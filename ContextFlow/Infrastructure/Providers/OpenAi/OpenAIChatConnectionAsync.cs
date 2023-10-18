using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
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
    protected override async Task<PartialRequestResult> CallAPIAsync(string input, LLMConfig conf, CFLogger log)
    {
        var chat = api.Chat.CreateConversation();

        chat.AppendSystemMessage(conf.SystemMessage);
        chat.AppendUserInput(input);
        var output = await chat.GetResponseFromChatbotAsync();

        return new PartialRequestResult(output, FinishReason.Stop);
    }
}
