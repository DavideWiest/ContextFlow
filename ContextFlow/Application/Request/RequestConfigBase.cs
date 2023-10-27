using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application.Request;

public abstract class RequestConfigBase
{
    public CFLogger Logger { get; private set; } = new CFSerilogLogger();

    public bool ValidateNumInputTokensBeforeRequest { get; private set; } = false;
    public bool RaiseExceptionOnOutputOverflow { get; set; } = false;

    public LLMTokenizer? Tokenizer { get; private set; } = null;

    public RequestConfigBase UsingTokenizer(LLMTokenizer? tokenizer)
    {
        Tokenizer = tokenizer;
        return this;
    }

    public RequestConfigBase ActivateCheckNumTokensBeforeRequest(LLMTokenizer tokenizer)
    {
        ValidateNumInputTokensBeforeRequest = true;
        Tokenizer = tokenizer;
        return this;
    }

    public RequestConfigBase DeactivateCheckNumTokensBeforeRequest()
    {
        ValidateNumInputTokensBeforeRequest = false;
        return this;
    }

    public RequestConfigBase UsingRaiseExceptionOnOutputOverflow(bool raiseExceptionOnOutputOverflow)
    {
        RaiseExceptionOnOutputOverflow = raiseExceptionOnOutputOverflow;
        return this;
    }

    public RequestConfigBase UsingLogger(CFLogger log)
    {
        Logger = log;
        return this;
    }

    public override abstract string ToString();
}
