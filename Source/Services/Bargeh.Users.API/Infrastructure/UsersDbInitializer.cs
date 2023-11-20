using Microsoft.EntityFrameworkCore;
using Users.API.Models;

namespace Users.API.Infrastructure;

public class UsersDbInitializer
{
	public static async Task InitializeDbAsync (IServiceScope scope, ILogger logger)
	{
		var context = scope.ServiceProvider.GetService<UsersContext> ();

		if (context == null)
		{
			logger.LogError ("UsersContext was null. UsersDbInitializer exits.");
			return;
		}

		//await context.Database.MigrateAsync ();

		if (!context.Users.Any ())
		{
			User user = new ()
			{
				Username = "test",
				DisplayName = "test display name",
				Email = "test@gmail.bargeh",
				VerificationCode = "0",
				Password = "".Hash (HashType.SHA256),
				PhoneNumber = "09123456789"
			};

			context.Add (user);

			await context.SaveChangesAsync ();

			logger.LogDebug ("Users database initialization completed successfully");
		}
	}
}