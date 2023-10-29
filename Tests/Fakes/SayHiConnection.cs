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
        return new RequestResultAsync("Hi", FinishReason.Stop);
    }
}

public class TriggerOverflowConnection : LLMConnection
{
    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        var output = string.Concat(Enumerable.Repeat("Hi", config.MaxTotalTokens));
        return new RequestResult(output, FinishReason.Overflow);
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
        return new RequestResultAsync("Hi", FinishReason.Stop);
    }
}

public class ThrowOrSayHiUnderConditionConnection : LLMConnection
{
    Func<string, LLMConfig, bool> ErrorCondition;

    public ThrowOrSayHiUnderConditionConnection(Func<string, LLMConfig, bool> errorCondition)
    {
        ErrorCondition = errorCondition;
    }

    protected override RequestResultAsync CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        if (ErrorCondition(prompt, config))
        {
            throw new LLMException("Standard exception of ThrowOrSayHiUnderConditionConnection");
        }
        return new RequestResultAsync("Hi", FinishReason.Stop);
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

public class RepeatInputConnectionAsync : LLMConnectionAsync
{

    protected override async Task<RequestResultAsync> CallAPIAsync(string prompt, LLMConfig config, CFLogger logger)
    {
        return new RequestResultAsync(prompt, FinishReason.Stop);
    }
}

public class OutputOverFlowConnection : LLMConnection
{
    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        return new RequestResult("Hi", FinishReason.Overflow);
    }
}