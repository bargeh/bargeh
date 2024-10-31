using Bargeh.Users.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Users.Api.Infrastructure;

public static class UsersDbInitializer
{
	public static async Task InitializeDbAsync(UsersDbContext dbContext, ILogger logger)
	{
		Retry:

		try
		{
			await dbContext.Database.MigrateAsync();

			if(!await dbContext.Users.AnyAsync())
			{
				User user = new()
				{
					Id = new("9844fd47-3236-46cb-898d-607b5c5563c1"),
					Username = "test",
					DisplayName = "test display name",
					Email = "test@gmail.bargeh",
					Password = "5".Hash(HashType.SHA256),
					PhoneNumber = "09123456789"
				};

				await dbContext.AddAsync(user);

				await dbContext.SaveChangesAsync();
			}

			logger.LogDebug("Users database initialization completed successfully");
		}
		catch
		{
			goto Retry;
		}
	}
}
