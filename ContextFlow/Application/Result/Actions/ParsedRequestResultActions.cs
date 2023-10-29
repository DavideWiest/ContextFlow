using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;

namespace ContextFlow.Application.Result.Actions;

/// <summary>
/// The result from the LLM API, parsed.
/// </summary>
public class ParsedRequestResultActions<T>
{
    public ParsedRequestResult<T> ParsedResult;

    public ParsedRequestResultActions(ParsedRequestResult<T> requestresult)
    {
        ParsedResult = requestresult;
    }

    public RequestResult Then(Func<ParsedRequestResult<T>, LLMRequest> funcForNextRequest)
    {
        return funcForNextRequest(ParsedResult).Complete();
    }

    public RequestResult ThenConditional(Func<ParsedRequestResult<T>, bool> condition, Func<ParsedRequestResult<T>, ParsedRequestResult<T>> funcForNextRequest)
    {
        if (condition(ParsedResult))
            return funcForNextRequest(ParsedResult);

        return ParsedResult;
    }

    public IEnumerable<RequestResult> ThenBranching(Func<ParsedRequestResult<T>, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        return funcForNextRequest(ParsedResult).Select(r => r.Complete());
    }

    public (IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed) ThenBranchingConditional(Func<RequestResult, bool> condition, Func<ParsedRequestResult<T>, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        var results = funcForNextRequest(ParsedResult).Select(result => result.Complete());
        return ActionsUtil.Partition(results, condition);
    }
}
