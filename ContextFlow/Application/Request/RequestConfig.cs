using ContextFlow.Application.Strategy;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application.Request;

public class RequestConfig
{
    public List<IFailStrategy> FailStrategies { get; private set; } = new() { new LLMFailStrategyThrowException() };
    //public OverflowStrategy OverflowStrategy = new InputOverflowStrategyThrowException();
    public CFLogger Logger { get; private set; }  = new CFSerilogLogger();

    public bool ValidateNumInputTokensBeforeRequest { get; private set; } = false;
    public bool RaiseExceptionOnOutputOverflow { get; set; } = false;

    public LLMTokenizer? Tokenizer { get; private set; } = null;

    public RequestConfig UsingFailStrategy(IFailStrategy failStrategy)
    {
        FailStrategies.Add(failStrategy);
        return this;
    }

    public RequestConfig UsingTokenizer(LLMTokenizer? tokenizer)
    {
        Tokenizer = tokenizer;
        return this;
    }

    public RequestConfig UsingOverflowStrategy(InputOverflowStrategy overflowStrategy)
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

    public RequestConfig UsingRaiseExceptionOnOutputOverflow(bool raiseExceptionOnOutputOverflow)
    {
        RaiseExceptionOnOutputOverflow = raiseExceptionOnOutputOverflow;
        return this;
    }

    public RequestConfig UsingLogger(CFLogger log)
    {
        Logger = log;
        return this;
    }

}
