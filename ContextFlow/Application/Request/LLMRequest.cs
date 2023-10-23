using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Application.Prompting;
using OpenAI_API.Moderation;
using ContextFlow.Application.State;

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

        RequestResult? result = TryLoadResult();

        if (result == null)
        {
            result = GetResultFromLLM();
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
        try
        {
            result = LLMConnection.GetResponse(Prompt.ToPlainText(), LLMConfig, RequestConfig.Logger);

            if (RequestConfig.RaiseExceptionOnOutputOverflow && result.FinishReason == FinishReason.Overflow)
                throw new OutputOverflowException("An overflow occured - The LLM was not able to finish its output [RaiseExceptionOnOutputOverflow=true]");
        }
        catch (Exception e)
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

    public LLMRequest UsingOutputLimitAttachment(double tokenToWordRatio, double marginOfSafetyMul)
    {
        if (tokenToWordRatio < 0) { throw new InvalidDataException("tokenToWordRatio must be positive");  }
        if (marginOfSafetyMul < 0 || marginOfSafetyMul > 1) { throw new InvalidDataException("marginOfSafetyMul must be between 0 and 1."); }

        int availableTokenSpace = LLMConfig.MaxTotalTokens - LLMConfig.MaxInputTokens;
        int availableWords = (int)Math.Round(availableTokenSpace / tokenToWordRatio * marginOfSafetyMul);
        Prompt.UsingAttachment("Output length", $"The output must be under {availableWords} words long");
        return this;
    }
}
