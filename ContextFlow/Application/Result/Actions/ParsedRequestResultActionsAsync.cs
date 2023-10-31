using ContextFlow.Application.Request.Async;

namespace ContextFlow.Application.Result.Actions;

/// <summary>
/// Asynchronous actions available for a ParsedRequestResult
/// </summary>
public class ParsedRequestResultActionsAsync<T>
{
    /// <summary>
    /// The ParsedRequestResult on which the methods run
    /// </summary>
    protected ParsedRequestResult<T> ParsedResult;

    internal ParsedRequestResultActionsAsync(ParsedRequestResult<T> parsedResult)
    {
        ParsedResult = parsedResult;
    }

    /// <summary>
    /// Returns the next result of an operation that has been build on the given function combined with this ParsedRequestResult
    /// </summary>
    /// <param name="funcForNextRequest">The function used to build the next request from this ParsedRequestResult</param>
    /// <returns>The result of the request</returns>
    public async Task<RequestResult> Then(Func<ParsedRequestResult<T>, LLMRequestAsync> funcForNextRequest)
    {
        return await funcForNextRequest(ParsedResult).Complete();
    }

    /// <summary>
    /// Returns the next result of an operation that has been build on the given function combined with this ParsedRequestResult
    /// </summary>
    /// <param name="funcForNextRequest">The function used to build the next request from this ParsedRequestResult</param>
    /// <returns>The result of the request</returns>
    public async Task<RequestResult> Then(Func<ParsedRequestResult<T>, Task<LLMRequestAsync>> funcForNextRequest)
    {
        return await (await funcForNextRequest(ParsedResult)).Complete();
    }

    /// <summary>
    /// If the given condition applies to this ParsedRequestResult, the function is executed on it and the result returned. 
    /// If the given condiition does not apply, the current result is returned. 
    /// This is useful to verify the output is valid, and to prompt the LLM to correct it in case it's not.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="funcForNextRequest"></param>
    /// <returns>The resulting or current result depending on the condition</returns>
    public async Task<ParsedRequestResult<T>> ThenConditional(Func<ParsedRequestResult<T>, Task<bool>> condition, Func<ParsedRequestResult<T>, ParsedRequestResult<T>> funcForNextRequest)
    {
        if (await condition(ParsedResult))
            return funcForNextRequest(ParsedResult);

        return ParsedResult;
    }

    /// <summary>
    /// Similar to Then, but with multiple resulting requests.
    /// </summary>
    /// <param name="funcForNextRequests">The function that builds the next requests</param>
    /// <returns>An enumerable containing the results</returns>
    public async Task<IEnumerable<RequestResult>> ThenBranching(Func<ParsedRequestResult<T>, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var nextRequests = funcForNextRequest(ParsedResult);
        return await Task.WhenAll(nextRequests.Select(async r => await r.Complete()));
    }

    /// <summary>
    /// Executes multiple requests based on the current one, and filters them based on a given condition.
    /// </summary>
    /// <param name="condition">The condition the results go into the Passed enumerable.</param>
    /// <param name="funcForNextRequests"></param>
    /// <returns>The results separated by the outcome of the condition</returns>
    public async Task<(IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed)> ThenBranchingConditionalAsync(Func<RequestResult, bool> asyncPredicate, Func<ParsedRequestResult<T>, IEnumerable<LLMRequestAsync>> funcForNextRequests)
    {
        var results = await CompleteAllRequests(funcForNextRequests);
        return ActionsUtil.Partition(results, asyncPredicate);
    }

    /// <summary>
    /// Executes multiple requests based on the current one, and filters them based on a given condition.
    /// </summary>
    /// <param name="condition">The condition the results go into the Passed enumerable.</param>
    /// <param name="funcForNextRequests"></param>
    /// <returns>The results separated by the outcome of the condition</returns>
    public async Task<(IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed)> ThenBranchingConditionalAsync(Func<RequestResult, Task<bool>> asyncPredicate, Func<ParsedRequestResult<T>, IEnumerable<LLMRequestAsync>> funcForNextRequests)
    {
        var results = await CompleteAllRequests(funcForNextRequests);
        return await ActionsUtil.PartitionAsync(results, asyncPredicate);
    }

    private async Task<IEnumerable<RequestResult>> CompleteAllRequests(Func<ParsedRequestResult<T>, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        return await Task.WhenAll(funcForNextRequest(ParsedResult).Select(async r => await r.Complete()));
    }
}
