using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes;

public class ThrowOrSayHiUnderConditionConnection : LLMConnection
{
    Func<string, LLMConfig, bool> ErrorCondition;

    public ThrowOrSayHiUnderConditionConnection(Func<string, LLMConfig, bool> errorCondition)
    {
        ErrorCondition = errorCondition;
    }

    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        if (ErrorCondition(prompt, config))
        {
            throw new LLMConnectionException("Standard exception of ThrowOrSayHiUnderConditionConnection");
        }
        return new RequestResult("Hi", FinishReason.Stop);
    }
}
