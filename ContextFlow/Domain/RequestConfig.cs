using ContextFlow.Application;
using ContextFlow.Application.TextUtil;
using ContextFlow.Infrastructure.Logging;
using Microsoft.DeepDev;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Domain;

public class RequestConfig
{
    public FailStrategy FailStrategy = new FailStrategyThrowException();
    public OverflowStrategy OverflowStrategy = new OverflowStrategyThrowException();
    public CFLogger Logger = new CFSerilogLogger();

    public bool SplitTextAndRetryOnOverflow = true;
    public bool ValidateNumInputTokensBeforeRequest { get; private set; } = true;

    public LLMTokenizer? Tokenizer = null;

    public RequestConfig(FailStrategy failStrategy) {
        FailStrategy = failStrategy;
    }

    public RequestConfig UsingFailStrategy(FailStrategy failStrategy)
    {
        FailStrategy = failStrategy;
        return this;
    }

    public RequestConfig UsingOverflowStrategy(OverflowStrategy overflowStrategy)
    {
        OverflowStrategy = overflowStrategy;
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

    public RequestConfig UsingSplitTextAndRetryOnOverflow(bool splitTextAndRetryOnOverflow)
    {
        SplitTextAndRetryOnOverflow = splitTextAndRetryOnOverflow;
        return this;
    }

}
