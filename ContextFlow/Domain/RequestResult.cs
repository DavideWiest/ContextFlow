using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Domain;

public enum FinishReason
{
    Overflow,
    Stop
}

/// <summary>
/// The result from the LLM API.
/// </summary>
public class RequestResult
{
    public string RawResponse;
    public FinishReason FinishReason = FinishReason.Stop;

    public RequestResult(string rawResponse, FinishReason finishReason)
    {
        RawResponse = rawResponse;
        FinishReason = finishReason;
    }
}
