using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes;

public class ThrowThenSayHiConnectionAfterN : LLMConnection
{
    int i = 0;
    int nThrow = 1;

    public ThrowThenSayHiConnectionAfterN(int nthrow)
    {
        nThrow = nthrow;
    }

    protected override RequestResult CallAPI(string prompt, LLMConfig config, CFLogger logger)
    {
        i++;
        if (i <= nThrow)
        {
            throw new LLMException("Standard exception of ThrowThenSayHiConnectionAfterN");
        }
        return new RequestResult("Hi", FinishReason.Stop);
    }
}
