using ContextFlow.Application.Result.Actions;
using ContextFlow.Application.TextUtil;
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
    public string RawOutput { get; }
    public FinishReason FinishReason { get; }
    public ResultAdditionalData? AdditionalData { get; } = null;
    public RequestResultActions Actions { get; }
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