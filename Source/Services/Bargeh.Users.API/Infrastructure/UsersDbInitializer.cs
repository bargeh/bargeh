using Bargeh.Users.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Bargeh.Users.API.Infrastructure;

public static class UsersDbInitializer
{
	public static async Task InitializeDbAsync (IServiceScope scope, ILogger logger)
	{
		UsersContext? context = scope.ServiceProvider.GetService<UsersContext> ();

		if (context == null)
		{
			logger.LogError ("UsersContext was null. UsersDbInitializer exits.");
			return;
		}

		byte retires = 20;

	TryConnect:

		if (!await context.Database.CanConnectAsync () && retires >= 1)
		{
			await Task.Delay (1000);
			logger.LogInformation("Unable to connect to the database, retrying...");
			retires--;

			goto TryConnect;
		}

		await context.Database.MigrateAsync ();

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

			logger.LogDebug ("Users database initialization completed successfully");
		}
	}
}