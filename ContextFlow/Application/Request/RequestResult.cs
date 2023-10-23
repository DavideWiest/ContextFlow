using ContextFlow.Application;
using ContextFlow.Application.TextUtil;
using OpenAI_API.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Request;

public enum FinishReason
{
    Overflow,
    Stop,
    Unknown
}

/// <summary>
/// The reponse of the LLM API, without the parsed version of the output
/// </summary>
public class RequestResult
{
    public string RawOutput { get; }
    public FinishReason FinishReason { get; }
    public dynamic? AdditionalData { get; }  = null;

    public RequestResult(string rawOutput, FinishReason finishReason)
    {
        RawOutput = rawOutput;
        FinishReason = finishReason;
    }

    public RequestResult(string rawOutput, FinishReason finishReason, dynamic additionalData)
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

    public virtual RequestResult ThenLinear(Func<RequestResult, LLMRequest> funcForNextRequest)
    {
        return funcForNextRequest(this).Complete();
    }

    public virtual IEnumerable<RequestResult> ThenBranching(Func<RequestResult, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        return funcForNextRequest(this).Select(r => r.Complete());
    }
}
