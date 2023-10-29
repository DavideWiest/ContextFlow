using Microsoft.EntityFrameworkCore;

namespace ContextFlow.Application.Storage.Db;

internal static class DbRequestUtil
{

    public static void Validate(DbContext context)
    {
        Type entityTypeToCheck = typeof(DbSavableRequest);

        var model = context.Model;

        // Check if the entity type is in the model
        if (!model.GetEntityTypes().Any(entity => entity.ClrType == entityTypeToCheck))
        {
            throw new InvalidOperationException($"{entityTypeToCheck.Name} is not supported by the DbContext.");
        }
    }
}
