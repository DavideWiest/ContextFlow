using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes;

public class AlwaysThrowLLMExceptionConnection : LLMConnection
{
    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        throw new LLMException("Standard exception of AlwaysThrowLLMExceptionConnection");
    }
}