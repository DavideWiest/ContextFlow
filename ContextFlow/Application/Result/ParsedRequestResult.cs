using ContextFlow.Application.Result.Actions;

namespace ContextFlow.Application.Result;

public class ParsedRequestResult<T> : RequestResult
{
    /// <summary>
    /// The output parsed from the original RequestResult
    /// </summary>
    public T ParsedOutput { get; }
    /// <summary>
    /// Synchronous actions based on the parsed output
    /// </summary>
    public new ParsedRequestResultActions<T> Actions { get; }
    /// <summary>
    /// Asynchronous action based on the parsed output
    /// </summary>
    public new ParsedRequestResultActionsAsync<T> AsyncActions { get; }

    internal ParsedRequestResult(RequestResult result, T parsedOutput) : base(result)
    {
        ParsedOutput = parsedOutput;
        Actions = new(this);
        AsyncActions = new(this);
    }

}
