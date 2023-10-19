using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContextFlow.Domain;

namespace ContextFlow.Application;

/// <summary>
/// The result from the LLM API, parsed.
/// </summary>
public class ParsedRequestResult<T>: RequestResult
{
    public T ParsedOutput;

    public ParsedRequestResult(RequestResult requestresult, dynamic parsedOutput) : base(requestresult)
    {
        ParsedOutput = parsedOutput;
    }
}
