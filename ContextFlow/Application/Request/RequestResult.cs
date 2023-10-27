using ContextFlow.Application;
using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;
using OpenAI_API.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Request;


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

    public virtual RequestResult ThenLinear(Func<RequestResult, LLMRequest> funcForNextRequest)
    {
        return funcForNextRequest(this).Complete();
    }

    public virtual RequestResult? ThenLinearCondititonal(Func<RequestResult, bool> condition, Func<RequestResult, LLMRequest> funcForNextRequest)
    {
        if (condition(this))
            return funcForNextRequest(this).Complete();

        return null;
    }

    public virtual IEnumerable<RequestResult> ThenBranching(Func<RequestResult, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        return funcForNextRequest(this).Select(r => r.Complete());
    }

    public virtual IEnumerable<RequestResult> ThenBranchingConditional(Func<RequestResult, bool> condition, Func<RequestResult, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        return funcForNextRequest(this).Select(r => r.Complete()).Where(res => condition(res));
    }
}

public abstract class ResultAdditionalData
{

}