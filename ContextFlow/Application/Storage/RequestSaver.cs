using ContextFlow.Application.Request;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Storage;

public abstract class RequestSaver
{
    public abstract void SaveRequest(LLMRequest request, RequestResult result);
}
