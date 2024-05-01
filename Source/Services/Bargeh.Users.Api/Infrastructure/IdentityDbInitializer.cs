using Microsoft.EntityFrameworkCore;

namespace Bargeh.Users.Api.Infrastructure;

public static class IdentityDbInitializer
{
	public static async Task InitializeDbAsync (UsersDbContext dbContext, ILogger logger)
	{
		TryConnect:

		try
		{
			await dbContext.Database.MigrateAsync();
		}
		catch
		{
			goto TryConnect;
		}

		logger.LogDebug ("Users database initialization completed successfully");
	}
}