using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Request.Async;

public class RequestResultAsync : RequestResult
{
    public RequestResultAsync(string rawOutput, FinishReason finishReason) : base(rawOutput, finishReason)
    {
    }

    public RequestResultAsync(string rawOutput, FinishReason finishReason, ResultAdditionalData additionalData) : base(rawOutput, finishReason, additionalData)
    {
    }

    public RequestResultAsync(RequestResult result) : base(result)
    {
    }

    public ParsedRequestResultAsync<T> Parse<T>(ToObjectConverter<T> converter)
    {
        ParsedRequestResultAsync<T> parsedResult = new(this, converter.Convert(RawOutput));
        return parsedResult;
    }

    public ParsedRequestResultAsync<T> Parse<T>(Func<RequestResultAsync, T> converter)
    {
        ParsedRequestResultAsync<T> parsedResult = new(this, converter(this));
        return parsedResult;
    }

    public async Task<RequestResultAsync> Then(Func<RequestResultAsync, LLMRequestAsync> funcForNextRequest)
    {
        return await funcForNextRequest(this).CompleteAsync();
    }

    public async Task<RequestResultAsync> ThenConditional(Func<RequestResultAsync, bool> condition, Func<RequestResultAsync, LLMRequestAsync> funcForNextRequest)
    {
        if (condition(this))
            return await funcForNextRequest(this).CompleteAsync();

        return this;
    }

    public async Task<IEnumerable<RequestResultAsync>> ThenBranching(Func<RequestResultAsync, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var nextRequests = funcForNextRequest(this);
       return await Task.WhenAll(nextRequests.Select(async r => await r.CompleteAsync()));
    }

    public async Task<(IEnumerable<RequestResultAsync> Passed, IEnumerable<RequestResultAsync> Failed)> ThenBranchingConditionalAsync(Func<RequestResultAsync, bool> condition, Func<RequestResultAsync, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        var results = await Task.WhenAll(funcForNextRequest(this).Select(async r => await r.CompleteAsync()));
        return Partition(results, condition);
    }
}
