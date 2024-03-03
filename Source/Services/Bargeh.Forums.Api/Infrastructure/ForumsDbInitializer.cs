using Microsoft.EntityFrameworkCore;

namespace Bargeh.Forums.Api.Infrastructure;

public static class ForumsDbInitializer
{
	public static async Task InitializeDbAsync (ForumsDbContext forumsDbContext, ILogger logger)
	{
		byte retires = 20;

		TryConnect:

		if (!await forumsDbContext.Database.CanConnectAsync () && retires >= 1)
		{
			await Task.Delay (1000);
			logger.LogInformation ("Unable to connect to the database, retrying...");
			retires--;

			goto TryConnect;
		}

		try
		{
			await forumsDbContext.Database.MigrateAsync ();
		}
		catch
		{
			// ignored
		}

		logger.LogDebug ("Forums database initialization completed successfully");
	}
}