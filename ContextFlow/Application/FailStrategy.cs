using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ContextFlow.Domain;

namespace ContextFlow.Application;

public abstract class FailStrategy
{
}

public class FailStrategyRetrySameSettings: FailStrategy
{
    public int MaxRetries { get; }

    public FailStrategyRetrySameSettings(int maxRetries=3)
    {
        MaxRetries = maxRetries;
    }
}

public class FailStrategyRetryNewSettings : FailStrategy
{
    public LLMConfig LConf { get; }
    public RequestConfig RConf { get; }
    public Prompt Prompt { get; }
    public int MaxRetries { get; }

    public FailStrategyRetryNewSettings(LLMConfig lConf, RequestConfig rConf, Prompt prompt, int maxRetries=3)
    {
        LConf = lConf;
        RConf = rConf;
        Prompt = prompt;
        MaxRetries = maxRetries;
    }
}

public class FailStrategyThrowException : FailStrategy
{ 

}