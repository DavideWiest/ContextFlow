using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Application.Prompting;
using OpenAI_API.Moderation;

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

        RequestResult result;
        try
        {
            result = LLMConnection.GetResponse(Prompt.ToPlainText(), LLMConfig, RequestConfig.Logger);
        } catch (Exception e)
        {
            RequestConfig.Logger.Error($"Caught Error {nameof(e)}: {e.Message}");
            RequestResult? possibleResult = UseFailStrategies(e);
            if (possibleResult == null)
            {
                RequestConfig.Logger.Information("Configured fail-strategies were unable to handle the exception");
                throw;
            }
            result = possibleResult;
        }


        RequestConfig.Logger.Debug("\n--- RAW OUTPUT ---\n" + result.RawOutput + "\n--- RAW OUTPUT ---\n");

        return result;
    }

    private RequestResult? UseFailStrategies(Exception e)
    {
        foreach (var strategy in RequestConfig.FailStrategies)
        {
            var result = strategy.HandleException(this, e);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}
