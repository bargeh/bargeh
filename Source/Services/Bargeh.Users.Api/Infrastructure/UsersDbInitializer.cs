using Bargeh.Users.Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Users.Api.Infrastructure;

public static class UsersDbInitializer
{
	public static async Task InitializeDbAsync(UsersDbContext dbContext, ILogger logger)
	{
		byte retires = 20;

		TryConnect:

		if(!await dbContext.Database.CanConnectAsync() && retires >= 1)
		{
			await Task.Delay(1000);
			logger.LogInformation("Unable to connect to the database, retrying...");
			retires--;

			goto TryConnect;
		}

		try
		{
			await dbContext.Database.MigrateAsync();
		}
		catch
		{
			// ignored
		}

		if(!dbContext.Users.Any())
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

			dbContext.Add(user);

			await dbContext.SaveChangesAsync();
		}

		logger.LogDebug("Users database initialization completed successfully");
	}
}