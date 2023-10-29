using ContextFlow.Application.Result;
using ContextFlow.Domain;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Models;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

internal static class OpenAIUtil
{
    public static RequestResult ChatResultToRequestResult(ChatResult result)
    {
        string output = result.Choices[0].ToString();
        FinishReason finish = ToCFFinishReason(result.Choices[0].FinishReason);
        return new RequestResult(output, finish);
    }

    public static RequestResult CompletionResultToRequestResult(CompletionResult result)
    {
        string output = result.Completions[0].ToString();
        FinishReason finish = ToCFFinishReason(result.Completions[0].FinishReason);
        return new RequestResult(output, finish);
    }

    public static async Task<ChatResult> GetChatResult(OpenAIAPI api, string input, LLMConfig conf)
    {
        var chatRequest = new ChatRequest()
        {
            Model = new Model(conf.ModelName),
            Temperature = conf.Temperature,
            MaxTokens = conf.MaxTotalTokens,
            PresencePenalty = conf.PresencePenalty,
            FrequencyPenalty = conf.FrequencyPenalty,
            TopP = conf.TopP,
            NumChoicesPerMessage = conf.NumOutputs,
            Messages = GetChatMessages(input, conf)
        };
        return await api.Chat.CreateChatCompletionAsync(chatRequest);
    }

    private static ChatMessage[] GetChatMessages(string input, LLMConfig conf)
    {
        return new ChatMessage[]
            {
                new ChatMessage(ChatMessageRole.System, conf.SystemMessage),
                new ChatMessage(ChatMessageRole.User, input)
            };
    }

    public static async Task<CompletionResult> GetCompletionResult(OpenAIAPI api, string input, LLMConfig conf)
    {
        var chatRequest = new CompletionRequest(
            input,
            new Model(conf.ModelName),
            conf.MaxTotalTokens,
            conf.Temperature,
            null,
            conf.TopP,
            conf.NumOutputs,
            conf.PresencePenalty,
            conf.FrequencyPenalty
        );
        return await api.Completions.CreateCompletionAsync(chatRequest);
    }

    public static FinishReason ToCFFinishReason(string finishReasonResponse)
    {
        return finishReasonResponse switch
        {
            "stop" => FinishReason.Stop,
            "length" => FinishReason.Overflow,
            _ => FinishReason.Unknown,
        };
    }
}
