using Microsoft.EntityFrameworkCore;

namespace Bargeh.Users.Api.Infrastructure;

public static class UsersDbInitializer
{
    public static async Task InitializeDbAsync (UsersContext? context, ILogger logger)
    {
        if (context == null)
        {
            logger.LogError ("UsersContext was null. UsersDbInitializer exits.");
            return;
        }

        byte retires = 20;

    TryConnect:

        if (!await context.Database.CanConnectAsync () && retires >= 1)
        {
            await Task.Delay (1000);
            logger.LogInformation ("Unable to connect to the database, retrying...");
            retires--;

            goto TryConnect;
        }

        await context.Database.MigrateAsync ();

        logger.LogDebug ("Users database initialization completed successfully");
    }
}