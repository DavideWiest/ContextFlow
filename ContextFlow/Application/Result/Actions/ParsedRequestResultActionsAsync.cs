using ContextFlow.Application.Request.Async;

namespace ContextFlow.Application.Result.Actions;

public class ParsedRequestResultActionsAsync<T>
{
    public ParsedRequestResult<T> ParsedResult;

    internal ParsedRequestResultActionsAsync(ParsedRequestResult<T> parsedResult)
    {
        ParsedResult = parsedResult;
    }

    public async Task<RequestResult> Then(Func<ParsedRequestResult<T>, LLMRequestAsync> funcForNextRequest)
    {
        return await funcForNextRequest(ParsedResult).Complete();
    }

    public async Task<RequestResult> Then(Func<ParsedRequestResult<T>, Task<LLMRequestAsync>> funcForNextRequest)
    {
        return await (await funcForNextRequest(ParsedResult)).Complete();
    }

    public async Task<ParsedRequestResult<T>> ThenConditional(Func<ParsedRequestResult<T>, Task<bool>> condition, Func<ParsedRequestResult<T>, ParsedRequestResult<T>> funcForNextRequest)
    {
        if (await condition(ParsedResult))
            return funcForNextRequest(ParsedResult);

        return ParsedResult;
    }

    public async Task<IEnumerable<RequestResult>> ThenBranching(Func<ParsedRequestResult<T>, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var nextRequests = funcForNextRequest(ParsedResult);
        return await Task.WhenAll(nextRequests.Select(async r => await r.Complete()));
    }

    public async Task<(IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed)> ThenBranchingConditionalAsync(Func<RequestResult, bool> asyncPredicate, Func<ParsedRequestResult<T>, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var results = await CompleteAllRequests(funcForNextRequest);
        return ActionsUtil.Partition(results, asyncPredicate);
    }

    public async Task<(IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed)> ThenBranchingConditionalAsync(Func<RequestResult, Task<bool>> asyncPredicate, Func<ParsedRequestResult<T>, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var results = await CompleteAllRequests(funcForNextRequest);
        return await ActionsUtil.PartitionAsync(results, asyncPredicate);
    }

    private async Task<IEnumerable<RequestResult>> CompleteAllRequests(Func<ParsedRequestResult<T>, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        return await Task.WhenAll(funcForNextRequest(ParsedResult).Select(async r => await r.Complete()));
    }
}
