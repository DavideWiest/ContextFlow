using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;
using ContextFlow.Domain;

namespace ContextFlow.Application.Storage;

public class WritableRequestResult
{
    public string RawOutput { get; set; }
    public FinishReason FinishReason { get; set; }
    public ResultAdditionalData? AdditionalData { get; set; } = null;

    public RequestResult ToRequestResult()
    {
        if (AdditionalData != null)
            return new RequestResult(RawOutput, FinishReason, AdditionalData);
        return new RequestResult(RawOutput, FinishReason);
    }
}
