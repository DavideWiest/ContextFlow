using ContextFlow.Application.Result;
using ContextFlow.Domain;
using Microsoft.EntityFrameworkCore;

namespace ContextFlow.Application.Storage.Db;

/// <summary>
/// The data class that is used to save and load RequestResults to and from a database context.
/// </summary>
[PrimaryKey(nameof(PromptHash), nameof(LLMConfigHash))]
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
