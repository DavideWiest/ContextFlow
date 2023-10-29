using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Request.Async;

public class ParsedRequestResultAsync<T> : RequestResultAsync
{
    public T ParsedOutput;

    public ParsedRequestResultAsync(RequestResultAsync requestresult, T parsedOutput) : base(requestresult)
    {
        ParsedOutput = parsedOutput;
    }

    public ParsedRequestResultAsync(ParsedRequestResult<T> requestresult) : base(requestresult)
    {
        ParsedOutput = requestresult.ParsedOutput;
    }

    public async Task<RequestResultAsync> Then(Func<ParsedRequestResultAsync<T>, LLMRequestAsync> funcForNextRequest)
    {
        return await funcForNextRequest(this).Complete();
    }

    public RequestResultAsync ThenConditional(Func<ParsedRequestResultAsync<T>, bool> condition, Func<ParsedRequestResultAsync<T>, ParsedRequestResultAsync<T>> funcForNextRequest)
    {
        if (condition(this))
            return funcForNextRequest(this);

        return this;
    }

    public async Task<IEnumerable<RequestResultAsync>> ThenBranching(Func<ParsedRequestResultAsync<T>, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var nextRequests = funcForNextRequest(this);
        return await Task.WhenAll(nextRequests.Select(async r => await r.Complete()));
    }

    public async Task<(IEnumerable<RequestResultAsync> Passed, IEnumerable<RequestResultAsync> Failed)> ThenBranchingConditionalAsync(Func<RequestResultAsync, bool> condition, Func<ParsedRequestResultAsync<T>, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var results = await Task.WhenAll(funcForNextRequest(this).Select(async r => await r.Complete()));
        return Partition(results, condition);
    }
}