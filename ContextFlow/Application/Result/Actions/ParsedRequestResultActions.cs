using ContextFlow.Application.Request;

namespace ContextFlow.Application.Result.Actions;

/// <summary>
/// Synchronous actions available for a ParsedRequestResult
/// </summary>
public class ParsedRequestResultActions<T>
{
    /// <summary>
    /// The ParsedRequestResult on which the methods run
    /// </summary>
    protected ParsedRequestResult<T> ParsedResult;

    internal ParsedRequestResultActions(ParsedRequestResult<T> requestresult)
    {
        ParsedResult = requestresult;
    }

    /// <summary>
    /// Returns the next result of an operation that has been build on the given function combined with this ParsedRequestResult
    /// </summary>
    /// <param name="funcForNextRequest">The function used to build the next request from this ParsedRequestResult</param>
    /// <returns>The result of the request</returns>
    public RequestResult Then(Func<ParsedRequestResult<T>, LLMRequest> funcForNextRequest)
    {
        return funcForNextRequest(ParsedResult).Complete();
    }

    /// <summary>
    /// If the given condition applies to this ParsedRequestResult, the function is executed on it and the result returned. 
    /// If the given condiition does not apply, the current result is returned. 
    /// This is useful to verify the output is valid, and to prompt the LLM to correct it in case it's not.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="funcForNextRequest"></param>
    /// <returns>The resulting or current result depending on the condition</returns>
    public ParsedRequestResult<T> ThenConditional(Func<ParsedRequestResult<T>, bool> condition, Func<ParsedRequestResult<T>, ParsedRequestResult<T>> funcForNextRequest)
    {
        if (condition(ParsedResult))
            return funcForNextRequest(ParsedResult);

        return ParsedResult;
    }

    /// <summary>
    /// Similar to Then, but with multiple resulting requests.
    /// </summary>
    /// <param name="funcForNextRequests">The function that builds the next requests</param>
    /// <returns>An enumerable containing the results</returns>
    public IEnumerable<RequestResult> ThenBranching(Func<ParsedRequestResult<T>, IEnumerable<LLMRequest>> funcForNextRequests)
    {
        return funcForNextRequests(ParsedResult).Select(r => r.Complete());
    }

    /// <summary>
    /// Executes multiple requests based on the current one, and filters them based on a given condition.
    /// </summary>
    /// <param name="condition">The condition the results go into the Passed enumerable.</param>
    /// <param name="funcForNextRequests"></param>
    /// <returns>The results separated by the outcome of the condition</returns>
    public (IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed) ThenBranchingConditional(Func<RequestResult, bool> condition, Func<ParsedRequestResult<T>, IEnumerable<LLMRequest>> funcForNextRequests)
    {
        var results = funcForNextRequests(ParsedResult).Select(result => result.Complete());
        return ActionsUtil.Partition(results, condition);
    }
}
