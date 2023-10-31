using ContextFlow.Application.Request.Async;

namespace ContextFlow.Application.Result.Actions;

/// <summary>
/// Asynchronous actions available for a RequestResult
/// </summary>
public class RequestResultActionsAsync
{
    /// <summary>
    /// The RequestResult on which the methods run
    /// </summary>
    protected RequestResult Result { get; }

    internal RequestResultActionsAsync(RequestResult result)
    {
        Result = result;
    }

    /// <summary>
    /// Returns the next result of an operation that has been build on the given function combined with this RequestResult
    /// </summary>
    /// <param name="funcForNextRequest">The function used to build the next request from this RequestResult</param>
    /// <returns>The result of the request</returns>
    public async Task<RequestResult> Then(Func<RequestResult, LLMRequestAsync> funcForNextRequest)
    {
        return await funcForNextRequest(Result).Complete();
    }

    /// <summary>
    /// If the given condition applies to this RequestResult, the function is executed on it and the result returned. 
    /// If the given condiition does not apply, the current result is returned. 
    /// This is useful to verify the output is valid, and to prompt the LLM to correct it in case it's not.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="funcForNextRequest"></param>
    /// <returns>The resulting or current result depending on the condition</returns>
    public async Task<RequestResult> ThenConditional(Func<RequestResult, bool> condition, Func<RequestResult, LLMRequestAsync> funcForNextRequest)
    {
        if (condition(Result))
            return await funcForNextRequest(Result).Complete();

        return Result;
    }

    /// <summary>
    /// Similar to Then, but with multiple resulting requests.
    /// </summary>
    /// <param name="funcForNextRequests">The function that builds the next requests</param>
    /// <returns>An enumerable containing the results</returns>
    public async Task<IEnumerable<RequestResult>> ThenBranching(Func<RequestResult, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var nextRequests = funcForNextRequest(Result);
        return await Task.WhenAll(nextRequests.Select(async r => await r.Complete()));
    }

    /// <summary>
    /// Executes multiple requests based on the current one, and filters them based on a given condition.
    /// </summary>
    /// <param name="condition">The condition the results go into the Passed enumerable.</param>
    /// <param name="funcForNextRequests"></param>
    /// <returns>The results separated by the outcome of the condition</returns>
    public async Task<(IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed)> ThenBranchingConditionalAsync(Func<RequestResult, bool> predicate, Func<RequestResult, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var results = await Task.WhenAll(funcForNextRequest(Result).Select(async r => await r.Complete()));
        return ActionsUtil.Partition(results, predicate);
    }

    /// <summary>
    /// Executes multiple requests based on the current one, and filters them based on a given condition. Uses an asynchronous predicate/condition.
    /// </summary>
    /// <param name="condition">The condition the results go into the Passed enumerable.</param>
    /// <param name="funcForNextRequests"></param>
    /// <returns>The results separated by the outcome of the condition</returns>
    public async Task<(IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed)> ThenBranchingConditionalAsync(Func<RequestResult, Task<bool>> asyncPredicate, Func<RequestResult, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var results = await Task.WhenAll(funcForNextRequest(Result).Select(async r => await r.Complete()));
        return await ActionsUtil.PartitionAsync(results, asyncPredicate);
    }
}
