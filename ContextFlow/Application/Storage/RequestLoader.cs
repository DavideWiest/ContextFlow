using ContextFlow.Application.Request;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Storage;

public abstract class RequestLoader
{
    public RequestResult? LoadMatchIfExists(LLMRequest request)
    {
        bool matchExists = MatchExists(request);
        request.RequestConfig.Logger.Information("Trying to load the result of the current request - Match exists: {matchExists}", matchExists);
        return matchExists ? LoadMatch(request) : null;
    }
    public abstract bool MatchExists(LLMRequest request);
    public abstract RequestResult LoadMatch(LLMRequest request);
}