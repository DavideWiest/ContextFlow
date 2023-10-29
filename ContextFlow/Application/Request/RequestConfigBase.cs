using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application.Request;

/// <summary>
/// Abstract base-class which contains data and functionality that both RequestConfig and RequestConfigAsync use
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class RequestConfigBase<T> where T : RequestConfigBase<T>
{
    public CFLogger Logger { get; private set; } = new CFSerilogLogger();

    public bool ValidateNumInputTokensBeforeRequest { get; private set; } = false;
    public bool ThrowExceptionOnOutputOverflow { get; set; } = false;

    public LLMTokenizer? Tokenizer { get; private set; } = null;

    /// <summary>
    /// Sets the tokenizer. It will be used to optionally validate the number of tokens.
    /// </summary>
    /// <param name="tokenizer"></param>
    /// <returns></returns>
    public T UsingTokenizer(LLMTokenizer? tokenizer)
    {
        Tokenizer = tokenizer;
        return (T)this;
    }

    /// <summary>
    /// Activates the option to check the number of tokens of the input (the prompt) before making the request. 
    /// </summary>
    /// <param name="tokenizer">The tokenizer which will be used</param>
    /// <returns></returns>
    public T ActivateCheckNumTokensBeforeRequest(LLMTokenizer tokenizer)
    {
        ValidateNumInputTokensBeforeRequest = true;
        Tokenizer = tokenizer;
        return (T)this;
    }

    /// <summary>
    /// Deactivates the option to check the number of tokens of the input (the prompt) before making the request. 
    /// </summary>
    public T DeactivateCheckNumTokensBeforeRequest()
    {
        ValidateNumInputTokensBeforeRequest = false;
        return (T)this;
    }

    /// <summary>
    /// Sets the option to raise an OutputOverflowException if the LLMConnection returns a RequestResult in which the FinishReason (an enum) is Overflow. 
    /// This means that the LLM could not finish its output before reaching the configured maximum tokens.
    /// it is generally disadvised to do this. You need to have a FailStrategy configured to handle this exception, but you lose the generated output in any case.
    /// </summary>
    /// <param name="raiseExceptionOnOutputOverflow">The options value</param>
    /// <returns></returns>
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
}
