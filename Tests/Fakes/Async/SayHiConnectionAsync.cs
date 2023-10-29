using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes.Async;

public class SayHiConnectionAsync : LLMConnectionAsync
{
    protected override async Task<RequestResult> CallAPIAsync(string prompt, LLMConfig config, CFLogger logger)
    {
        return new RequestResult("Hi", FinishReason.Stop);
    }
}