using ContextFlow.Application.Prompting;
using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using OpenAI_API.Moderation;

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

    public async Task<RequestResult> Complete()
    {
        RequestConfig.Logger.Debug("\n--- RAW PROMPT ---\n" + "{rawprompt}" + "\n--- RAW PROMPT ---\n", Prompt.ToPlainText());


        RequestResult result;

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
            result = await UseFailStrategiesWrapperAsync(e);
        }

        RequestConfig.Logger.Debug("\n--- RAW OUTPUT ---\n" + "{rawoutput}" + "\n--- RAW OUTPUT ---\n", result.RawOutput);

        return result;
    }

    private async Task<RequestResult?> TryLoadResult()
    {
        if (RequestConfig.RequestLoaderAsync != null)
        {
            return await RequestConfig.RequestLoaderAsync.LoadMatchIfExistsAsync(this);
        }
        return null;
    }

    private async Task<RequestResult> GetResultFromLLMAsync()
    {
        RequestResult result;
        
        result = await LLMConnection.GetResponseAsync(Prompt.ToPlainText(), LLMConfig, RequestConfig.Logger);

        if (RequestConfig.ThrowExceptionOnOutputOverflow && result.FinishReason == FinishReason.Overflow)
            throw new OutputOverflowException("An overflow occured - The LLM was not able to finish its output [ThrowExceptionOnOutputOverflow=true]");
        
        
        return result;
    }

    private async Task<RequestResult> UseFailStrategiesWrapperAsync(Exception e)
    {
        RequestConfig.Logger.Error("Caught Error {exceptionname} when trying to get response: {exceptionmsg}", nameof(e), e.Message);
        RequestResult? result = await UseFailStrategiesAsync(e);
        if (result == null)
        {
            RequestConfig.Logger.Error("Configured fail-strategies were unable to handle the exception");
            throw e;
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
