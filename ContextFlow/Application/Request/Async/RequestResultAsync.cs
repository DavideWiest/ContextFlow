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

    public async Task<RequestResult> ThenLinear(Func<RequestResultAsync, LLMRequestAsync> funcForNextRequest)
    {
        return funcForNextRequest(this).Complete();
    }

    public async Task<RequestResultAsync?> ThenLinearCondititonal(Func<RequestResultAsync, bool> condition, Func<RequestResultAsync, LLMRequestAsync> funcForNextRequest)
    {
        if (condition(this))
            return funcForNextRequest(this).Complete();

        return null;
    }

    public async Task<IEnumerable<RequestResultAsync>> ThenBranching(Func<RequestResultAsync, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        return funcForNextRequest(this).Select(r => r.Complete());
    }

    public async Task<IEnumerable<RequestResultAsync>> ThenBranchingConditional(Func<RequestResultAsync, bool> condition, Func<RequestResultAsync, IEnumerable<LLMRequestAsync>> funcForNextRequest)
    {
        return funcForNextRequest(this).Select(r => r.Complete()).Where(res => condition(res));
    }

}
