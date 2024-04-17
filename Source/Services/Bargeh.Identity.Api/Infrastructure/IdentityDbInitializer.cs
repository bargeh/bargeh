using Microsoft.EntityFrameworkCore;

namespace Bargeh.Identity.Api.Infrastructure;

public static class IdentityDbInitializer
{
	public static async Task InitializeDbAsync (IdentityDbContext identityDbContext, ILogger logger)
	{
		TryConnect:

		try
		{
			await identityDbContext.Database.MigrateAsync();
		}
		catch
		{
			goto TryConnect;
		}

		logger.LogDebug ("Users database initialization completed successfully");
	}
}