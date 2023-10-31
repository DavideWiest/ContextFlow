using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using Microsoft.EntityFrameworkCore;

namespace ContextFlow.Application.Storage.Db;

/// <summary>
/// Uses a DbContext to load requests. The DbContext must contain DbSavableRequest to be compatible.
/// </summary>
public class DbRequestLoader : RequestLoader
{
    private readonly DbContext _context;
    public bool ConsiderLLMConfig { get; set; }

    public DbRequestLoader(DbContext context, bool considerLLMConfig = true)
    {
        _context = context;
        DbRequestUtil.Validate(_context);
        ConsiderLLMConfig = considerLLMConfig;
    }

    public override bool MatchExists(LLMRequest request)
    {
        (string key1, string key2) = RequestHasher.GenerateKeys(request);

        // Check if the match exists in the database
        var matchExists = _context.Set<DbSavableRequest>()
            .Any(req => req.PromptHash == key1 && (!ConsiderLLMConfig || req.LLMConfigHash == key2));

        return matchExists;
    }

    public override RequestResult LoadMatch(LLMRequest request)
    {
        (string key1, string key2) = RequestHasher.GenerateKeys(request);

        // Load the match from the database
        var dbRequest = _context.Set<DbSavableRequest>()
            .FirstOrDefault(req => req.PromptHash == key1 && req.LLMConfigHash == key2);

        if (dbRequest != null)
        {
            return dbRequest.ToRequestResult();
        }
        else
        {
            throw new InvalidOperationException("No matching request found in the database.");
        }
    }

}