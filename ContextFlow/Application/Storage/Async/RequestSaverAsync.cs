using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Storage.Async;

/// <summary>
/// Async version of RequestSaver
/// </summary>
public abstract class RequestSaverAsync
{
    public abstract Task SaveRequestAsync(LLMRequestAsync request, RequestResult result);
}