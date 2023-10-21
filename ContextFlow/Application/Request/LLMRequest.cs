using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Application.Prompting;

namespace ContextFlow.Application.Request;

public class LLMRequest
{
    public Prompt Prompt { get; }
    public LLMConfig LLMConfig { get; }
    public LLMConnection LLMConnection { get; }
    public RequestConfig RequestConfig { get; }

    public LLMRequest(Prompt prompt, LLMConfig llmconf, LLMConnection llmcon, RequestConfig requestconf)
    {
        Prompt = prompt;
        LLMConfig = llmconf;
        LLMConnection = llmcon;
        RequestConfig = requestconf;
    }

    public RequestResult Complete()
    {
        RequestConfig.Logger.Debug("\n--- RAW PROMPT ---\n" + Prompt.ToPlainText() + "\n--- RAW PROMPT ---\n");

        if (RequestConfig.ValidateNumInputTokensBeforeRequest)
        {
            RequestConfig.Tokenizer!.ValidateNumTokens(Prompt.ToPlainText(), LLMConfig.MaxInputTokens);
        }

        RequestResult result = LLMConnection.GetResponse(Prompt.ToPlainText(), LLMConfig, RequestConfig.Logger);

        RequestConfig.Logger.Debug("\n--- RAW OUTPUT ---\n" + result.RawOutput + "\n--- RAW OUTPUT ---\n");

        return result;
    }
}
