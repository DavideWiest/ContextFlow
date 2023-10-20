using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Infrastructure.Providers.OpenAi;

internal class OpenAIChatUtil
{
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

    protected FinishReason ToCFFinishReason(string finishReasonResponse)
    {
        switch (finishReasonResponse)
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
