using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes;

public class SayHiConnection : LLMConnection
{
    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        return new RequestResult("Hi", FinishReason.Stop);
    }
}

public class SayHiConnectionAsync : LLMConnectionAsync
{
    protected override async Task<RequestResultAsync> CallAPIAsync(string prompt, LLMConfig config, CFLogger logger)
    {
        return new RequestResultAsync("hi", FinishReason.Stop);
    }
}

public class TriggerOverflowConnection : LLMConnection
{
    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        return new RequestResult("hi", FinishReason.Overflow);
    }
}

public class ThrowThenSayHiConnectionAfterN : LLMConnection
{
    int i = 0;
    int nThrow = 1;

    public ThrowThenSayHiConnectionAfterN(int nthrow)
    {
        nThrow = nthrow;
    }

    protected override RequestResultAsync CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        i++;
        if (i <= nThrow)
        {
            throw new LLMException("Standard exception of ThrowThenSayHiConnectionAfterN");
        }
        var output = string.Concat(Enumerable.Repeat("hi", config.MaxTotalTokens));
        return new RequestResultAsync(output, FinishReason.Stop);
    }
}

public class ThrowThenSayHiConnectionAfterEvent : LLMConnection
{
    Func<string, LLMConfig, bool> SayHiEvent;
    int nThrow = 1;

    public ThrowThenSayHiConnectionAfterEvent(Func<string, LLMConfig, bool> sayHiEvent)
    {
        SayHiEvent = sayHiEvent;
    }

    protected override RequestResultAsync CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        if (SayHiEvent(prompt, config))
        {
            throw new LLMException("Standard exception of ThrowThenSayHiConnectionAfterEvent");
        }
        var output = string.Concat(Enumerable.Repeat("hi", config.MaxTotalTokens));
        return new RequestResultAsync(output, FinishReason.Stop);
    }
}

public class AlwaysThrowLLMExceptionConnection : LLMConnection
{
    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        throw new LLMException("Standard exception of AlwaysThrowLLMExceptionConnection");
    }
}

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

public class SayGivenInputConnectionAsync : LLMConnectionAsync
{
    int i = -1;
    string[] Input;

    public SayGivenInputConnectionAsync(string[] input)
    {
        Input = input;
    }

    protected override async Task<RequestResultAsync> CallAPIAsync(string prompt, LLMConfig config, CFLogger logger)
    {
        i++;
        return new RequestResultAsync(Input[i], FinishReason.Stop);
    }
}

public class RepeatInputConnection : LLMConnection
{

    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        return new RequestResult(prompt, FinishReason.Stop);
    }
}