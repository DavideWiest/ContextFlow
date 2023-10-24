using ContextFlow.Application.Storage;
using ContextFlow.Application.Strategy;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application.Request;

public class RequestConfig
{
    public List<IFailStrategy> FailStrategies { get; private set; } = new() { new LLMFailStrategyThrowException() };
    public CFLogger Logger { get; private set; }  = new CFSerilogLogger();
    public RequestLoader? RequestLoader { get; private set; }
    public RequestSaver? RequestSaver { get; private set; }

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

    public RequestConfig UsingRequestSaver(RequestSaver requestSaver)
    {
        RequestSaver = requestSaver;
        return this;
    }

    public RequestConfig UsingRequestLoader(RequestLoader requestLoader)
    {
        RequestLoader = requestLoader;
        return this;
    }

    public override string ToString()
    {
        string failStrategies = string.Join(", ", FailStrategies.Select(fs => fs.GetType().Name));
        string requestLoader = RequestLoader != null ? RequestLoader.GetType().Name : "null";
        string requestSaver = RequestSaver != null ? RequestSaver.GetType().Name : "null";
        string tokenizer = Tokenizer != null ? Tokenizer.GetType().Name : "null";

        return $"RequestConfig(FailStrategies=[{failStrategies}], Logger={Logger.GetType().Name}, RequestLoader={requestLoader}, RequestSaver={requestSaver}, ValidateNumInputTokensBeforeRequest={ValidateNumInputTokensBeforeRequest}, RaiseExceptionOnOutputOverflow={RaiseExceptionOnOutputOverflow}, Tokenizer={tokenizer})";
    }
}
