using Microsoft.EntityFrameworkCore;

namespace Bargeh.Topics.Api.Infrastructure;

public static class TopicsDbInitializer
{
	public static async Task InitializeDbAsync (TopicsDbContext topicsDbContext, ILogger logger)
	{
		byte retires = 20;

		TryConnect:

		if (!await topicsDbContext.Database.CanConnectAsync () && retires >= 1)
		{
			await Task.Delay (1000);
			logger.LogInformation ("Unable to connect to the database, retrying...");
			retires--;

			goto TryConnect;
		}

		try
		{
			await topicsDbContext.Database.MigrateAsync ();
		}
		catch
		{
			// ignored
		}

		logger.LogDebug ("Topics database initialization completed successfully");
	}
}