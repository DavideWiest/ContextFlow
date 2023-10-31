using ContextFlow.Application.Result;
using ContextFlow.Domain;

namespace ContextFlow.Application.Storage;

/// <summary>
/// Represents a data-object for a request result, where all corresponding fields are publicly writable to. 
/// Useful to load data with a package that sets the properties and requires a parameterless constructor, but be aware that this class and its children may unknowingly deprecate.
/// </summary>
public class WritableRequestResult
{
    public string RawOutput { get; set; } = default!;
    public FinishReason FinishReason { get; set; }
    public ResultAdditionalData? AdditionalData { get; set; } = null;

    public RequestResult ToRequestResult()
    {
        if (AdditionalData != null)
            return new RequestResult(RawOutput, FinishReason, AdditionalData);
        return new RequestResult(RawOutput, FinishReason);
    }
}
