

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

    public RequestResult ThenLinear(Func<ParsedRequestResult<T>, LLMRequest> funcForNextRequest)
    {
        return funcForNextRequest(this).Complete();
    }

    public RequestResult? ThenLinearCondititonal(Func<ParsedRequestResult<T>, bool> condition, Func<ParsedRequestResult<T>, LLMRequest> funcForNextRequest)
    {
        if (condition(this))
            return funcForNextRequest(this).Complete();

        return null;
    }

    public IEnumerable<RequestResult> ThenBranching(Func<ParsedRequestResult<T>, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        return funcForNextRequest(this).Select(r => r.Complete());
    }

    public IEnumerable<RequestResult> ThenBranchingConditional(Func<RequestResult, bool> condition, Func<ParsedRequestResult<T>, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        return funcForNextRequest(this).Select(r => r.Complete()).Where(res => condition(res));
    }
}
