using ContextFlow.Application.Result.Actions;

namespace ContextFlow.Application.Result;

public class ParsedRequestResult<T> : RequestResult
{
    public T ParsedOutput { get; }
    public new ParsedRequestResultActions<T> Actions { get; }
    public new ParsedRequestResultActionsAsync<T> AsyncActions { get; }

    public ParsedRequestResult(RequestResult result, T parsedOutput) : base(result)
    {
        ParsedOutput = parsedOutput;
        Actions = new(this);
        AsyncActions = new(this);
    }

}
