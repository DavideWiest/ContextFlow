using ContextFlow.Application.Result;
using ContextFlow.Domain;

namespace ContextFlow.Application.Storage.Db;

public class DbSavableRequest
{
    public string PromptHash = default!;
    public string LLMConfigHash = default!;
    public string RawOutput = default!;
    public int FinishReason;

    public DbSavableRequest() { }
    public DbSavableRequest(string promptHash, string lLMConfigHash, RequestResult result)
    {
        PromptHash = promptHash;
        LLMConfigHash = lLMConfigHash;
        RawOutput = result.RawOutput;
        FinishReason = (int)result.FinishReason;
    }

    public RequestResult ToRequestResult()
    {
        return new RequestResult(RawOutput, (FinishReason)FinishReason);
    }
}
