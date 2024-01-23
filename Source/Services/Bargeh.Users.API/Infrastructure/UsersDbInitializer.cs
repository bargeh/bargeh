using Bargeh.Users.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Users.Api.Infrastructure;

public static class UsersDbInitializer
{
    public static async Task InitializeDbAsync (UsersContext context, ILogger logger)
    {
        byte retires = 20;

        TryConnect:

        if (!await context.Database.CanConnectAsync () && retires >= 1)
        {
            await Task.Delay (1000);
            logger.LogInformation ("Unable to connect to the database, retrying...");
            retires--;

            goto TryConnect;
        }

        try
        {
            await context.Database.MigrateAsync ();
        }
        catch
        {
            // ignored
        }

        if (!context.Users.Any ())
        {
            User user = new ()
            {
                Id = new ("9844fd47-3236-46cb-898d-607b5c5563c1"),
                Username = "test",
                DisplayName = "test display name",
                Email = "test@gmail.bargeh",
                VerificationCode = "0",
                Password = "5".Hash (HashType.SHA256),
                PhoneNumber = "09123456789"
            };

            context.Add (user);

            await context.SaveChangesAsync ();

        }

        logger.LogDebug ("Users database initialization completed successfully");
    }
}