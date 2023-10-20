using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;

public class OpenAICompletionConnection : LLMConnection
{
    OpenAIAPI api = default!;

    public OpenAICompletionConnection(string apiKey)
    {
        api = new(apiKey);
    }

    public OpenAICompletionConnection()
    {
        // tries to use the environment variable OPENAI_API_KEY
        api = new();
    }

    protected override RequestResult CallAPI(string input, LLMConfig conf, CFLogger log)
    {
        var chat = api.Chat.CreateChatCompletionAsync(new OpenAI_API.Chat.ChatRequest()
        {

        }).GetAwaiter().GetResult();

        chat.AppendSystemMessage(conf.SystemMessage);
        chat.AppendUserInput(input);

        try
        {

            return new RequestResult(output, FinishReason.Stop);
        }
        catch (Exception e)
        {
            log.Error($"Failed to get the output from the LLM. Exception: {e.GetType()}: {e.Message}");
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType()}: {e.Message}");
        }
    }
}
