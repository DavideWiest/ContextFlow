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
    public CFLogger Logger = new CFDefaultLogger();

    public bool SplitTextAndRetryOnOverflow = true;

    public bool CheckNumTokensBeforeRequest { get; private set; } = false;

    public LLMTokenizer? Tokenizer = null;

    public RequestConfig(FailStrategy failStrategy) {
        FailStrategy = failStrategy;
    }

    public RequestConfig UsingFailStrategy(FailStrategy failStrategy)
    {
        FailStrategy = failStrategy;
        return this;
    }

    public RequestConfig ActivateCheckNumTokensBeforeRequest(LLMTokenizer tokenizer)
    {
        CheckNumTokensBeforeRequest = true;
        Tokenizer = tokenizer;
        return this;
    }

    public RequestConfig DeactivateCheckNumTokensBeforeRequest()
    {
        CheckNumTokensBeforeRequest = false;
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
