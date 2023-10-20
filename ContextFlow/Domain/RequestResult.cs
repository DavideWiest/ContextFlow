using ContextFlow.Application;
using ContextFlow.Application.TextUtil;
using OpenAI_API.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Domain;

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
    public string RawOutput;
    public FinishReason FinishReason;
    public dynamic? AdditionalData = null;

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

    public ParsedRequestResult<T> Parse<T>(CFConverter<T> converter, dynamic? data = null)
    {
        ParsedRequestResult<T> parsedResult = converter.FromString(RawOutput, data);
        return parsedResult;
    }
}
