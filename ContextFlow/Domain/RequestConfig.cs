using ContextFlow.Application;
using Microsoft.DeepDev;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Domain;

public class RequestConfig
{
    protected FailStrategy FailStrategy = new FailStrategyThrowException();

    public bool PassAsStringIfNoConverterDefined = false;
    public bool SplitTextAndRetryOnOverflow = true;
    protected bool CheckNumTokensBeforeRequest = false;

    public bool ParseOutputToDynamic = true;

    public LLMTokenizer? Tokenizer = null;

    public RequestConfig(FailStrategy failStrategy) {
        FailStrategy = failStrategy;
    }

    public RequestConfig UsingFailStrategy(FailStrategy failStrategy)
    {
        FailStrategy = failStrategy;
        return this;
    }

    public void ActivateCheckNumTokensBeforeRequest(LLMTokenizer tokenizer)
    {
        CheckNumTokensBeforeRequest = true;
        Tokenizer = tokenizer;
    }

    public void DeactivateCheckNumTokensBeforeRequest()
    {
        CheckNumTokensBeforeRequest = false;
    }

    public bool GetCheckNumTokensBeforeRequest()
    {
        return CheckNumTokensBeforeRequest;
    }

    public RequestConfig UsingOutputIsString(bool isString)
    {
        ParseOutputToDynamic = isString;
        return this;
    }
}
