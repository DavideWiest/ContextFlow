using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;
using Newtonsoft.Json.Linq;

namespace ContextFlow.Application.Request;


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
    public ResultAdditionalData? AdditionalData { get; }  = null;

    public RequestResult(string rawOutput, FinishReason finishReason)
    {
        RawOutput = rawOutput;
        FinishReason = finishReason;
    }

    public RequestResult(string rawOutput, FinishReason finishReason, ResultAdditionalData additionalData)
    {
        RawOutput = rawOutput;
        FinishReason = finishReason;
        AdditionalData = additionalData;
    }

    public RequestResult(RequestResult result)
    {
        RawOutput = result.RawOutput;
        FinishReason = result.FinishReason;
        AdditionalData = result.AdditionalData;
    }

    public ParsedRequestResult<T> Parse<T>(ToObjectConverter<T> converter)
    {
        ParsedRequestResult<T> parsedResult = new(this, converter.Convert(RawOutput));
        return parsedResult;
    }

    public ParsedRequestResult<T> Parse<T>(Func<RequestResult, T> converter)
    {
        ParsedRequestResult<T> parsedResult = new(this, converter(this));
        return parsedResult;
    }

    public RequestResult SaveAndContinue(out RequestResult varToSaveTo)
    {
        varToSaveTo = this;
        return this;
    }

    public virtual RequestResult Then(Func<RequestResult, LLMRequest> funcForNextRequest)
    {
        return funcForNextRequest(this).Complete();
    }

    public virtual RequestResult ThenConditional(Func<RequestResult, bool> condition, Func<RequestResult, LLMRequest> funcForNextRequest)
    {
        if (condition(this))
            return funcForNextRequest(this).Complete();

        return this;
    }

    public virtual IEnumerable<RequestResult> ThenBranching(Func<RequestResult, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        return funcForNextRequest(this).Select(r => r.Complete());
    }

    public virtual (IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed) ThenBranchingConditional(Func<RequestResult, bool> condition, Func<RequestResult, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        var results = funcForNextRequest(this).Select(result => result.Complete());
        return Partition(results, condition);
    }

    protected static (IEnumerable<T> Passed, IEnumerable<T> Failed) Partition<T>(IEnumerable<T> source, Func<T, bool> predicate)
    {
        var trueList = new List<T>();
        var falseList = new List<T>();

        foreach (var item in source)
        {
            if (predicate(item))
            {
                trueList.Add(item);
            }
            else
            {
                falseList.Add(item);
            }
        }

        return (trueList, falseList);
    }
}