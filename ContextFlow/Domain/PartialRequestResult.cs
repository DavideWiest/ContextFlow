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
/// The reponse of the LLM API, without the parsed version of the output
/// </summary>
public class PartialRequestResult
{
    public string RawOutput;
    public FinishReason FinishReason;
    public object? AdditionalData = null;

    public PartialRequestResult(string rawOutput, FinishReason finishReason)
    {
        RawOutput = rawOutput;
        FinishReason = finishReason;
    }

    public PartialRequestResult(string rawOutput, FinishReason finishReason, object additionalData)
    {
        RawOutput = rawOutput;
        FinishReason = finishReason;
        AdditionalData = additionalData;
    }
}
