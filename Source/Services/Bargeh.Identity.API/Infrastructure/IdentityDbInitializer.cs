using Microsoft.EntityFrameworkCore;

namespace Bargeh.Identity.API.Infrastructure;

public static class IdentityDbInitializer
{
	public static async Task InitializeDbAsync (IServiceScope scope, ILogger logger)
	{
		IdentityDbContext? context = scope.ServiceProvider.GetService<IdentityDbContext> ();

		if (context == null)
		{
			logger.LogError ("IdentityDbContext was null. IdentityDbInitializer exits.");
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