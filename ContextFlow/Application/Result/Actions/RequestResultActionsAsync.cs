using ContextFlow.Application.Request.Async;

namespace ContextFlow.Application.Result.Actions;

public class RequestResultActionsAsync
{

    protected RequestResult Result { get; }

    internal RequestResultActionsAsync(RequestResult result)
    {
        Result = result;
    }

    public async Task<RequestResult> Then(Func<RequestResult, LLMRequestAsync> funcForNextRequest)
    {
        return await funcForNextRequest(Result).Complete();
    }

    public async Task<RequestResult> ThenConditional(Func<RequestResult, bool> condition, Func<RequestResult, LLMRequestAsync> funcForNextRequest)
    {
        if (condition(Result))
            return await funcForNextRequest(Result).Complete();

        return Result;
    }

    public async Task<IEnumerable<RequestResult>> ThenBranching(Func<RequestResult, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var nextRequests = funcForNextRequest(Result);
        return await Task.WhenAll(nextRequests.Select(async r => await r.Complete()));
    }

    public async Task<(IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed)> ThenBranchingConditionalAsync(Func<RequestResult, bool> asyncPredicate, Func<RequestResult, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var results = await Task.WhenAll(funcForNextRequest(Result).Select(async r => await r.Complete()));
        return ActionsUtil.Partition(results, asyncPredicate);
    }

    public async Task<(IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed)> ThenBranchingConditionalAsync(Func<RequestResult, Task<bool>> asyncPredicate, Func<RequestResult, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var results = await Task.WhenAll(funcForNextRequest(Result).Select(async r => await r.Complete()));
        return await ActionsUtil.PartitionAsync(results, asyncPredicate);
    }
}
