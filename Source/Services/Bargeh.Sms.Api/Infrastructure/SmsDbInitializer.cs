using Microsoft.EntityFrameworkCore;

namespace Bargeh.Sms.Api.Infrastructure;

public static class SmsDbInitializer
{
	public static async Task InitializeDbAsync (SmsDbContext smsDbContext, ILogger logger)
	{
		byte retires = 20;

		TryConnect:

		if (!await smsDbContext.Database.CanConnectAsync () && retires >= 1)
		{
			await Task.Delay (1000);
			logger.LogInformation ("Unable to connect to the database, retrying...");
			retires--;

			goto TryConnect;
		}

		try
		{
			await smsDbContext.Database.MigrateAsync ();
		}
		catch
		{
			// ignored
		}

		logger.LogDebug ("Users database initialization completed successfully");
	}
}