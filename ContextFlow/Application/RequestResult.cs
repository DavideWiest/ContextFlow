using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContextFlow.Domain;

namespace ContextFlow.Application;

/// <summary>
/// The result from the LLM API.
/// </summary>
public class RequestResult
{
    public string RawOutput;
    public dynamic ParsedOutput;
    public FinishReason FinishReason;
    public dynamic? AdditionalData = null;

    public RequestResult(PartialRequestResult partialrequestresult, dynamic parsedOutput)
    {
        RawOutput = partialrequestresult.RawOutput;
        ParsedOutput = parsedOutput;
        FinishReason = partialrequestresult.FinishReason;
        AdditionalData = partialrequestresult.AdditionalData;
    }

    public T CastOutput<T>()
    {
        return Convert.ChangeType(ParsedOutput, typeof(T));
    }

    public T CastAdditionalData<T>()
    {
        return Convert.ChangeType(AdditionalData, typeof(T));
    }
}
