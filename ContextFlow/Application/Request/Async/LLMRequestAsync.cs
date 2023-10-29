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

    public async Task<RequestResultAsync> Complete()
    {
        RequestConfig.Logger.Debug("\n--- RAW PROMPT ---\n" + Prompt.ToPlainText() + "\n--- RAW PROMPT ---\n");


        RequestResultAsync result;

        try
        {
            if (RequestConfig.ValidateNumInputTokensBeforeRequest)
            {
                RequestConfig.Tokenizer!.ValidateNumTokens(Prompt.ToPlainText(), LLMConfig.MaxInputTokens);
            }

            result = await TryLoadResult() ?? await GetResultFromLLMAsync();

            if (result.FinishReason == FinishReason.Overflow && RequestConfig.ThrowExceptionOnOutputOverflow)
            {
                throw new OutputOverflowException("Output-overflow occured [RequestConfig.ThrowExceptionOnOutputOverflow=true]");
            }
        }
        catch (Exception e)
        {
            RequestConfig.Logger.Error($"Caught Error {nameof(e)} when trying to get response: {e.Message}");
            RequestResultAsync? possibleResult = await UseFailStrategiesAsync(e);
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

    private async Task<RequestResultAsync?> TryLoadResult()
    {
        if (RequestConfig.RequestLoaderAsync != null)
        {
            return await RequestConfig.RequestLoaderAsync.LoadMatchIfExistsAsync(this);
        }
        return null;
    }

    private async Task<RequestResultAsync> GetResultFromLLMAsync()
    {
        RequestResultAsync result;
        
        result = await LLMConnection.GetResponseAsync(Prompt.ToPlainText(), LLMConfig, RequestConfig.Logger);

        if (RequestConfig.ThrowExceptionOnOutputOverflow && result.FinishReason == FinishReason.Overflow)
            throw new OutputOverflowException("An overflow occured - The LLM was not able to finish its output [ThrowExceptionOnOutputOverflow=true]");
        
        
        return result;
    }

    private async Task<RequestResultAsync?> UseFailStrategiesAsync(Exception e)
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
