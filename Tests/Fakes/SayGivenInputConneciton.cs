using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes;

public class SayGivenInputConnection : LLMConnection
{
    int i = -1;
    string[] Input;

    public SayGivenInputConnection(string[] input)
    {
        Input = input;
    }

    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        i++;
        return new RequestResult(Input[i], FinishReason.Stop);
    }
}