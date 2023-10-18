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
    private FailStrategy FailStrategy;

    public bool PassAsStringIfNoConverterDefined = false;
    public bool SplitTextAndRetryOnOverflow = true;
    private bool CheckNumTokensBeforeRequest = false;
    public LLMTokenizer? Tokenizer = null;

    public RequestConfig(FailStrategy failStrategy) {
        FailStrategy = failStrategy;
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

}
