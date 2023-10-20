using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using Serilog.Core;
using SmartFormat.Core.Output;

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

    protected override RequestResult CallAPI(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            var result = GetChatResult(input, conf, log).GetAwaiter().GetResult();
            string output = result.Choices[0].ToString();
            FinishReason finish = toCFFinishReason(result.Choices[0].FinishReason);
            
            return new RequestResult(output, FinishReason.Stop);
        }
        catch (Exception e)
        {
            log.Error($"Failed to get the output from the LLM. Exception: {e.GetType()}: {e.Message}");
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType()}: {e.Message}");
        }
    }

    protected async Task<ChatResult> GetChatResult(string input, LLMConfig conf, CFLogger log)
    {
        var chatRequest = new ChatRequest()
        {
            Model = new Model(conf.ModelName),
            Temperature = conf.Temperature,
            MaxTokens = conf.MaxTotalTokens,
            Messages = new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.System, conf.SystemMessage),
                new ChatMessage(ChatMessageRole.User, input)
            }
        };
        return await api.Chat.CreateChatCompletionAsync(chatRequest);
    }

    protected FinishReason toCFFinishReason(string finishReasonResponse)
    {
        switch(finishReasonResponse)
        {
            case "stop":
                return FinishReason.Stop;
            case "length":
                return FinishReason.Overflow;
            default:
                return FinishReason.Unknown;
        }
    }
}
