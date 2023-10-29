using ContextFlow.Application.Request;
using ContextFlow.Application.TextUtil;

namespace ContextFlow.Application.Result.Actions;

public class RequestResultActions
{
    protected RequestResult Result { get; }

    internal RequestResultActions(RequestResult result)
    {
        Result = result;
    }

    public ParsedRequestResult<T> Parse<T>(ToObjectConverter<T> converter)
    {
        ParsedRequestResult<T> parsedResult = new(Result, converter.Convert(Result.RawOutput));
        return parsedResult;
    }

    public ParsedRequestResult<T> Parse<T>(Func<RequestResult, T> converter)
    {
        ParsedRequestResult<T> parsedResult = new(Result, converter(Result));
        return parsedResult;
    }

    public RequestResult SaveAndContinue(out RequestResult varToSaveTo)
    {
        varToSaveTo = Result;
        return Result;
    }

    public virtual RequestResult Then(Func<RequestResult, LLMRequest> funcForNextRequest)
    {
        return funcForNextRequest(Result).Complete();
    }

    public virtual RequestResult ThenConditional(Func<RequestResult, bool> condition, Func<RequestResult, LLMRequest> funcForNextRequest)
    {
        if (condition(Result))
            return funcForNextRequest(Result).Complete();

        return Result;
    }

    public virtual IEnumerable<RequestResult> ThenBranching(Func<RequestResult, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        return funcForNextRequest(Result).Select(r => r.Complete());
    }

    public virtual (IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed) ThenBranchingConditional(Func<RequestResult, bool> condition, Func<RequestResult, IEnumerable<LLMRequest>> funcForNextRequest)
    {
        var results = funcForNextRequest(Result).Select(result => result.Complete());
        return ActionsUtil.Partition(results, condition);
    }

}
