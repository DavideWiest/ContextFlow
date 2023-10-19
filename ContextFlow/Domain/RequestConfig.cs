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
    public CFLogger log = new CFDefaultLogger();
    public CFConverter outputConverter = new DefaultConverter(true);

    public bool SplitTextAndRetryOnOverflow = true;
    protected bool CheckNumTokensBeforeRequest = false;
    protected bool ParseOutputToDynamic = true;

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

    public bool GetCheckNumTokensBeforeRequest()
    {
        return CheckNumTokensBeforeRequest;
    }

    public RequestConfig UsingLogger(CFLogger log)
    {
        SetLogger(log);
        return this;
    }

    public void SetLogger(CFLogger log)
    {
        this.log = log;
    }

    public RequestConfig UsingOutputConverter(CFConverter converter)
    {
        SetOutputConverter(converter);
        return this;
    }

    public void SetOutputConverter(CFConverter converter)
    {
        outputConverter = converter;
    }

    public RequestConfig ActivateParseOutputToDynamic(CFConverter converter)
    {
        ParseOutputToDynamic = true;
        UsingOutputConverter(converter);
        return this;
    }

    public RequestConfig DeactivateParseOutputToDynamic()
    {
        ParseOutputToDynamic = true;
        return this;
    }

    public bool GetParseOutputToDynamic()
    {
        return ParseOutputToDynamic;
    }
}
