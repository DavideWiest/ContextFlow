using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes;

public class OutputOverFlowConnection : LLMConnection
{
    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        return new RequestResult("Hi", FinishReason.Overflow);
    }
}
