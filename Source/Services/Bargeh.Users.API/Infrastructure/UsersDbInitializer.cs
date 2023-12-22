using Bargeh.Users.API.Models;

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

		// context.Database.MigrateAsync();

		if (!context.Users.Any ())
		{
			User user = new ()
			{
				Id = new ("9844fd47-3236-46cb-898d-607b5c5563c1"),
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