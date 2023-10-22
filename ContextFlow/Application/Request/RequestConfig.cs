using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application.Request;

public class RequestConfig
{
    public List<IFailStrategy> FailStrategies = new() { new FailStrategyThrowException() };
    //public OverflowStrategy OverflowStrategy = new OverflowStrategyThrowException();
    public CFLogger Logger = new CFSerilogLogger();

    public bool ValidateNumInputTokensBeforeRequest { get; private set; } = true;

    public LLMTokenizer? Tokenizer = null;

    public RequestConfig UsingFailStrategy(IFailStrategy failStrategy)
    {
        FailStrategies.Add(failStrategy);
        return this;
    }

    public RequestConfig UsingOverflowStrategy(OverflowStrategy overflowStrategy)
    {
        FailStrategies.Add(overflowStrategy);
        return this;
    }

    public RequestConfig ActivateCheckNumTokensBeforeRequest(LLMTokenizer tokenizer)
    {
        ValidateNumInputTokensBeforeRequest = true;
        Tokenizer = tokenizer;
        return this;
    }

    public RequestConfig DeactivateCheckNumTokensBeforeRequest()
    {
        ValidateNumInputTokensBeforeRequest = false;
        return this;
    }

    public RequestConfig UsingLogger(CFLogger log)
    {
        Logger = log;
        return this;
    }

}
