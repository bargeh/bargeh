using Microsoft.EntityFrameworkCore;

namespace Bargeh.Sms.Api.Infrastructure;

public static class SmsDbInitializer
{
	public static async Task InitializeDbAsync (SmsDbContext smsDbContext, ILogger logger)
	{
		TryConnect:

		try
		{
			await smsDbContext.Database.MigrateAsync ();
		}
		catch
		{
			goto TryConnect;
		}

		logger.LogDebug ("Users database initialization completed successfully");
	}
}