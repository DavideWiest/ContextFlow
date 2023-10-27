using ContextFlow.Application.Prompting;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application.Request.Async;

public class LLMRequestAsync : LLMRequestBase
{
    public LLMConnectionAsync LLMConnection { get; }
    public RequestConfigAsync RequestConfig { get; }

    public LLMRequestAsync(Prompt prompt, LLMConfig llmconf, LLMConnectionAsync llmcon, RequestConfigAsync requestConfig) : base(prompt, llmconf)
    {
        LLMConnection = llmcon;
        RequestConfig = requestConfig;
    }

    public async Task<RequestResult> CompleteAsync()
    {
        RequestConfig.Logger.Debug("\n--- RAW PROMPT ---\n" + Prompt.ToPlainText() + "\n--- RAW PROMPT ---\n");

        if (RequestConfig.ValidateNumInputTokensBeforeRequest)
        {
            RequestConfig.Tokenizer!.ValidateNumTokens(Prompt.ToPlainText(), LLMConfig.MaxInputTokens);
        }

        RequestResult? result = TryLoadResult();

        if (result == null)
        {
            result = await GetResultFromLLMAsync();
        }

        RequestConfig.Logger.Debug("\n--- RAW OUTPUT ---\n" + result.RawOutput + "\n--- RAW OUTPUT ---\n");

        return result;
    }

    private RequestResult? TryLoadResult()
    {
        if (RequestConfig.RequestLoaderAsync != null)
        {
            return RequestConfig.RequestLoaderAsync.LoadMatchIfExistsAsync(this);
        }
        return null;
    }

    private async Task<RequestResult> GetResultFromLLMAsync()
    {
        RequestResult result;
        try
        {
            result = await LLMConnection.GetResponseAsync(Prompt.ToPlainText(), LLMConfig, RequestConfig.Logger);

            if (RequestConfig.RaiseExceptionOnOutputOverflow && result.FinishReason == FinishReason.Overflow)
                throw new OutputOverflowException("An overflow occured - The LLM was not able to finish its output [RaiseExceptionOnOutputOverflow=true]");
        }
        catch (Exception e)
        {
            RequestConfig.Logger.Error($"Caught Error {nameof(e)} when trying to get response: {e.Message}");
            RequestResult? possibleResult = await UseFailStrategiesAsync(e);
            if (possibleResult == null)
            {
                RequestConfig.Logger.Error("Configured fail-strategies were unable to handle the exception");
                throw;
            }
            result = possibleResult;
        }
        return result;
    }

    private async Task<RequestResult?> UseFailStrategiesAsync(Exception e)
    {
        foreach (var strategy in RequestConfig.FailStrategies)
        {
            var result = await strategy.HandleExceptionAsync(this, e);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }
}
