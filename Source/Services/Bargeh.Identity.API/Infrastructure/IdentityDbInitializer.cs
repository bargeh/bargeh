using Microsoft.EntityFrameworkCore;

namespace Bargeh.Identity.Api.Infrastructure;

public static class IdentityDbInitializer
{
    public static async Task InitializeDbAsync (IdentityDbContext identityDbContext, ILogger logger)
    {
        byte retires = 20;

        TryConnect:

        if (!await identityDbContext.Database.CanConnectAsync () && retires >= 1)
        {
            await Task.Delay (1000);
            logger.LogInformation ("Unable to connect to the database, retrying...");
            retires--;

            goto TryConnect;
        }

        await identityDbContext.Database.MigrateAsync ();

        logger.LogDebug ("Users database initialization completed successfully");
    }
}