using ContextFlow.Application.Result.Actions;
using ContextFlow.Domain;
using Newtonsoft.Json.Linq;

namespace ContextFlow.Application.Result;


public abstract class ResultAdditionalData
{
    public abstract void LoadFromJObject(JObject jObject);
}

/// <summary>
/// The reponse of the LLM API, without the parsed version of the output
/// </summary>
public class RequestResult
{
    /// <summary>
    /// The unmodified otput of the LLM-request
    /// </summary>
    public string RawOutput { get; }
    /// <summary>
    /// The reason why the LLM had to stop its response
    /// </summary>
    public FinishReason FinishReason { get; }
    /// <summary>
    /// Any additional data returned from the LLMConnection. This must be an implementation of the abstract class ResultAdditionalData
    /// </summary>
    public ResultAdditionalData? AdditionalData { get; } = null;
    /// <summary>
    /// Synchronous actions based on the result
    /// </summary>
    public RequestResultActions Actions { get; }
    /// <summary>
    /// Asynchronous action based on the result
    /// </summary>
    public RequestResultActionsAsync AsyncActions { get; }

    public RequestResult(string rawOutput, FinishReason finishReason)
    {
        RawOutput = rawOutput;
        FinishReason = finishReason;
        Actions = new(this);
        AsyncActions = new(this);
    }

    public RequestResult(string rawOutput, FinishReason finishReason, ResultAdditionalData additionalData)
    {
        RawOutput = rawOutput;
        FinishReason = finishReason;
        AdditionalData = additionalData;
        Actions = new(this);
        AsyncActions = new(this);
    }

    public RequestResult(RequestResult result)
    {
        RawOutput = result.RawOutput;
        FinishReason = result.FinishReason;
        AdditionalData = result.AdditionalData;
        Actions = new(this);
        AsyncActions = new(this);
    }

}