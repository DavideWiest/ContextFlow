using ContextFlow.Application.Request;
using ContextFlow.Application.TextUtil;

namespace ContextFlow.Application.Result.Actions;

/// <summary>
/// Synchronous actions available for a RequestResult
/// </summary>
public class RequestResultActions
{
    /// <summary>
    /// The RequestResult on which the methods run
    /// </summary>
    protected RequestResult Result { get; }

    internal RequestResultActions(RequestResult result)
    {
        Result = result;
    }

    /// <summary>
    /// Converts a normal request into a ParsedRequestResult with a ToObjectConverter class.
    /// A ParsedRequestResult has the advantage of implementing the control methods of a normal RequestResult with its parsed content.
    /// </summary>
    /// <typeparam name="T">The type of the returned content inside a ParsedRequestResult</typeparam>
    /// <param name="converter"></param>
    /// <returns>The parsed result</returns>
    public ParsedRequestResult<T> Parse<T>(ToObjectConverter<T> converter)
    {
        ParsedRequestResult<T> parsedResult = new(Result, converter.Convert(Result.RawOutput));
        return parsedResult;
    }

    /// <summary>
    /// Converts a normal request into a ParsedRequestResult with a function.
    /// A ParsedRequestResult has the advantage of implementing the control methods of a normal RequestResult with its parsed content.
    /// </summary>
    /// <typeparam name="T">The type of the returned content inside a ParsedRequestResult</typeparam>
    /// <param name="converter"></param>
    /// <returns>The parsed result</returns>
    public ParsedRequestResult<T> Parse<T>(Func<RequestResult, T> converter)
    {
        ParsedRequestResult<T> parsedResult = new(Result, converter(Result));
        return parsedResult;
    }

    /// <summary>
    /// saves the result to a variable. This exists to save a result for later without disrupting the fluent interface control flow
    /// </summary>
    /// <param name="varToSaveTo">The variable which will store the result</param>
    /// <returns>The RequestResult of the actions class</returns>
    public RequestResult SaveAndContinue(out RequestResult varToSaveTo)
    {
        varToSaveTo = Result;
        return Result;
    }

    /// <summary>
    /// Returns the next result of an operation that has been build on the given function combined with this RequestResult
    /// </summary>
    /// <param name="funcForNextRequest">The function used to build the next request from this RequestResult</param>
    /// <returns>The result of the request</returns>
    public virtual RequestResult Then(Func<RequestResult, LLMRequest> funcForNextRequest)
    {
        return funcForNextRequest(Result).Complete();
    }

    /// <summary>
    /// If the given condition applies to this RequestResult, the function is executed on it and the result returned. 
    /// If the given condiition does not apply, the current result is returned. 
    /// This is useful to verify the output is valid, and to prompt the LLM to correct it in case it's not.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="funcForNextRequest"></param>
    /// <returns>The resulting or current result depending on the condition</returns>
    public virtual RequestResult ThenConditional(Func<RequestResult, bool> condition, Func<RequestResult, LLMRequest> funcForNextRequest)
    {
        if (condition(Result))
            return funcForNextRequest(Result).Complete();

        return Result;
    }

    /// <summary>
    /// Similar to Then, but with multiple resulting requests.
    /// </summary>
    /// <param name="funcForNextRequests">The function that builds the next requests</param>
    /// <returns>An enumerable containing the results</returns>
    public virtual IEnumerable<RequestResult> ThenBranching(Func<RequestResult, IEnumerable<LLMRequest>> funcForNextRequests)
    {
        return funcForNextRequests(Result).Select(r => r.Complete());
    }

    /// <summary>
    /// Executes multiple requests based on the current one, and filters them based on a given condition.
    /// </summary>
    /// <param name="condition">The condition the results go into the Passed enumerable.</param>
    /// <param name="funcForNextRequests"></param>
    /// <returns>The results separated by the outcome of the condition</returns>
    public virtual (IEnumerable<RequestResult> Passed, IEnumerable<RequestResult> Failed) ThenBranchingConditional(Func<RequestResult, bool> condition, Func<RequestResult, IEnumerable<LLMRequest>> funcForNextRequests)
    {
        var results = funcForNextRequests(Result).Select(result => result.Complete());
        return ActionsUtil.Partition(results, condition);
    }

}
