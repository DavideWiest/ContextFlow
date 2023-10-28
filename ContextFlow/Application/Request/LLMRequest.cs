using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Application.Prompting;

namespace ContextFlow.Application.Request;

public class LLMRequest : LLMRequestBase
{
    public LLMConnection LLMConnection { get; }
    public RequestConfig RequestConfig { get; }

    public LLMRequest(Prompt prompt, LLMConfig llmconf, LLMConnection llmcon, RequestConfig requestconf) : base(prompt, llmconf)
    {
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
            result = TryLoadResult() ?? GetResultFromLLM();
        }
        catch (Exception e)
        {
            RequestConfig.Logger.Error($"Caught Error {nameof(e)} when trying to get response: {e.Message}");
            RequestResult? possibleResult = UseFailStrategies(e);
            if (possibleResult == null)
            {
                RequestConfig.Logger.Error("Configured fail-strategies were unable to handle the exception");
                throw;
            }
            result = possibleResult;
        }

        RequestConfig.Logger.Debug("\n--- RAW OUTPUT ---\n" + result.RawOutput + "\n--- RAW OUTPUT ---\n");

        return result;
    }

    private RequestResult? TryLoadResult()
    {
        if (RequestConfig.RequestLoader != null)
        {
            return RequestConfig.RequestLoader.LoadMatchIfExists(this);
        }
        return null;
    }

    private RequestResult GetResultFromLLM()
    {
        RequestResult result;
        
        result = LLMConnection.GetResponse(Prompt.ToPlainText(), LLMConfig, RequestConfig.Logger);

        if (RequestConfig.RaiseExceptionOnOutputOverflow && result.FinishReason == FinishReason.Overflow)
            throw new OutputOverflowException("An overflow occured - The LLM was not able to finish its output [RaiseExceptionOnOutputOverflow=true]");
        
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
