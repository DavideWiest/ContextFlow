using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContextFlow.Domain;

namespace ContextFlow.Application;

public abstract class FailStrategy
{
}

public class FailStrategyRetrySameSettings: FailStrategy
{  
}

public class FailStrategyRetryNewSettings : FailStrategy
{
    public LLMConfig LConf { get; }
    public RequestConfig RConf { get; }
    public FailStrategy(LLMConfig lconf, RequestConfig rconf)
    {
        LConf = lconf;
        RConf = rconf;
    }
}

public class FailStrategyRaiseException : FailStrategy
{ 
}