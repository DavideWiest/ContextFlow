using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Application.Prompting;
using ContextFlow.Application.Result;
using OpenAI_API.Moderation;

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
        RequestConfig.Logger.Debug("\n--- RAW PROMPT ---\n" + "{rawprompt}" + "\n--- RAW PROMPT ---\n", Prompt.ToPlainText());


        RequestResult result;

        try
        {
            if (RequestConfig.ValidateNumInputTokensBeforeRequest)
            {
                RequestConfig.Tokenizer!.ValidateNumTokens(Prompt.ToPlainText(), LLMConfig.MaxInputTokens);
            }

            result = TryLoadResult() ?? GetResultFromLLM();

            if (result.FinishReason == FinishReason.Overflow && RequestConfig.ThrowExceptionOnOutputOverflow)
            {
                throw new OutputOverflowException("Output-overflow occured [RequestConfig.ThrowExceptionOnOutputOverflow=true]");
            }
        }
        catch (Exception e)
        {
            result = UseFailStrategiesWrapper(e);
        }

        RequestConfig.Logger.Debug("\n--- RAW OUTPUT ---\n" + "{rawoutput}" + "\n--- RAW OUTPUT ---\n", result.RawOutput);

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

        if (RequestConfig.ThrowExceptionOnOutputOverflow && result.FinishReason == FinishReason.Overflow)
            throw new OutputOverflowException("An overflow occured - The LLM was not able to finish its output [ThrowExceptionOnOutputOverflow=true]");
        
        return result;
    }

    private RequestResult UseFailStrategiesWrapper(Exception e)
    {
        RequestConfig.Logger.Error("Caught Error {exceptionname} when trying to get response: {exceptionmsg}", nameof(e), e.Message);
        RequestResult? possibleResult = UseFailStrategies(e);
        if (possibleResult == null)
        {
            RequestConfig.Logger.Error("Configured fail-strategies were unable to handle the exception");
            throw e;
        }
        return possibleResult;
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
