

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

    public RequestResult Then(Func<ParsedRequestResult<T>, LLMRequest> funcForNextRequest)
    {
        return funcForNextRequest(this).Complete();
    }

    public RequestResult ThenConditional(Func<ParsedRequestResult<T>, bool> condition, Func<ParsedRequestResult<T>, ParsedRequestResult<T>> funcForNextRequest)
    {
        if (condition(this))
            return funcForNextRequest(this);

        return this;
    }

    public IEnumerable<RequestResult> ThenBranching(Func<ParsedRequestResult<T>, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        return funcForNextRequest(this).Select(r => r.Complete());
    }

    public (IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed) ThenBranchingConditional(Func<RequestResult, bool> condition, Func<ParsedRequestResult<T>, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        var results = funcForNextRequest(this).Select(result => result.Complete());
        return Partition(results, condition);
    }
}
