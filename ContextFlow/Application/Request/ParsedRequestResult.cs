

namespace ContextFlow.Application.Request;

/// <summary>
/// The result from the LLM API, parsed.
/// </summary>
public class ParsedRequestResult<T>: RequestResult
{
    public T ParsedOutput;

    public ParsedRequestResult(RequestResult requestresult, T parsedOutput) : base(requestresult)
    {
        ParsedOutput = parsedOutput;
    }
}
