using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using Microsoft.EntityFrameworkCore;

namespace ContextFlow.Application.Storage.Db;


public class DbRequestSaver : RequestSaver
{
    private readonly DbContext _context;
    private readonly RequestHasher RequestHasher = new();

    public DbRequestSaver(DbContext context)
    {
        _context = context;
        DbRequestUtil.Validate(_context);
    }

    public override void SaveRequest(LLMRequest request, RequestResult result)
    {
        (string key1, string key2) = RequestHasher.GenerateKeys(request);

        SaveToDatabase(new DbSavableRequest(key1, key2, result));
    }

    protected void SaveToDatabase(DbSavableRequest entity)
    {
        // Add the entity to the context
        _context.Add(entity);

        // Save changes to the database
        _context.SaveChanges();
    }
}