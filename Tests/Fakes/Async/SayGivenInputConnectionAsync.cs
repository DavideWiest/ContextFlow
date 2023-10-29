using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes.Async;

public class SayGivenInputConnectionAsync : LLMConnectionAsync
{
    int i = -1;
    string[] Input;

    public SayGivenInputConnectionAsync(string[] input)
    {
        Input = input;
    }

    protected override async Task<RequestResult> CallAPIAsync(string prompt, LLMConfig config, CFLogger logger)
    {
        i++;
        return new RequestResult(Input[i], FinishReason.Stop);
    }
}