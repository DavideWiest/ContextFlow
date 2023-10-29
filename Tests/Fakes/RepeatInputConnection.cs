using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes;

public class RepeatInputConnection : LLMConnection
{

    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        return new RequestResult(prompt, FinishReason.Stop);
    }
}