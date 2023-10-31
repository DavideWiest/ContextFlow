using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes.Async;

public class ThrowOrSayHiUnderConditionConnectionAsync : LLMConnectionAsync
{
    Func<string, LLMConfig, bool> ErrorCondition;

    public ThrowOrSayHiUnderConditionConnectionAsync(Func<string, LLMConfig, bool> errorCondition)
    {
        ErrorCondition = errorCondition;
    }

    protected override async Task<RequestResult> CallAPIAsync(string prompt, LLMConfig config, CFLogger logger)
    {
        if (ErrorCondition(prompt, config))
        {
            throw new LLMConnectionException("Standard exception of ThrowOrSayHiUnderConditionConnection");
        }
        return new RequestResult("Hi", FinishReason.Stop);
    }
}