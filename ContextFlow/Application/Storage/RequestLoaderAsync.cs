using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Storage;

/// <summary>
/// Async version of RequestLoader
/// </summary>
public abstract class RequestLoaderAsync
{
    public async Task<RequestResult?> LoadMatchIfExistsAsync(LLMRequestAsync request)
    {
        bool matchExists = await MatchExistsAsync(request);
        request.RequestConfig.Logger.Information("Trying to load the result of the current request - Match exists: {matchExists}", matchExists);
        return matchExists ? await LoadMatchAsync(request) : null;
    }

    public abstract Task<bool> MatchExistsAsync(LLMRequestAsync request);
    public abstract Task<RequestResult> LoadMatchAsync(LLMRequestAsync request);
}