using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application.Request;

public abstract class RequestConfigBase<T> where T : RequestConfigBase<T>
{
    public CFLogger Logger { get; private set; } = new CFSerilogLogger();

    public bool ValidateNumInputTokensBeforeRequest { get; private set; } = false;
    public bool ThrowExceptionOnOutputOverflow { get; set; } = false;

    public LLMTokenizer? Tokenizer { get; private set; } = null;

    public T UsingTokenizer(LLMTokenizer? tokenizer)
    {
        Tokenizer = tokenizer;
        return (T)this;
    }

    public T ActivateCheckNumTokensBeforeRequest(LLMTokenizer tokenizer)
    {
        ValidateNumInputTokensBeforeRequest = true;
        Tokenizer = tokenizer;
        return (T)this;
    }

    public T DeactivateCheckNumTokensBeforeRequest()
    {
        ValidateNumInputTokensBeforeRequest = false;
        return (T)this;
    }

    public T UsingRaiseExceptionOnOutputOverflow(bool raiseExceptionOnOutputOverflow)
    {
        ThrowExceptionOnOutputOverflow = raiseExceptionOnOutputOverflow;
        return (T)this;
    }

    public T UsingLogger(CFLogger log)
    {
        Logger = log;
        return (T)this;
    }

    public override abstract string ToString();
}
